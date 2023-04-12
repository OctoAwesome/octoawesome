Fragen:
1. Was macht eine Notification aus?
   1. Will man steuern können, ob Notifications gesendet werden (Beibehaltung des Network Channels)? 
   2. Einfach jede senden?
2. Darf man am PAH (PackageActionHub) auf eine Notification Registern?
   1. Dürfen mehrere Leute an einer Notification am PAH registrieren?
3. Sollte Notification den Target Channel beinhalten?
   1. Darf eine Notification nur in ein Target Channel gepusht werden?
4. Mehr über Updatehub machen und weniger über locale Relays?

Antworten:
1. Package Flag Notification
   1. Ja (Wie noch zu klären)
      1. Network Channel beibehalten fürs senden
      2. UpdateHub prüft auf SerID Existenz und sendet bei Ja vorhanden
      3. Man regelt das über die AddSource Methode im Update hub, das hießt der updatehub kann das intern regeln, das wäre ein Mixtur aus 1 und 2. New Parameter SendOverNetwork
   2. Nein
2. Ja
   1. Wäre toll, aber aufwand
3. Vlt. müsste nach SerId kommen und wäre damit Pflichtfeld. Als erweiterung vom UpdateHub
   1. Eher nicht, sondern nur mit multi Relays / PushNetworkOnly
4. Joa why not? Maxi sieht damit kein Problem

Aussagen:
1. Alle "Notifications" eigene Ser Id 
2. Serializer Neue Methode SerializeNetwork wegen SerId