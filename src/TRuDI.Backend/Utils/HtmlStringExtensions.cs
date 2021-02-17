namespace TRuDI.Backend.Utils
{
   using System;
   using System.Collections.Generic;
   using System.Globalization;
   using System.Linq;
   using System.Security.Cryptography;
   using System.Security.Cryptography.X509Certificates;
   using System.Text;

   using Microsoft.AspNetCore.Html;

   using TRuDI.HanAdapter.Interface;
   using TRuDI.Models;
   using TRuDI.Models.BasicData;

   /// <summary>
   /// Extension methods used to convert enum values to readable text or HTML.
   /// </summary>
   public static class HtmlStringExtensions
   {
      /// <summary>
      /// Adds spaces after the specified number of characters.
      /// </summary>
      /// <param name="source">The source string.</param>
      /// <param name="length">The length.</param>
      /// <returns>The source string with inserted spaces.</returns>
      public static HtmlString AddSpace(this string source, int length)
      {
         if (string.IsNullOrWhiteSpace(source))
         {
            return new HtmlString("");
         }

         var sb = new StringBuilder();

         for (int i = 0; i < source.Length; i++)
         {
            sb.Append(source[i]);
            if ((i + 1) % length == 0)
            {
               sb.Append(" ");
            }
         }

         return new HtmlString(sb.ToString());
      }

      /// <summary>
      /// Converts OIDs to friendly names.
      /// </summary>
      /// <param name="oid">The OID.</param>
      /// <returns>The friendly name of the OID.</returns>
      public static string OidToFriendlyName(this string oid)
      {
         var o = new Oid(oid);
         return o.FriendlyName;
      }

      /// <summary>
      /// Converts the TAF id to a friendly name.
      /// </summary>
      /// <param name="id">The TAF identifier.</param>
      /// <returns>The converted text.</returns>
      public static string TafToFriendlyName(this TafId? id)
      {
         if (id == null)
         {
            return string.Empty;
         }

         return id.Value.TafToFriendlyName();
      }

      /// <summary>
      /// Converts the TAF id to a friendly name.
      /// </summary>
      /// <param name="id">The TAF identifier.</param>
      /// <returns>The converted text.</returns>
      public static string TafToFriendlyName(this TafId id)
      {
         switch (id)
         {
            case TafId.Taf1:
               return "TAF-1: Datensparsamer Tarif";

            case TafId.Taf2:
               return "TAF-2: Zeitvariabler Tarif";

            case TafId.Taf3:
               return "TAF-3: Lastvariabler Tarif";

            case TafId.Taf4:
               return "TAF-4: Verbrauchsvariabler Tarif";

            case TafId.Taf5:
               return "TAF-5: Ereignisvariabler Tarif";

            case TafId.Taf6:
               return "TAF-6: Ablesung von Messwerten im Bedarfsfall";

            case TafId.Taf7:
               return "TAF-7: Zählerstandgangmessung";

            case TafId.Taf8:
               return "TAF-8: Erfassung von Extremwerten";

            case TafId.Taf9:
               return "TAF-9: Abruf der IST-Einspeisung";

            case TafId.Taf10:
               return "TAF-10: Abruf der Netzzustandsdaten";

            case TafId.Taf11:
               return "TAF-11: Steuerung von unterbrechbaren Verbrauchseinrichtungen und Erzeugungsanlagen";

            case TafId.Taf12:
               return "TAF-12: Prepaid Tarif";

            case TafId.Taf13:
               return "TAF-13: Bereitstellung von Messwertsätzen zur Visualisierung für den Letztverbraucher über die WAN-Schnittstelle";

            case TafId.Taf14:
               return "TAF-14: Hochfrequente Messwertbereitstellung";

            default:
               return id.ToString().ToUpperInvariant();
         }
      }

      /// <summary>
      /// Determines whether this billing period is completed.
      /// </summary>
      /// <param name="billingPeriod">The billing period.</param>
      /// <returns>Returns "ja" or "nein".</returns>
      public static string IsCompletedText(this BillingPeriod billingPeriod)
      {
         return billingPeriod.IsCompleted() ? "ja" : "nein";
      }

      /// <summary>
      /// Converts the status of the interval reading to the PTB status text.
      /// </summary>
      /// <param name="reading">The interval reading.</param>
      /// <param name="count">The count used for the distinction between plural and singular.</param>
      /// <returns>The status text.</returns>
      public static string ToStatusString(this IntervalReading reading, int count = 1)
      {
         if (reading == null || (reading.StatusPTB == null && reading.StatusFNN == null))
         {
            return string.Empty;
         }

         if (reading.StatusFNN != null)
         {
            return GetFnnStatusString(reading.StatusFNN, count);
         }

         var status = reading.StatusPTB ?? reading.StatusFNN.MapToStatusPtb();
         return status.GetStatusString(count);
      }

      /// <summary>
      /// Gets the background color of the status from specified interval reading.
      /// </summary>
      /// <param name="reading">The interval reading.</param>
      /// <returns>The CSS class.</returns>
      public static string ToStatusBackground(this IntervalReading reading)
      {
         if (reading == null)
         {
            return string.Empty;
         }

         if (reading.Value == null)
         {
            return "bg-warning";
         }

         if (reading.StatusPTB == null && reading.StatusFNN == null)
         {
            return string.Empty;
         }

         var status = reading.StatusPTB ?? reading.StatusFNN.MapToStatusPtb();

         switch (status)
         {
            case StatusPTB.NoError:
               return string.Empty;

            case StatusPTB.Warning:
               return "bg-warning";

            case StatusPTB.TemporaryError:
               return "bg-warning";

            case StatusPTB.CriticalTemporaryError:
               return "bg-warning";

            case StatusPTB.FatalError:
               return "bg-danger";

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Gets the status string for the specified PTB status.
      /// </summary>
      /// <param name="status">The status.</param>
      /// <param name="count">The count used for the distinction between plural and singular.</param>
      /// <returns>The status text.</returns>
      public static string GetStatusString(this StatusPTB status, int count = 0)
      {
         switch (status)
         {
            case StatusPTB.NoError:
               return "keine Fehler";

            case StatusPTB.Warning:
               return count == 1 ? "Warnung" : "Warnungen";

            case StatusPTB.TemporaryError:
               return count == 1 ? "temporärer Fehler" : "temporäre Fehler";

            case StatusPTB.CriticalTemporaryError:
               return count == 1 ? "kritischer temporärer Fehler" : "kritische temporäre Fehler";

            case StatusPTB.FatalError:
               return count == 1 ? "fataler Fehler" : "fatale Fehler";

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public static string GetFnnStatusString(this StatusFNN status, int count = 0)
      {
         var statusItems = new List<string>();

         if (status.BzStatusWord.HasFlag(BzStatusWord.Fatal_Error))
         {
            statusItems.Add("Zähler: fataler Fehler");
         }

         if (status.SmgwStatusWord.HasFlag(SmgwStatusWord.Fatal_Error))
         {
            statusItems.Add("SMGw: fataler Fehler");
         }

         if (status.SmgwStatusWord.HasFlag(SmgwStatusWord.Systemtime_Invalid))
         {
            statusItems.Add("SMGw: ungültige Systemzeit");
         }

         if (status.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Temp_Error_signed_invalid))
         {
            statusItems.Add("SMGw: temporärer Fehler");
         }

         if (status.BzStatusWord.HasFlag(BzStatusWord.Manipulation_KD_PS))
         {
            statusItems.Add("Zähler: mechanische Beeinflussung");
         }

         if (status.BzStatusWord.HasFlag(BzStatusWord.Magnetically_Influenced))
         {
            statusItems.Add("Zähler: magnetsiche Beeinflussung");
         }

         if (status.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Temp_Error_is_invalid))
         {
            statusItems.Add("SMGw: kritischer temporärer Fehler");
         }

         if (status.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Warning))
         {
            statusItems.Add("SMGw: Warnung");
         }

         if (!statusItems.Any())
         {
            return "keine Fehler";
         }

         return string.Join(", ", statusItems);
      }

      /// <summary>
      /// Gets the CSS class for the status icon from the specified interval reading.
      /// </summary>
      /// <param name="reading">The interval reading.</param>
      /// <returns>The CSS class of the status icon.</returns>
      public static string ToStatusIcon(this IntervalReading reading)
      {
         if (reading == null || (reading.StatusPTB == null && reading.StatusFNN == null))
         {
            return string.Empty;
         }

         var status = reading.StatusPTB ?? reading.StatusFNN.MapToStatusPtb();
         return status.ToStatusIcon();
      }

      /// <summary>
      /// Gets the CSS class for the status icon from the specified PTB status.
      /// </summary>
      /// <param name="status">The PTB status.</param>
      /// <returns>The CSS class of the status icon. </returns>
      public static string ToStatusIcon(this StatusPTB status)
      {
         switch (status)
         {
            case StatusPTB.NoError:
               return "fa fa-check-circle-o";

            case StatusPTB.Warning:
               return "fa fa-check-circle";

            case StatusPTB.TemporaryError:
               return "fa fa-exclamation-circle";

            case StatusPTB.CriticalTemporaryError:
               return "fa fa-exclamation-triangle";

            case StatusPTB.FatalError:
               return "fa fa-times-circle";

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Generates an identification string for the specified original value list.
      /// </summary>
      /// <param name="ovl">The original value list.</param>
      /// <returns>The generated identification string.</returns>
      public static string GetOriginalValueListIdent(this OriginalValueList ovl)
      {
         return $"ovl_{ovl.Meter}_{ovl.Obis.ToHexString()}_{ovl.MeasurementPeriod.TotalSeconds}";
      }

      /// <summary>
      /// Converts the service category kind to a readable string.
      /// </summary>
      /// <param name="kind">The service category kind.</param>
      /// <returns>The text of the service category kind.</returns>
      public static string ToServiceCategoryString(this Kind? kind)
      {
         if (kind == null)
         {
            return string.Empty;
         }

         switch (kind.Value)
         {
            case Kind.Electricity:
               return "Strom";

            case Kind.Gas:
               return "Gas";

            case Kind.Water:
               return "Wasser";

            case Kind.Pressure:
               return "Druck";

            case Kind.Heat:
               return "Wärme";

            case Kind.Cold:
               return "Kälte";

            case Kind.Communication:
               return "Kommunikation";

            case Kind.Time:
               return "Zeit";

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Generates the description text for the specified historic value.
      /// </summary>
      /// <param name="value">The historic value.</param>
      /// <returns>The formatted string.</returns>
      public static string ToHistoricValueDescription(this HistoricConsumption value)
      {
         switch (value.UnitOfTime)
         {
            case TimeUnit.Day:
               return value.Begin.ToString("dddd, dd.MM.yyyy", CultureInfo.GetCultureInfo("DE"));

            case TimeUnit.Week:
               var cal = new GregorianCalendar();
               var week = cal.GetWeekOfYear(value.Begin, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
               return $"Woche {week}/{value.Begin.Year}, {value.Begin.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("DE"))} bis {value.End.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("DE"))}";

            case TimeUnit.Month:
               return value.Begin.ToString("MMMM yyyy");

            case TimeUnit.Year:
               return value.Begin.ToString("yyyy");

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Convert the string to a formatted device id.
      /// </summary>
      /// <param name="value">The string value to format.</param>
      /// <returns>The formatted string.</returns>
      public static string ToFormattedDeviceId(this string value)
      {
         try
         {
            var serverId = new ServerId(value);

            if (serverId.IsValid && string.IsNullOrWhiteSpace(serverId.FlagId))
            {
               return serverId.Number.ToString();
            }

            return serverId.ToString();
         }
         catch
         {
            return value;
         }
      }

      /// <summary>
      /// Gets the user readable medium from the contract (based on the meter server id).
      /// </summary>
      /// <param name="contract">The contract.</param>
      /// <returns>The medium.</returns>
      public static string GetMedium(this ContractInfo contract)
      {
         ObisMedium medium;
         if (contract.Medium.HasValue)
         {
            medium = contract.Medium.Value;
         }
         else
         {
            var meter = contract.Meters?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(meter))
            {
               return "unbekannt";
            }

            var serverId = new ServerId(meter);
            medium = serverId.Medium;
         }

         switch (medium)
         {
            case ObisMedium.Electricity:
               return "Strom";

            case ObisMedium.HeatCostAllocator:
               return "Heizkostenabrechnung";

            case ObisMedium.Cooling:
               return "Kälte";

            case ObisMedium.Heat:
               return "Wärme";

            case ObisMedium.Gas:
               return "Gas";

            case ObisMedium.WaterCold:
               return "Kaltwasser";

            case ObisMedium.WaterHot:
               return "Warmwasser";

            case ObisMedium.Communication:
            case ObisMedium.Abstract:
               return string.Empty;

            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Converts the log level to a readable string.
      /// </summary>
      /// <param name="level">The log level.</param>
      /// <returns>The converted string.</returns>
      public static string GetLogLevelString(this Level level)
      {
         switch (level)
         {
            case Level.INFO:
               return "Info";

            case Level.WARNING:
               return "Warnung";

            case Level.ERROR:
               return "Fehler";

            case Level.FATAL:
               return "Fataler Fehler";

            case Level.EXTENSION:
               return "Erweiterung";

            default:
               return level.ToString();
         }
      }

      /// <summary>
      /// Converts the log entry outcome to a text string.
      /// </summary>
      /// <param name="outcome">The log entry outcome.</param>
      /// <returns>The converted string.</returns>
      public static string GetOutcomeString(this Outcome? outcome)
      {
         if (outcome == null)
         {
            return string.Empty;
         }

         switch (outcome)
         {
            case Outcome.SUCCESS:
               return "Erfolgreich";

            case Outcome.FAILURE:
               return "Fehlgeschlagen";

            case Outcome.EXTENSION:
               return "Erweiterung";

            default:
               return outcome.ToString();
         }
      }

      /// <summary>
      /// Converts the measurement period time span to a formatted string.
      /// </summary>
      /// <param name="measurementPeriod">The measurement period.</param>
      /// <returns>The formatted string.</returns>
      public static string GetMeasurementPeriodString(this TimeSpan measurementPeriod)
      {
         switch (measurementPeriod.TotalSeconds)
         {
            case 0:
               return "unbekannt";

            case 900:
               return "15 Minuten";

            case 1800:
               return "30 Minuten";

            case 3600:
               return "1 Stunde";

            case 86400:
               return "1 Tag";

            case 86400 * 28:
            case 86400 * 29:
            case 86400 * 30:
            case 86400 * 31:
               return "1 Monat";

            case 86400 * 365:
            case 86400 * 366:
               return "1 Jahr";

            default:
               if ((int)measurementPeriod.TotalSeconds % 3600 == 0)
               {
                  return $"{measurementPeriod.TotalSeconds / 3600} Stunden";
               }

               if ((int)measurementPeriod.TotalSeconds % 60 == 0)
               {
                  return $"{measurementPeriod.TotalSeconds / 60} Minuten";
               }

               return $"{measurementPeriod.TotalSeconds} Sekunden";
         }
      }

      /// <summary>
      /// Formats the specified OBIS without the F component.
      /// </summary>
      /// <param name="obisId">The OBIS to format</param>
      /// <returns>The formatted OBIS id.</returns>
      public static string ToReadableString(this ObisId obisId)
      {
         return $"{obisId.A}-{obisId.B}:{obisId.C}.{obisId.D}.{obisId.E}";
      }

      /// <summary>
      /// Gets the certificate serial number as decimal and hex formated string.
      /// </summary>
      /// <param name="cert">The certificate.</param>
      /// <returns>Formatted string with serial numbers.</returns>
      public static string GetHexAndDecimalSerialNumber(this X509Certificate2 cert)
      {
         var serialNumberBytes = cert.GetSerialNumber();
         if(serialNumberBytes.Length < 8)
         {
            var bytes = new byte[16];
            Array.Copy(serialNumberBytes, bytes, serialNumberBytes.Length);
            serialNumberBytes = bytes;
         }

         var serialNumber = BitConverter.ToUInt64(serialNumberBytes, 0);
         return $"{serialNumber} (0x{serialNumber:x16})";
      }
   }
}
