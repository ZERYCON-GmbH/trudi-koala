# HanAdapter Example: Erstellen einer JSON Konfigurationsdatei

Durch die Konfiguration k�nnen �ber den TRuDI.HanAdapter.Example die Verbindung zum SMGW und
zur�ckgelieferte Daten simuliert werden. Nach jedem Block ist eine Beispielkonfiguration angef�gt.
F�r die Erstellung einer Konfiguration liegt in diesem Ordner ein Template bereit und je eine 
Konfigurationsdatei f�r Taf1, Taf2 und Taf7.

## Daten f�r den Verbindungsaufbau mit dem SMGW

### DeviceId
Die ID des SMGW. Der Wert muss mit der SmgwId der Xml Konfigurationsdaten �bereinstimmen. 

### User, Password
Die Anmeldedaten, die ben�tigt werden um sich am SMGW anzumelden.

### IPAddress, IPPort
Die IP-Adresse und der Port des SMGW mit dem eine Verbindung aufgebaut werden soll.

### TimeToConnect
Der hier angegebene Wert simmuliert die Zeit, die ben�tigt wird, um eine Verbindung zum SMGW aufzubauen.

### Cert
Das Zertifikat, das vom SMGW zur�ckgeliefert wird.

### Version 
Die Firmwareversion des SMGW. Folgende Parameter k�nnen eingestellt werden: Component, Version, Hash.

#### Beispiel f�r eine Verbindungsaufbaukonfiguration

```json
	"DeviceId": "EXXX0012345678",
	"User": "User01",
	"Password": "123456",
	"IPAddress": "1.2.3.4",
	"IPPort": "443",
	"TimeToConnect": "00:00:05",
	"Cert": "a89d0392c32d32ff1123b2d2s212bc21a2d2ff23988e0c983" 
	"Version": 
	{
		"Component": "System",
		"Version": "0.0.1",
		"Hash": "c82cd3d3232d321a1123cd2214212bc21a2df3111a39b88e0c983"
	}
```

## Konfigurierbare Daten der im SMGW enthaltenen Vertr�ge

### Contracts
Hier werden alle Vertr�ge konfiguriert, die im SMGW enthalten sein sollen und durch den HanAdapter ausgelesen werden k�nnen.

### TafId
Hier wird die TafId eingetragen, die dem Vertrag zugrund liegt ('1' f�r Taf1 , '2' f�r Taf2, usw.).

### TafName
Die Bezeichnung des Tarifs. Diese Bezeichnung tauch auc in den Xml Konfigurationsdaten nochmals auf. 

### Description
Die Beschreibung des Vertrages. 

### Meters
Hier werden alle Meter (die MeterIds) aufgelistet, die mit dem Vertrag in Verbindung stehen.

### MeteringPointId
In diesem Element kann die Z�hlpunktbezeichnung eingetragen werden. Sie entspricht dem Feld UsagePointId der Xml Konfiguration.

### SupplierId
Die Id des Stromlieferanten. In der Xml Konfiguration ist diese Id unter InvoicingPartyId wiederzufinden.

### ConsumerId
Die Id des Stromverbrauchers. Auch dieser Wert ist in den Xml Konfigurationen zu finden (CustomerId).

### Begin, End
Start und Ende der Vertragslaufzeit. Die Angabe der Parameter erfolgt nach folgendem Schema: YYYY-MM-DDThh:mm:ss Beispiel: 2017-07-01T12:00:00

### BillingPeriods (Begin, End)
Abrechnungsperioden des Vertrags. Die Angabe der Parameter erfolgt nach folgendem Schema: YYYY-MM-DDThh:mm:ss Beispiel: 2017-07-01T12:00:00

#### Beispiel f�r einen Vertrag 

```json
	"Contracts":[
		{
			"TafId": "7",
			"TafName": "Tarif-Taf7",
			"Description": "Z�hlerstandsgangmessung",
			"Meters": [
				"AD34234DC39230F342E21"
			],
			"MeteringPointId": "DE00056287342AB324HL04323ET3214OP",
			"SupplierId": "IVU Imag Strom",
			"ConsumerId": "userId-01",
			"Begin": "2017-01-01T00:00:00",
			"End": "2018-01-01T00:00:00",
			"BillingPeriods":[
				{
					"Begin": "2017-01-01T00:00:00",
					"End": "2017-08-01T00:00:00"
				}
			]
		}	
	]
```

## Daten zur Konfiguration der generierbaren Xml Datei

### WithLogData
Dieser Wert (true oder false) legt fest, ob Logdaten generiert werden sollen. 

### XmlConfig
Konfiguration der Xml Daten. Die einzelnen Parameter werden n�her erl�utert. 

### UsagePointId
Die UsagePointId ist einmalig und entspricht der MeteringPointId (siehe Vertr�ge).

### TariffName
Entspricht dem Parameter TafName in den Vertr�gen. �ber diesen Parameter wird der passende Vertrag zu den SMGW Daten ermittelt.

### TariffId
Entspricht der Bezeichnung des Anwendungsfalls (z.B. Taf2, Taf7 etc.).

### CustomerId
Entspricht dem Parameter ConsumerId in den Vertr�gen.

### InvoicingPartyId
Entspricht dem Parameter SupplierId in den Vertr�gen.

### SmgwId
Entspricht dem Parameter MeteringPointId in den Vertr�gen.

### ServiceCategoryKind
Mit diesem Paramter wird die Art der Service Kategorie festgelegt. (0 z.B. entspricht der Elektrizit�t)

### LogCount 
Legt die Azahl der simmulierten LogEintr�ge fest. 

### RandomLogCount
Ist der Parametert auf true gesetzt, wird der Parameter LogCount ignoriert und eine zuf�llige Anzahl von LogEintr�gen generiert.

### PossibleLogMessages
In dieser Liste k�nnen Meldungen f�r die Logeintr�ge angegeben werden. Sie werden dann per Zufall f�r die einzelnen Logeintr�ge ausgew�hlt.

### Certificates, Certificate
Eine Liste m�glicher Zertifikate, die vom SMGW mitgeliefert werden. 

### CertId, CertType, ParentCertId
Die Id und die Art des Zertifikats. In ParentCertId k�nnte die Id des n�chtsh�heren SubCA in der Zertifikatskette angegeben werden.

### CertContent
Der Inhalt des Zertifikats als hex String. 

### TariffUseCase (Nur bei Taf7)
Legt fest welcher Auswerteprofil g�ltig ist. (z.B. '2' f�r Taf2).

### TariffStageCount 
Legt die Anzahl der Tarifstufen fest (z.B. 2 f�r NT und HT).

### DefaultTariffNumber (Nur bei Taf7)
Legt die Standardtarifstufe fest. Wurde zum Beispiel bei *TariffStageCount* 2 angegeben f�r NT und HT w�rde hier 1 eingetragen werden.

### DayIdCount (Nur bei Taf7)
Legt die Anzhal der unterschiedlichen Tagesprofile fest.

### MaxPeriodUsage
Der maximal m�gliche Verbrauch zwischen zwei Messwerten. Er tats�chliche eingetragene Wert wird dann per Zufall berechnet.

### MeterReadingConfigs
Parameter f�r die generierung eines MeterReading Objekts.

### MeterId
Die Z�hlernummer des Smartmeters, die auch im Vertrag in der Meters Liste notiert ist.

### MeterReadingId
Der eindeutige Bezeichner f�r das MeterReading Objekt.

### IsOML
Ist IsOML auf true gesetzt, wird das MeterReading Objekt als origin�re Messwertliste behandelt.

### OMLInitValue
Der initiale Wert des Z�hlers zum Startzeitpunkt des angegebenen Zeitbereichs.

### PeriodSeconds
Die Periodendauer der Abbrechnungen in Sekunden. Der Wert darf nicht kleiner als 900 oder ein vielfaches davon sein. (900 Sekunden entspricht 15 Minuten)

### PowerOfTenMultiplier
Repr�sentiert den Einheitenvorsatz der Ma�einheit der in der Messwertliste �bermittelten Werte. Zum Beispiel 0 f�r None, 1 f�r deca, 3 f�r kilo.

### Uom
Enspricht der Kodierung der Masseinheit, die den Messwerten zugrunde liegt. 72 zum Beispiel bedeutet Wh.

### Scaler
Anhand dieses Parameters l�sst sich der Skalierungsfaktor der ganzzahligen Messwerte bestimmten. 

### ObisCode
Der entsprechende ObisCode

### UsedStatus 
Hier wird der gew�nschte Status angegeben. "FNN" f�r StatusFNN und "PTB" f�r StatusPTB. Andere Werte sind nicht g�ltig.

### TariffStageConfigs (Nur bei Taf7)
Jede Tarifstufe muss hier konfiguriert werden. 

### TariffNumber (Nur bei Taf7)
Die Nummer des Tarifs.

### Description (Nur bei Taf7)
Eine Beschreibung des Tarifs.

### ObisCode (Nur bei Taf7)
Der ObisCode des Tarifs.

### DayProfiles (Nur bei Taf7)
Hier k�nnen Tagesprofile angelegt und die Umschaltzeiten unter DayTimeProfiles eingetragen werden. 

### DayId (Nur bei Taf7)
Jedem Tagesprofil wird eine eindeutige Identifikationsnummer zugeordnet.

### DayTimeProfiles (Nur bei Taf7)
Konfiguration der einzelnen Tagesprofile

### Start, End (Nur bei Taf7)
Der G�ltigkeitszeitraum der Tagesprofile. In der Xml Datei sind diese Zeiten als viertelst�ndlicher Kalender ausgerollt.

### TariffNumber (Nur bei Taf7)
Die Nummer des zu diesen DayTimeProfile geh�rigem Tarifs.


#### Beispiel f�r ein vollst�ndiges XmlConfig 

```json
	"WithLogData": "true",
	"XmlConfig": 
	{
		"UsagePointId": "DE00056287342AB324HL04323ET3214O",
		"TariffName": "Tarif-Taf7",
		"TariffId": "Taf7",
		"CustomerId": "userId-01",
		"InvoicingPartyId": "IVU Imag Strom",
		"SmgwId": "EXXX0012345678",
		"ServiceCategoryKind": "0",
		"RandomLogCount": "false",
		"LogCount": "20",
		"PossibleLogMessages": 
		[
			"Ein neuer Wert wurde abgelesen.",
			"Tarifumschaltung war erfolgreich."
        ],
		"Certificates": 
		[
			{
				"Certificate": 
				{
					"CertId": "1",
					"CertType": "1"
				},
				"CertContent": "a89d0392c32d32ff1123b2d2s212bc21a2d2ff23988e0c983"
			}
		],
		"TariffUseCase": "2",
		"TariffStageCount": "2",
		"DefaultTariffNumber": "1",
		"DayIdCount": "1",
		"MaxPeriodUsage": "300",
		"MeterReadingConfigs": 
		[
			{
				"MeterId": "AD34234DC39230F342E21",
				"MeterReadingId": "2341230413DA3234CF234",
				"IsOML": "true",
				"OMLInitValue": "100000",
				"PeriodSeconds": "900",
				"PowerOfTenMultiplier": "0",
				"Uom": "72",
				"Scaler": "-1",
				"ObisCode": "0100010800ff",
				"UsedStatus:" "FNN"
			}
		],
		"TariffStageConfigs": 
		[
			{
				"TariffNumber": "1",
				"Description": "NT",
				"ObisCode": "0100010801ff"
			},
			{
				"TariffNumber": "2",
				"Description": "HT",
				"ObisCode": "0100010802ff"
			}
		],
		"DayProfiles": 
		[
			{
				"DayId": "1",
				"DayTimeProfiles": 
				[
					{
						"Start": "0001-01-01T00:00:00",
						"End": "0001-01-01T05:45:00",
						"TariffNumber": "1"
					},
					{
						"Start": "0001-01-01T06:00:00",
						"End": "0001-01-01T19:45:00",
						"TariffNumber": "2"
					},
					{
						"Start": "0001-01-01T20:00:00",
						"End": "001-01-01T23:45:00",
						"TariffNumber": "1"
					}
				]
			}
		]
	}
```
