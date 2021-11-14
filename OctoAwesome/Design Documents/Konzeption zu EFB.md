# Konzeption zu OctoAwesome Chunkabhängige Entities / Functional Blocks

## Problemstellungen
- Wenn Chunks geladen werden, wie kommen wir an die benötigten Entities / Functional Blocks -_Nachfolgend EFB genannt_-?
- Lazy Loading vom Planet kann zu Deadlock führen
- Mein Abhängigkeitsbaum ist ein Kreis
- Nebenläufigkeiten im Cache Prozess die nicht vollständig synchronisiert sind

## Ist Zustand
__Entities:__ Werden anhand der PositionComponent im GlobalChunkCache -_nachfolgend GCC genannt_- ctor geladen. Im späteren Verlauf werden dann die Position Components durchsucht und anhand von X, Y, Z und Planeten gefiltert.

Planet auf der Position Component ist Lazy Loading.

__Functional Blocks:__ werden aktuell noch nicht geladen oder gespeichert.

### Weitergehende Informationen
- GCC lädt die Chunks vom zugehörigen ResourceManager, welcher dann den Persistence Manager (Disk oder Network) anfragt
- EFB haben einen Local Chunk Cache -_nachfolgend LCC genannt_-, welcher vom GCC die einzelnen Chunks lädt.
- GCC verwendet einen Background Thread bzw. Task der CleanUp aufgaben übernimmt und erzeugt so eine Nebenläufigkeit
- Chunk ruft ChunkColumn BlockChanged auf, welches das OnUpdate im GCC triggert
- GCC ist pro Planet (readonly)

## Lösungsansätze
Quasi so etwas wie "Database Context". Eine Relation welche Information zum nachladen genutzt wird und welches Objekt die Relationen anfordert.
Beispiel ein Chunk wird geladen und gibt eine Position raus. Anhand dieser Position müssten dann Entities und Functional Blocks geladen werden.

Dieses Laden übernimmt ein Cache, welcher zum Programmierzeitpunkt angibt, mit welchen Daten er umgehen kann zum laden. 

Welche Komponente Abhängigkeiten auf andere Komponenten hat wird in einer Datei gespeichert, welche menschlich lesbar sein sollte. Die Lesbarkeit wird darin begründet, dass es nachträglich durch einen Mod oder Menschen einfacher anpassbar wäre und die Fehleranalyse vereinfacht, wenn zum Beispiel nach dem hinzufügen eines Mods oder nach einer Code änderung eine Abhängigkeit fehlt. Vermutlich wird hierführ JSON genutzt. 

## Lessons Learned
- Dependency Tree ist kompliziert, löst 1 Problem bringt 2 neue (Wäre aber cool und Alghorithmen haben wir auch schon).
- Entities laden ihren Components automatisch, deshalb eigentlich keine Child <=> Parent Beziehung notwendig, aber für einen Dependency Tree schon. Schwierig das zu trennen
- Position Component ist was ganz besonderes, da viele Sachen anhand ihrer Position geladen/identifiert werden

## Neuer Lösungsansatz
- Caches und Cacheservice wird erstellt
- Ctor Ladelogikfrei! (Softreferences), nur Übergabeparameter!
- Mapping Position => Type
    | Lösung 1 |Lösung 2|
    |----------|--------|
    | Mapping von TypeId, PositionComponentId und TypeType in zusätzlichen Type | Mapping mit Position, TypeId, TypeType|
    | Position Component wird anfänglich geladen |  Nur Laden der Mappings und PositionComponents erst wenn diese von dem Abhängigkeitstypen geladen wird |

- Ggf. zusätzliche Methode zum aufrufen um Softreferences zu laden
- Planet in der Position auch bei Deserialize echtes Lazy Loading => Feld anlegen, Property mit get => _field ??= TryUpdatePlanet(planet) umsetzen
- Cache befüllt sich beim 1. Aufruf mit benötigten Infos
- Aufruf von PositionCache entweder auf GCC oder ChunkColumn oder ...
- EntityService, FunctionalBlockService, ... können an Events subscriben und daraufhin zum Beispiel aus Caches laden
