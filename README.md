# Quiz-Roboterarm-Demo – Unity & Meta Quest 3

Mixed-Reality-Demo für die Meta Quest 3 (HAW Hamburg):

1. **Challenge 1 – Krise im Kongo:** Würfel greifen und in die passenden Zielboxen setzen
2. **Challenge 2 – Roboterarm:** einen UR5e-Roboterarm von Hand zum Ziel führen und mit der
   inversen Kinematik des Roboters vergleichen

**Unity-Version:** 6000.4.6f1 (Unity 6) mit *Android Build Support*
Git wird nicht benötigt – alle Pakete sind enthalten oder kommen aus der Unity-Registry.

## Projekt auf die Brille bringen

1. Auf dieser Seite **Code → Download ZIP**, entpacken (kurzer Pfad ohne Leerzeichen)
2. Unity Hub → **Add → Add project from disk** → Ordner wählen → öffnen
   (der erste Import dauert einige Minuten)
3. Szene `Assets/0Eigene/scenesRIS/asc20260422-ur-control-GirlsDay.unity` öffnen
4. Quest per USB-C anschließen, in der Brille USB-Debugging erlauben
5. **File → Build Profiles** → Android → Quest bei *Run Device* wählen → **Build And Run**

## Neuen Stand holen

Im Unity-Menü **Einführung → Aktualisieren** holt den aktuellen Stand dieses Repositorys –
ohne Git-Kenntnisse. Danach erneut **Build And Run**.

## Lizenzen

`Packages/com.whisper.unity`: MIT-Lizenz, © Macoron ([whisper.unity](https://github.com/Macoron/whisper.unity)).
Sprecherstimme der Kongo-Erklärung (`Assets/LogoPuzzle/Kongo/kongo_erklaerung.wav`): künstlich erzeugt mit
[Piper](https://github.com/OHF-Voice/piper1-gpl), Stimme „Thorsten“ ([Thorsten-Voice](https://github.com/thorstenMueller/Thorsten-Voice), Datensatz CC0;
das Stimmmodell basiert auf einem nur nicht-kommerziell nutzbaren Basismodell – daher nur für nicht-kommerzielle Zwecke).
Meta XR SDK: Lizenz von Meta, wird über den Unity Package Manager geladen.
Weitere Pakete und Assets unterliegen den Lizenzen ihrer jeweiligen Anbieter.
