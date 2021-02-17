namespace TRuDI.Backend.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Newtonsoft.Json;

    using Serilog;

    using TRuDI.Backend.Exceptions;
    using TRuDI.Backend.MessageHandlers;
    using TRuDI.Backend.Models;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.HanAdapter.Repository;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;
    using TRuDI.TafAdapter.Repository;

    using AnalysisProfile = TRuDI.Models.CheckData.AnalysisProfile;

    /// <summary>
    /// This class is used to manage the current state of the application.
    /// It's instantiated as singleton instance.
    /// </summary>
    public class ApplicationState
    {
        /// <summary>
        /// The notifications message handler used to send progress notification to the progress page.
        /// </summary>
        private readonly NotificationsMessageHandler notificationsMessageHandler;

        /// <summary>
        /// The backend checksums calculated at application startup.
        /// </summary>
        private ApplicationChecksums backendChecksums;

        /// <summary>
        /// The active HAN adapter selected by the user.
        /// </summary>
        private HanAdapterContainer activeHanAdapter;

        /// <summary>
        /// The cancellation token that is used to cancel HAN adapter operations.
        /// </summary>
        private CancellationTokenSource cts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationState" /> class.
        /// </summary>
        /// <param name="notificationsMessageHandler">The notifications message handler.</param>
        public ApplicationState(NotificationsMessageHandler notificationsMessageHandler)
        {
            this.notificationsMessageHandler = notificationsMessageHandler;

            this.BreadCrumbTrail.Add("Start", "/OperatingModeSelection", false);
            this.Reset();
        }

        /// <summary>
        /// Gets or sets the current active operation mode (display or transparency function).
        /// </summary>
        public OperationMode OperationMode { get; set; } = OperationMode.NotSelected;

        /// <summary>
        /// Gets the bread crumb trail.
        /// </summary>
        public BreadCrumbTrail BreadCrumbTrail { get; } = new BreadCrumbTrail();

        /// <summary>
        /// Gets the side bar menu of the current page.
        /// </summary>
        public SideBarMenu SideBarMenu { get; } = new SideBarMenu();

        /// <summary>
        /// Gets the last error messages.
        /// </summary>
        public List<string> LastErrorMessages { get; } = new List<string>();

        /// <summary>
        /// Gets the active han adapter.
        /// </summary>
        public HanAdapterContainer ActiveHanAdapter => this.activeHanAdapter;

        /// <summary>
        /// Gets or sets the connect data.
        /// </summary>
        public ConnectData ConnectData { get; set; }

        /// <summary>
        /// Gets or sets the client certificate used for the TLS connection to the SMGW.
        /// </summary>
        public CertData ClientCert { get; set; }

        /// <summary>
        /// Gets or sets the dictionary with HAN adapter specific parameters.
        /// </summary>
        public Dictionary<string, string> ManufacturerParameters { get; set; }

        /// <summary>
        /// Gets the current progress state relevant for the progress page.
        /// </summary>
        public ProgressData CurrentProgressState { get; } = new ProgressData();

        /// <summary>
        /// Gets the last connect result of the active HAN adapter.
        /// </summary>
        public ConnectResult LastConnectResult { get; private set; }

        /// <summary>
        /// Gets the current list of contracts read from the SMGW.
        /// </summary>
        public IReadOnlyList<ContractContainer> Contracts { get; private set; }

        /// <summary>
        /// Gets the current adapter context used to load the data from the SMGW.
        /// </summary>
        public AdapterContext CurrentAdapterContext { get; private set; }

        /// <summary>
        /// Gets the current data result loaded by the HAN adapter from the SMGW.
        /// </summary>
        public XmlDataResult CurrentDataResult { get; private set; }

        /// <summary>
        /// Gets or sets the current supplier file containing the analysis profile.
        /// </summary>
        public SupplierFile CurrentSupplierFile { get; set; }

        /// <summary>
        /// Gets the backend checksums calculated at application startup.
        /// </summary>
        public ApplicationChecksums BackendChecksums
        {
            get
            {
                // Because calculation takes some time, calculate it 
                // on demand to speed up application startup.
                if (this.backendChecksums == null)
                {
                    this.backendChecksums = new ApplicationChecksums();
                    this.backendChecksums.Calculate();
                }

                return this.backendChecksums;
            }
        }

        /// <summary>
        /// Loads the HAN adapter for the specified server ID.
        /// </summary>
        /// <param name="serverId">The server identifier to load the HAN adapter for.</param>
        public void LoadAdapter(string serverId)
        {
            this.activeHanAdapter = new HanAdapterContainer(HanAdapterRepository.LoadAdapter(serverId), serverId);
        }

        /// <summary>
        /// Gets the specified file from the embedded resources of the current active HAN adapter (e.g. for the SMGW image).
        /// </summary>
        /// <param name="path">The path to the resource file.</param>
        /// <returns>The file content and it's content type.</returns>
        /// <exception cref="InvalidOperationException">Is thrown if there's no active HAN adapter.</exception>
        public (byte[] data, string contentType) GetResourceFile(string path)
        {
            if (this.activeHanAdapter == null)
            {
                throw new InvalidOperationException($"There's no active HAN adapter to get resource files: {path}");
            }

            return this.activeHanAdapter.GetResourceFile(path);
        }

        /// <summary>
        /// Loads the contracts list from the SMGW.
        /// </summary>
        public void ConnectAndLoadContracts()
        {
            Log.Information("Connecting to the SMGW and loading TAF contracts");

            this.LoadAdapter(this.ConnectData.DeviceId);

            this.CurrentProgressState.Reset("Verbindungsaufbau", "_ConnectingPartial");
            this.cts = new CancellationTokenSource();
            var ct = this.cts.Token;

            this.BreadCrumbTrail.RemoveUnselectedItems();

            Task.Run(async () =>
                {
                    this.LastErrorMessages.Clear();

                    try
                    {
                        Log.Information("Connecting to the SMGW {0}", this.ConnectData.DeviceId);
                        this.LastConnectResult = await this.activeHanAdapter.Connect(
                                                     this.ConnectData,
                                                     this.ClientCert,
                                                     this.ManufacturerParameters,
                                                     ct,
                                                     this.ProgressCallback);

                        Log.Information("Loading available contracts from SMGW {0}", this.ConnectData.DeviceId);
                        var contracts = await this.activeHanAdapter.LoadAvailableContracts(ct, this.ProgressCallback);

                        Log.Debug("Contracts received: {@Contracts}", contracts);

                        var containers = contracts.Where(c => c.TafId != TafId.Taf6)
                            .Select(c => new ContractContainer { Contract = c }).ToList();

                        Log.Debug("Grouping TAF-6 contracts");
                        foreach (var taf6Contract in contracts.Where(c => c.TafId == TafId.Taf6))
                        {
                            var cnt = containers.FirstOrDefault(
                                c => taf6Contract.TafName == c.Contract.TafName
                                     && taf6Contract.Begin == c.Contract.Begin && taf6Contract.End == c.Contract.End
                                     && taf6Contract.SupplierId == c.Contract.SupplierId
                                     && taf6Contract.Meters.SequenceEqual(c.Contract.Meters));

                            if (cnt != null)
                            {
                                cnt.Taf6 = taf6Contract;
                            }
                            else
                            {
                                containers.Add(new ContractContainer { Contract = taf6Contract });
                            }
                        }

                        this.Contracts = containers;

                        if (this.OperationMode == OperationMode.DisplayFunction)
                        {
                            Log.Information("Display function: showing contract list");
                            await this.LoadNextPageAfterProgress("/Contracts");
                        }
                        else
                        {
                            Log.Information("Transparency function: search matching contract for metering point {0} and TAF name {1}",
                                this.CurrentSupplierFile.Model.UsagePointId,
                                this.CurrentSupplierFile.Model.AnalysisProfile.TariffId);

                            var taf7Contracts =
                                this.Contracts.Select(c => c.Contract).Where(c => c.TafId == TafId.Taf7).ToList();

                            if (!taf7Contracts.Any())
                            {
                                Log.Error("No contract found: no TAF-7 contract found");
                                this.LastErrorMessages.Add($"Vertrag mit der ID \"{this.CurrentSupplierFile.Model.AnalysisProfile.TariffId}\" für Messlokation \"{this.CurrentSupplierFile.Model.UsagePointId}\" konnte nicht im Smart Meter Gateway gefunden werden: Kein TAF-7-Profil vorhanden.");
                                await this.LoadNextPageAfterProgress("/Error");
                                return;
                            }

                            var tariffContract = taf7Contracts.FirstOrDefault(c =>
                                    c.MeteringPointId == this.CurrentSupplierFile.Model.UsagePointId
                                    && c.TafName == this.CurrentSupplierFile.Model.AnalysisProfile.TariffId);

                            if (tariffContract == null)
                            {
                                Log.Error("No contract found");
                                this.LastErrorMessages.Add($"Vertrag mit der ID \"{this.CurrentSupplierFile.Model.AnalysisProfile.TariffId}\" für Messlokation \"{this.CurrentSupplierFile.Model.UsagePointId}\" konnte nicht im Smart Meter Gateway gefunden werden.");
                                await this.LoadNextPageAfterProgress("/Error");
                                return;
                            }

                            this.CurrentSupplierFile.Ctx.Contract = tariffContract;
                            this.CurrentAdapterContext = this.CurrentSupplierFile.Ctx;

                            Log.Information("Loading TAF-7 data from SMGW");
                            this.LoadData(this.CurrentSupplierFile.Ctx);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Log.Warning("Connect/Loading contracts canceled");
                        await this.LoadNextPageAfterProgress("/Connect");
                    }
                    catch (HanAdapterException ex)
                    {
                        this.HandleHanAdapterException(ex);
                        await this.LoadNextPageAfterProgress("/Error");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Connect/Loading contracts failed: {0}", ex.Message);
                        this.LastErrorMessages.Add(ex.Message);
                        await this.LoadNextPageAfterProgress("/Error");
                    }
                });
        }

        /// <summary>
        /// Loads the data from the specifed XML document.
        /// This is called if the user loads a XML file instead of connecting to the SMGW.
        /// </summary>
        /// <param name="raw">The XML document to load.</param>
        public void LoadData(XDocument raw)
        {
            this.LastErrorMessages.Clear();
            this.LoadDataFromXml(raw, null);

            if (this.CurrentDataResult?.Model?.Smgw?.SmgwId != null)
            {
                this.LoadAdapter(this.CurrentDataResult?.Model?.Smgw?.SmgwId);
            }
        }

        public void LoadData(AdapterContext ctx)
        {
            Log.Information("Loading data for metering point {0}, TAF name: {1}, Start: {2}, End: {3}", ctx.Contract.MeteringPointId, ctx.Contract.TafName, ctx.Start, ctx.End);
            Log.Verbose("Adapter context: {@AdapterContext}", ctx);

            if (this.activeHanAdapter == null)
            {
                throw new InvalidOperationException($"There's no active HAN adapter to load data");
            }

            this.CurrentAdapterContext = ctx;

            this.CurrentProgressState.Reset("Daten laden", "_LoadingDataPartial");
            this.cts = new CancellationTokenSource();
            var ct = this.cts.Token;

            Task.Run(async () =>
                {
                    this.LastErrorMessages.Clear();

                    try
                    {
                        Log.Information("Loading data from SMGW {0}", this.ConnectData.DeviceId);
                        var raw = await this.activeHanAdapter.LoadData(ctx, ct, this.ProgressCallback);

                        try
                        {
                            this.LoadDataFromXml(raw, ctx);
                        }
                        catch
                        {
                            // Errors are collected by LoadDataFromXml, just go to the error page now
                            await this.LoadNextPageAfterProgress("/ValidationError");
                            return;
                        }

                        if (!ctx.BillingPeriod.IsCompleted() && (ctx.Contract.Begin <= DateTime.Now && (ctx.Contract.End == null || ctx.Contract.End > DateTime.Now)))
                        {
                            Log.Information("Billing period not completed: get current register values", this.ConnectData.DeviceId);

                            XDocument rawCurrentRegisters = null;

                            try
                            {
                                rawCurrentRegisters =
                                    await this.activeHanAdapter.GetCurrentRegisterValues(
                                        ctx,
                                        ct,
                                        this.ProgressCallback);
                            }
                            catch (HanAdapterException ex)
                            {
                                if (ex.AdapterError.Type != ErrorType.SensorNotConnected &&
                                    ex.AdapterError.Type != ErrorType.NoDataInSelectedTimeRange &&
                                    ex.AdapterError.Type != ErrorType.DeviceError)
                                {
                                    throw;
                                }

                                this.HandleHanAdapterException(ex);
                            }

                            try
                            {

                                if (rawCurrentRegisters != null)
                                {
                                    this.UpdateRegisterValuesFromXml(rawCurrentRegisters, ctx);
                                }
                            }
                            catch
                            {
                                // Errors are collected by LoadDataFromXml, just go to the error page now
                                await this.LoadNextPageAfterProgress("/ValidationError");
                                return;
                            }
                        }

                        await this.LoadNextPageAfterProgress("/DataView");
                    }
                    catch (OperationCanceledException)
                    {
                        Log.Warning("Loading data canceled");
                        await this.LoadNextPageAfterProgress(this.BreadCrumbTrail.Items.Last(i => i.IsSelected).Link);
                    }
                    catch (HanAdapterException ex)
                    {
                        this.HandleHanAdapterException(ex);
                        await this.LoadNextPageAfterProgress("/Error");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Loading data from device {0} failed: {1}", this.ConnectData.DeviceId, ex.Message);
                        this.LastErrorMessages.Add(ex.Message);
                        await this.LoadNextPageAfterProgress("/Error");
                    }
                });
        }

        public void LoadSupplierXml()
        {
            this.LastErrorMessages.Clear();
            this.BreadCrumbTrail.RemoveUnselectedItems();

            try
            {
                Log.Information("Parsing supplier model");
                this.CurrentSupplierFile.Model = XmlModelParser.ParseSupplierModel(this.CurrentSupplierFile.Xml.Root.Descendants());

                Log.Information("Validating supplier model");
                ModelValidation.ValidateSupplierModel(this.CurrentSupplierFile.Model);
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Log.Error(err, err.Message);
                    this.LastErrorMessages.Add(err.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Loading supplier file failed: {0}", ex.Message);
                this.LastErrorMessages.Add("Keine gültige Datei.");
                throw;
            }
        }

        public void CancelOperation()
        {
            Log.Information("Operation canceld by the user.");
            this.cts?.Cancel();
        }

        public async Task LoadNextPageAfterProgress(string page)
        {
            Log.Debug("Next page after progress: {0}", page);
            this.CurrentProgressState.NextPageAfterProgress = page;
            await this.notificationsMessageHandler.LoadNextPage(page);
        }

        /// <summary>
        /// Resets the application state in case of an internal error.
        /// </summary>
        public void Reset()
        {
            this.ConnectData = new ConnectData();
            this.BreadCrumbTrail.Reset();

            this.SideBarMenu.Clear();
            this.SideBarMenu.Add("Über TRuDI", "/About", true);
            this.SideBarMenu.Add("Beschreibung", "/Help", true);

            this.activeHanAdapter = null;
            this.CurrentDataResult = null;
            this.CurrentSupplierFile = null;
            this.CurrentAdapterContext = null;
            this.CurrentProgressState.Reset();
            this.Contracts = null;
            this.CurrentSupplierFile = null;
            this.ClientCert = null;
            this.LastConnectResult = null;
            this.LastErrorMessages.Clear();
            this.ManufacturerParameters = null;
            this.OperationMode = OperationMode.NotSelected;
        }

        private void LoadDataFromXml(XDocument raw, AdapterContext ctx)
        {
            Log.Information("Loading XML file");
            this.LastErrorMessages.Clear();

            try
            {
                this.CurrentDataResult = new XmlDataResult { Raw = raw };

                Log.Information("Validating XSD schema");
                Ar2418Validation.ValidateSchema(raw);

                Log.Information("Parsing model");
                this.CurrentDataResult.Model = XmlModelParser.ParseHanAdapterModel(this.CurrentDataResult?.Raw?.Root?.Descendants());

                Log.Information("Validating model");
                ModelValidation.ValidateHanAdapterModel(this.CurrentDataResult.Model);

                Log.Information("Validating model using the adapter context");
                ContextValidation.ValidateContext(this.CurrentDataResult.Model, ctx);

                if (this.CurrentSupplierFile?.Model != null)
                {
                    Log.Information("Validating model using supplier file model");
                    ContextValidation.ValidateContext(this.CurrentDataResult.Model, this.CurrentSupplierFile.Model, ctx);

                    Log.Information("Loading TAF adapter: {0}", this.CurrentSupplierFile.Model.AnalysisProfile.TariffUseCase);
                    var tafAdapter = TafAdapterRepository.LoadAdapter(this.CurrentSupplierFile.Model.AnalysisProfile.TariffUseCase);
                    this.CurrentSupplierFile.TafData = tafAdapter.Calculate(this.CurrentDataResult.Model, this.CurrentSupplierFile.Model);
                }
            }
            catch (UnknownTafAdapterException ex)
            {
                Log.Error(ex, "Unknown TAF adapter: {0}", ex.TafId);
                this.LastErrorMessages.Add($"Die Berechnung des Tarifanwendungsfall {ex.TafId} wird nicht unterstützt.");
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Log.Error(err, err.Message);
                    this.LastErrorMessages.Add(err.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Loading XML data failed: {0}", ex.Message);
                this.LastErrorMessages.Add(ex.Message);
                throw;
            }

            var originalValueLists = 
                this.CurrentDataResult.Model.MeterReadings.Where(mr => mr.IsOriginalValueList()).Select(mr => new OriginalValueList(mr, this.CurrentDataResult.Model.ServiceCategory.Kind ?? Kind.Electricity)).ToList();

            var meterReadings = this.CurrentDataResult.Model.MeterReadings.Where(mr => !mr.IsOriginalValueList()).ToList();
            meterReadings.Sort((a, b) => string.Compare(a.ReadingType.ObisCode, b.ReadingType.ObisCode, StringComparison.InvariantCultureIgnoreCase));

            var ovlRegisters = originalValueLists.Where(ovl => ovl.MeterReading?.IntervalBlocks?.FirstOrDefault()?.Interval.Duration == 0).ToList();

            meterReadings.AddRange(ovlRegisters.Select(ovl => ovl.MeterReading));

            if (ovlRegisters.Count < originalValueLists.Count)
            {
                ovlRegisters.ForEach(ovl => originalValueLists.Remove(ovl));
            }

            foreach (var ovl in originalValueLists)
            {
                Log.Information("Original value list: meter: {0}, OBIS: {1}, {2} values", ovl.Meter, ovl.Obis, ovl.ValueCount);
            }

            foreach (var mr in meterReadings)
            {
                Log.Information("Meter reading: {@Meters}, OBIS: {1}", mr.Meters, mr.ReadingType.ObisCode);
            }

            this.CurrentDataResult.OriginalValueLists = originalValueLists;
            this.CurrentDataResult.MeterReadings = meterReadings;
            this.CurrentDataResult.Begin = meterReadings.FirstOrDefault()?.IntervalBlocks?.FirstOrDefault()?.Interval?.Start;

            if (this.CurrentDataResult.Begin != null)
            {
                var duration = meterReadings.FirstOrDefault()?.IntervalBlocks?.FirstOrDefault()?.Interval?.Duration;
                if (duration != null)
                {
                    this.CurrentDataResult.End = this.CurrentDataResult.Begin + TimeSpan.FromSeconds(duration.Value);
                }
            }

            // If the analysis profile is missing, add it based on the contract info
            if (this.CurrentDataResult.Model.AnalysisProfile == null && ctx?.Contract != null)
            {
                this.AddAnalysisProfile(ctx);
            }
        }

        private void AddAnalysisProfile(AdapterContext ctx)
        {
            Log.Information("Addding analysis profile");

            this.CurrentDataResult.Model.AnalysisProfile =
                new AnalysisProfile { TariffUseCase = ctx.Contract.TafId, TariffId = ctx.Contract.TafName, };

            var lowestTariffId = ushort.MaxValue;

            if (this.CurrentDataResult.MeterReadings.Count == 0)
            {
                var ts = new TariffStage
                {
                    ObisCode = "010000000FF",
                    TariffNumber = 0,
                    Description = string.Empty
                };
                lowestTariffId = 0;
                this.CurrentDataResult.Model.AnalysisProfile.TariffStages.Add(ts);
            }
            else
            {
                foreach (var mr in this.CurrentDataResult.MeterReadings)
                {
                    var ts = new TariffStage
                    {
                        ObisCode = mr.ReadingType.ObisCode,
                        TariffNumber = new ObisId(mr.ReadingType.ObisCode).E,
                        Description = string.Empty
                    };

                    lowestTariffId = Math.Min(lowestTariffId, ts.TariffNumber);
                    this.CurrentDataResult.Model.AnalysisProfile.TariffStages.Add(ts);
                }
            }

            this.CurrentDataResult.Model.AnalysisProfile.DefaultTariffNumber = lowestTariffId;

            DateTime begin;
            DateTime end;

            if (ctx.BillingPeriod != null)
            {
                begin = ctx.BillingPeriod.Begin;
                end = ctx.BillingPeriod.End ?? DateTime.Now;
            }
            else
            {
                begin = ctx.Contract.Begin;
                end = ctx.Contract.End ?? DateTime.Now;
            }

            this.CurrentDataResult.Model.AnalysisProfile.BillingPeriod =
                new Interval { Start = begin, Duration = (uint)(end - begin).TotalSeconds };

            XNamespace ns = @"http://vde.de/AR_2418-6.xsd";
            var tariffNameElement = this.CurrentDataResult?.Raw?.Root?.Descendants().FirstOrDefault(n => n.Name.LocalName == "tariffName");
            if (tariffNameElement == null)
            {
                return;
            }

            var analysisProfile = new XElement(ns + "AnalysisProfile",
                new XElement(ns + "tariffUseCase", (int)this.CurrentDataResult.Model.AnalysisProfile.TariffUseCase),
                new XElement(ns + "tariffId", this.CurrentDataResult.Model.AnalysisProfile.TariffId),
                new XElement(ns + "billingPeriod",
                    new XElement(ns + "duration", this.CurrentDataResult.Model.AnalysisProfile.BillingPeriod.Duration),
                    new XElement(ns + "start", this.CurrentDataResult.Model.AnalysisProfile.BillingPeriod.Start)));

            foreach (var ts in this.CurrentDataResult.Model.AnalysisProfile.TariffStages)
            {
                analysisProfile.Add(new XElement(ns + "TariffStage",
                    new XElement(ns + "tariffNumber", ts.TariffNumber),
                    new XElement(ns + "description", ts.Description),
                    new XElement(ns + "obisCode", ts.ObisCode))
                );
            }

            analysisProfile.Add(new XElement(ns + "defaultTariffNumber", this.CurrentDataResult.Model.AnalysisProfile.DefaultTariffNumber));
            tariffNameElement.AddAfterSelf(analysisProfile);
        }

        private void UpdateRegisterValuesFromXml(XDocument raw, AdapterContext ctx)
        {
            Log.Information("Adding the current register value to the XML result file.");

            this.LastErrorMessages.Clear();
            UsagePointAdapterTRuDI model;

            try
            {
                Log.Information("Validating XSD schema");
                Ar2418Validation.ValidateSchema(raw);

                Log.Information("Parsing model");
                model = XmlModelParser.ParseHanAdapterModel(raw?.Root?.Descendants());

                Log.Information("Validating model");
                ModelValidation.ValidateHanAdapterModel(model);
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Log.Error(err, err.Message);
                    this.LastErrorMessages.Add(err.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Loading XML data with current values failed: {0}", ex.Message);
                this.LastErrorMessages.Add(ex.Message);
                throw;
            }

            model.MeterReadings.Sort((a, b) => string.Compare(a.ReadingType.ObisCode, b.ReadingType.ObisCode, StringComparison.InvariantCultureIgnoreCase));
            this.CurrentDataResult.MeterReadings = model.MeterReadings;
            this.CurrentDataResult.Begin = model.MeterReadings.FirstOrDefault()?.IntervalBlocks?.FirstOrDefault()?.Interval?.Start;
            var usagePoint = this.CurrentDataResult.Raw.Root.Elements().FirstOrDefault();
            var readings = raw?.Root?.Elements().FirstOrDefault().Elements().Where(e => e.Name.LocalName == "MeterReading");
            usagePoint.Add(readings);

            if (this.CurrentDataResult.Begin != null)
            {
                var duration = model.MeterReadings.FirstOrDefault()?.IntervalBlocks?.FirstOrDefault()?.Interval?.Duration;
                if (duration != null)
                {
                    this.CurrentDataResult.End = this.CurrentDataResult.Begin + TimeSpan.FromSeconds(duration.Value);
                }
            }
        }

        private void ProgressCallback(ProgressInfo progressInfo)
        {
            this.CurrentProgressState.StatusText = progressInfo.Message;
            this.CurrentProgressState.Progress = progressInfo.Progress;

            this.notificationsMessageHandler.ProgressUpdate(progressInfo.Message, progressInfo.Progress).Wait();
        }

        /// <summary>
        /// Handles specified HAN adapter exception and adds a message to <see cref="LastErrorMessages"/>.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <exception cref="ArgumentOutOfRangeException">Unsupported <see cref="ErrorType"/>.</exception>
        private void HandleHanAdapterException(HanAdapterException ex)
        {
            Log.Error("HAN adapter error: {0}, message: {1}", ex.AdapterError.Type, ex.AdapterError.Message);

            switch (ex.AdapterError.Type)
            {
                case ErrorType.TcpConnectFailed:
                    this.LastErrorMessages.Add("Netzwerkverbindung zum Smart Meter Gateway konnte nicht hergestellt werden.");
                    this.LastErrorMessages.Add("Bitte überprüfen Sie die IP-Adresse sowie den Port.");

                    if (!string.IsNullOrWhiteSpace(ex.AdapterError?.Message))
                    {
                        this.LastErrorMessages.Add("Systemmeldung:");
                        this.LastErrorMessages.Add(ex.AdapterError.Message);
                    }

                    return;

                case ErrorType.TlsConnectFailed:
                    this.LastErrorMessages.Add("TLS-Verbindung zum Smart Meter Gateway konnte nicht hergestellt werden.");
                    break;

                case ErrorType.AuthenticationFailed:
                    this.LastErrorMessages.Add("Anmeldung am Smart Meter Gateway fehlgeschlagen.");
                    break;

                case ErrorType.NoTafProfileForUser:
                    this.LastErrorMessages.Add($"Für den Benutzer {(this.ConnectData.AuthMode == AuthMode.UserPassword ? this.ConnectData.Username : this.ClientCert.Subject)} sind keine Vertragsdaten im Smart Meter Gateway vorhanden.");
                    return;

                case ErrorType.DeviceError:
                    this.LastErrorMessages.Add("Fehler während der Kommunikation mit dem Smart Meter Gateway. Das Smart Meter Gateway lieferte folgenden Fehler zurück:");
                    break;

                case ErrorType.SensorNotConnected:
                    this.LastErrorMessages.Add("Das Smart Meter Gateway konnte keine Kommunikationsverbindung zum Zähler aufbauen.");
                    return;

                case ErrorType.NoDataInSelectedTimeRange:
                    if (this.CurrentAdapterContext != null)
                    {
                        this.LastErrorMessages.Add(
                            $"Im gewählten Zeitbereich ({this.CurrentAdapterContext.Start:G} bis {this.CurrentAdapterContext.End:G}) konnten keine Messdaten vom Smart Meter Gateway abgerufen werden.");
                    }
                    else
                    {
                        this.LastErrorMessages.Add(
                            "Im gewählten Zeitbereich konnten keine Messdaten vom Smart Meter Gateway abgerufen werden.");
                    }
                    break;

                case ErrorType.Other:
                    this.LastErrorMessages.Add("Nicht spezifizierter Fehler.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrWhiteSpace(ex.AdapterError.Message))
            {
                this.LastErrorMessages.Add(ex.AdapterError.Message);
            }
        }
    }
}
