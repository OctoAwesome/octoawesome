# Network / Multiplayer

## Protokoll

### Ablaufplan

__Handshake__

1. Client sicht:
    1. Auf Multiplayer klicken
    2. Eventuell Optional nach Nickname abfragen -> Settings evtl. generell? Vlt. bei Spielstart direkt abfragen
    3. Hostname des Zielservers eingeben

    ---

    4. Verbindungsaufbau mit TCP -> das steht bereits quasi
    5. "LogIn-Command" beinhaltet: Nickname, Client-Version, Avatar, Modliste
    6. Warte auf Bestätigung
    7. Bei Fehler fehlerhandling bei Bestätigung: 
        * Daten des Servers prüfen
        * Wenn fehler dann melden

2. Server sicht:
    1. Client verbindet sich
    2. Wartet auf Login-Command
    3. Prüft Daten aus Command
    4. Sende Bestätigung (Serverversion) oder Fehler

__Gameprotokoll__

1. Client sicht:
    1. WHOAMI: Wer bin ich? was bin ich? wo bin ich? warum bin ich? und wenn ja wie viele?
    2. Chunkabonnement.

    3. Wenn chunk nicht mehr benötigt wird dann Chunkdeabonnement.
    4. Empfängt update paket => aktuallisieren der chunks
    5. Sende update paket

2. Server sicht:
    1. Bekommt ein WHOAMI: nachgucken der Daten und dann initial antworten
        * Position
        * Weltinformation (Planetmetadaten)
    2. Chunkabonnement: zurücksenden der Chunkdaten und client zur "aboliste" des chunks hinzufügen
    3. Chunkdeabonnement: client aus "aboliste" entfernen.
    4. Updatepaket bei änderung eines Chunks
    5. Empfängt update paket:
        * Prüft update paket auf plausibilität
        * Wenn nicht plausibel verwerfen und Update packet senden (des aktuellen chunks)

## Nächste Schritte

1. Neuer Button für die UI
2. Zielserver eintippmöglichkeit
3. WHOAMI implementieren
4. Client: eine Simulation starten => "keine eigene Map generiert"
5. Client: NetworkPersistentManager implementieren der mit Server kommuniziert