## Was brauchen wir?
1. Update Methode
2. Textur
3. Inventar im Block
4. Input, Output Felder

## Fragen:
* Wann muss der Ofen geladen werden? (PositionComponent?)
* Wann entladen? (Postponed)
* Hat der Ofen einen Prozess State der serialisiert werden muss?
* Wie machen wir das mit den brennbaren Materialien? (Postponed bis in Json/YAML etc ausgelagert)
    - IBurnableMaterialDefinition
        - UpperHeatingValue (kJ/kg, MJ/m3, kWh/kg, kWh/m3) [Heizwert](https://www.energie-lexikon.info/heizwert.html)
        - BurnTemperature
        - BurnTime => Siehe UpperHeatingValue und BurnTemperature
        - Burnability (Easy, Medium, Hard, None)
* Temperaturgeregelte Outputmaterialien oder gleicher Input führt zu gleichen Output
    - Temperaturgeregelt klingt lustiger
* Brennmaterial mit Output?
    - Holz => Asche
    - Alkohol => Nüschts
    - Kohle => Asche
    - Benzin => Ruß
* Getrennte Öfen für Essen und Materialschmelzung?
    - Ofen für Essen aufgrund von Bauweise weniger Heiß
        - Teig in diesen Ofen => Brot
        - ~50 - ~477°C
    - Schmelzofen für Materialschmelzung heißer
        - Teig in diesen Ofen => Kohle
* Hilfsmittel?
    - Pfanne
    - Tiegel
    - etc.pp.
    - Input => Output (Muss Rezept, klingt eher wie Workaround)
    - Neuer Slot für sowas (Block geregelt)
* Wie definieren wir die Input Output Verhältnisse?
    - Benötigte Temperatur
    - Ofentyp?
    - Input, output
    - Dauer (kJ/kg, Sekunden, ?)
    - Welches Format? (YAML, JSON, XML, HTML, MD, XAML, PostScript, RTF, Binär, LaTeX, TeX, SGML)


TODOS:
1. Componentsystem erweitern um UI Components 
    - Teil von ScreenComponent
    - Entity und Functionalblock Components sind Dataholder
    - Updates gehen (nur) über SimulationComponents
    - UIComponents sind quasi SimulationComponents für die UI (Bsp: ITransferScreenComponent für Entity sorgt für Anzeige einer UI Component)
    - Ui Component bekommt die Logik (Bsp: Item Transfer von Inventory A zu Inventory B)
    - Screens sollte UI only sein
    - Screens können der Component eine Änderung mitteilen
    - UI Component sollte Update bekommen um kein Missverständnis zu verursachen
    - Quasi Screen View und UI Component View Model (Wo das Model ist müssen wir noch schauen (nicht ernst gemeint))
2. Componentsystem dokumentieren
    - IEntity/FunctionalBlockComponent => DataHolder
    - SimulationComponent => Logik / Update
    - UI Component => UI 
3. Ofen braucht:
    - OvenSimulationComponent
    - OvenUIComponent
      - OvenUI
    - Ofen Model
    - Ofen Item
4. Rezepte
~~ 5. Datenbank ~~


Könnten wir tun:
* Temperaturgeregelte Outputmaterialien oder gleicher Input führt zu gleichen Output (Dictionary<string, string>, Dictionary<string, object>?)
    - Temperaturgeregelt klingt lustiger
* Brennmaterial mit Output?
    - Holz => Asche
    - Alkohol => Nüschts
    - Kohle => Asche
    - Benzin => Ruß
* Furnace aufräumen