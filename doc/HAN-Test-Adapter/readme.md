# HanAdapter Example: Erstellen einer JSON Konfigurationsdatei

Durch die Konfiguration können über den TRuDI.HanAdapter.Example die Verbindung zum SMGW und
zurückgelieferte Daten simuliert werden. Nach jedem Block ist eine Beispielkonfiguration angefügt.
Für die Erstellung einer Konfiguration liegt in diesem Ordner ein Template bereit und je eine 
Konfigurationsdatei für Taf1, Taf2 und Taf7.

## Daten für den Verbindungsaufbau mit dem SMGW

### DeviceId
Die ID des SMGW. Der Wert muss mit der SmgwId der Xml Konfigurationsdaten übereinstimmen. 

### User, Password
Die Anmeldedaten, die benötigt werden um sich am SMGW anzumelden.

### IPAddress, IPPort
Die IP-Adresse und der Port des SMGW mit dem eine Verbindung aufgebaut werden soll.

### TimeToConnect
Der hier angegebene Wert simmuliert die Zeit, die benötigt wird, um eine Verbindung zum SMGW aufzubauen.

### Cert
Das Zertifikat, das vom SMGW zurückgeliefert wird.

### Version 
Die Firmwareversion des SMGW. Folgende Parameter können eingestellt werden: Component, Version, Hash.

#### Beispiel für eine Verbindungsaufbaukonfiguration

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

## Konfigurierbare Daten der im SMGW enthaltenen Verträge

### Contracts
Hier werden alle Verträge konfiguriert, die im SMGW enthalten sein sollen und durch den HanAdapter ausgelesen werden können.

### TafId
Hier wird die TafId eingetragen, die dem Vertrag zugrund liegt ('1' für Taf1 , '2' für Taf2, usw.).

### TafName
Die Bezeichnung des Tarifs. Diese Bezeichnung tauch auc in den Xml Konfigurationsdaten nochmals auf. 

### Description
Die Beschreibung des Vertrages. 

### Meters
Hier werden alle Meter (die MeterIds) aufgelistet, die mit dem Vertrag in Verbindung stehen.

### MeteringPointId
In diesem Element kann die Zählpunktbezeichnung eingetragen werden. Sie entspricht dem Feld UsagePointId der Xml Konfiguration.

### SupplierId
Die Id des Stromlieferanten. In der Xml Konfiguration ist diese Id unter InvoicingPartyId wiederzufinden.

### ConsumerId
Die Id des Stromverbrauchers. Auch dieser Wert ist in den Xml Konfigurationen zu finden (CustomerId).

### Begin, End
Start und Ende der Vertragslaufzeit. Die Angabe der Parameter erfolgt nach folgendem Schema: YYYY-MM-DDThh:mm:ss Beispiel: 2017-07-01T12:00:00

### BillingPeriods (Begin, End)
Abrechnungsperioden des Vertrags. Die Angabe der Parameter erfolgt nach folgendem Schema: YYYY-MM-DDThh:mm:ss Beispiel: 2017-07-01T12:00:00

#### Beispiel für einen Vertrag 

```json
	"Contracts":[
		{
			"TafId": "7",
			"TafName": "Tarif-Taf7",
			"Description": "Zählerstandsgangmessung",
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
Konfiguration der Xml Daten. Die einzelnen Parameter werden näher erläutert. 

### UsagePointId
Die UsagePointId ist einmalig und entspricht der MeteringPointId (siehe Verträge).

### TariffName
Entspricht dem Parameter TafName in den Verträgen. Über diesen Parameter wird der passende Vertrag zu den SMGW Daten ermittelt.

### TariffId
Entspricht der Bezeichnung des Anwendungsfalls (z.B. Taf2, Taf7 etc.).

### CustomerId
Entspricht dem Parameter ConsumerId in den Verträgen.

### InvoicingPartyId
Entspricht dem Parameter SupplierId in den Verträgen.

### SmgwId
Entspricht dem Parameter MeteringPointId in den Verträgen.

### ServiceCategoryKind
Mit diesem Paramter wird die Art der Service Kategorie festgelegt. (0 z.B. entspricht der Elektrizität)

### LogCount 
Legt die Azahl der simmulierten LogEinträge fest. 

### RandomLogCount
Ist der Parametert auf true gesetzt, wird der Parameter LogCount ignoriert und eine zufällige Anzahl von LogEinträgen generiert.

### PossibleLogMessages
In dieser Liste können Meldungen für die Logeinträge angegeben werden. Sie werden dann per Zufall für die einzelnen Logeinträge ausgewählt.

### Certificates, Certificate
Eine Liste möglicher Zertifikate, die vom SMGW mitgeliefert werden. 

### CertId, CertType, ParentCertId
Die Id und die Art des Zertifikats. In ParentCertId könnte die Id des nächtshöheren SubCA in der Zertifikatskette angegeben werden.

### CertContent
Der Inhalt des Zertifikats als hex String. 

### TariffUseCase (Nur bei Taf7)
Legt fest welcher Auswerteprofil gültig ist. (z.B. '2' für Taf2).

### TariffStageCount 
Legt die Anzahl der Tarifstufen fest (z.B. 2 für NT und HT).

### DefaultTariffNumber (Nur bei Taf7)
Legt die Standardtarifstufe fest. Wurde zum Beispiel bei *TariffStageCount* 2 angegeben für NT und HT würde hier 1 eingetragen werden.

### DayIdCount (Nur bei Taf7)
Legt die Anzhal der unterschiedlichen Tagesprofile fest.

### MaxPeriodUsage
Der maximal mögliche Verbrauch zwischen zwei Messwerten. Er tatsächliche eingetragene Wert wird dann per Zufall berechnet.

### MeterReadingConfigs
Parameter für die generierung eines MeterReading Objekts.

### MeterId
Die Zählernummer des Smartmeters, die auch im Vertrag in der Meters Liste notiert ist.

### MeterReadingId
Der eindeutige Bezeichner für das MeterReading Objekt.

### IsOML
Ist IsOML auf true gesetzt, wird das MeterReading Objekt als originäre Messwertliste behandelt.

### OMLInitValue
Der initiale Wert des Zählers zum Startzeitpunkt des angegebenen Zeitbereichs.

### PeriodSeconds
Die Periodendauer der Abbrechnungen in Sekunden. Der Wert darf nicht kleiner als 900 oder ein vielfaches davon sein. (900 Sekunden entspricht 15 Minuten)

### PowerOfTenMultiplier
Repräsentiert den Einheitenvorsatz der Maßeinheit der in der Messwertliste übermittelten Werte. Zum Beispiel 0 für None, 1 für deca, 3 für kilo.

### Uom
Enspricht der Kodierung der Masseinheit, die den Messwerten zugrunde liegt. 72 zum Beispiel bedeutet Wh.

### Scaler
Anhand dieses Parameters lässt sich der Skalierungsfaktor der ganzzahligen Messwerte bestimmten. 

### ObisCode
Der entsprechende ObisCode

### UsedStatus 
Hier wird der gewünschte Status angegeben. "FNN" für StatusFNN und "PTB" für StatusPTB. Andere Werte sind nicht gültig.

### TariffStageConfigs (Nur bei Taf7)
Jede Tarifstufe muss hier konfiguriert werden. 

### TariffNumber (Nur bei Taf7)
Die Nummer des Tarifs.

### Description (Nur bei Taf7)
Eine Beschreibung des Tarifs.

### ObisCode (Nur bei Taf7)
Der ObisCode des Tarifs.

### DayProfiles (Nur bei Taf7)
Hier können Tagesprofile angelegt und die Umschaltzeiten unter DayTimeProfiles eingetragen werden. 

### DayId (Nur bei Taf7)
Jedem Tagesprofil wird eine eindeutige Identifikationsnummer zugeordnet.

### DayTimeProfiles (Nur bei Taf7)
Konfiguration der einzelnen Tagesprofile

### Start, End (Nur bei Taf7)
Der Gültigkeitszeitraum der Tagesprofile. In der Xml Datei sind diese Zeiten als viertelstündlicher Kalender ausgerollt.

### TariffNumber (Nur bei Taf7)
Die Nummer des zu diesen DayTimeProfile gehörigem Tarifs.


#### Beispiel für ein vollständiges XmlConfig 

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
