# Arbeitsprotokoll 2026-08-11

Sitzung im Repo `D:\Projekte\Github\CSharpUndVB\DecimalMath`, Branch `master`.
Ergebnis: korrigierte Projektdokumentation, sechs Bugfixes, Umstellung auf .NET 10, neues
Testprojekt mit 30 Tests, Release 1.2.7 (ohne NuGet-Upload).

Diese Datei ist bewusst **nicht** committet und **nicht** gepusht. Sie liegt untracked im
Repository-Wurzelverzeichnis, ein späteres `git add -A` würde sie mit einsammeln.

Vorlage der Vorgehensweise: `D:\Projekte\Github\CSharpUndVB\Doing\Arbeitsprotokoll-2026-08-10.md`.
Vorlage für das Testprojekt: `D:\Projekte\Github\CSharpUndVB\AesCryptFromStreamExample`.

## Inhalt

1. [Ausgangslage](#1-ausgangslage)
2. [CLAUDE.md geprüft und neu geschrieben](#2-claudemd-geprüft-und-neu-geschrieben)
3. [Umstellung auf .NET 10 und NuGet-Updates](#3-umstellung-auf-net-10-und-nuget-updates)
4. [Bugfixes](#4-bugfixes)
5. [Testprojekt](#5-testprojekt)
6. [Solution aufgeräumt](#6-solution-aufgeräumt)
7. [Release 1.2.7](#7-release-127)
8. [Alle Prüfungen im Überblick](#8-alle-prüfungen-im-überblick)
9. [Commits und Tag](#9-commits-und-tag)
10. [Offene Punkte und Beobachtungen](#10-offene-punkte-und-beobachtungen)

## 1. Ausgangslage

Stand zu Beginn: `master` auf `eb61eb5 "Readme: Added new gitter badge."`, einzige Änderung im
Arbeitsverzeichnis eine untrackte `CLAUDE.md`. Letzter Tag `1.2.6`, Zielframeworks
`net8.0;net9.0`, 17 getrackte Dateien, ein Bibliotheksprojekt, kein Testprojekt, kein `.github`.

Anders als im Nachbar-Repo `512kbChecker` ist das hier eine Bibliothek, die als NuGet-Paket
`HaemmerElectronics.SeppPenner.DecimalMath` veröffentlicht wird: `GeneratePackageOnBuild`,
`src/Directory.Build.props`, `BuildAndPushPackage.bat` und `Updating.md` gehören dazu, ein
Installer existiert nicht.

## 2. CLAUDE.md geprüft und neu geschrieben

Auftrag war, die Vorgehensweise aus dem Protokoll vom Vortag auf dieses Repo anzuwenden. Erster
Schritt also die Prüfung der vorhandenen `CLAUDE.md`. Ergebnis: sie passte überhaupt nicht, sie war
eine Kopie aus `D:\Projekte\Github\CSharpUndVB\AesCryptFromStreamExample` und beschrieb die
Verschlüsselung von XML-Dateien mit SharpAESCrypt statt dieser Mathematikbibliothek.

Falsch waren unter anderem: Solution `src/AesCryptFromStreamExample.sln`, drei Projekte,
`OutputType WinExe`, der `Crypt`-Alias, die Ordner `Services` und `Datatypes`, die Aussage
"nicht als NuGet-Paket veröffentlicht", das einzelne Zielframework `net10.0` und die Behauptung, es
gäbe kein `Updating.md`. Nichts davon trifft hier zu.

Prüfschritte zur Erhebung des Ist-Zustands:

| Prüfung | Kommando | Ergebnis |
| --- | --- | --- |
| Getrackte Dateien | `git ls-files` | 17 Dateien, ein Projekt, keine Tests, kein `.github` |
| Projektdatei | Lesen von `DecimalMath.csproj` | `net8.0;net9.0`, `GeneratePackageOnBuild`, `TreatWarningsAsErrors`, `NoWarn NU1803` |
| Quellcode | Lesen von `DecimalMath.cs` | 633 Zeilen, eine statische Klasse, 18 Funktionen, 5 öffentliche Konstanten |
| Konventionen | Lesen von `src/.editorconfig` | CRLF, 4 Spaces, UTF-8, `this.`-Qualifizierung, file scoped namespaces |
| Doku-Pflicht | Lesen von `src/Directory.Build.props` | `GenerateDocumentationFile`, damit ist ein fehlender XML-Kommentar per CS1591 ein Fehler |
| Versionierung | `git tag`, `git describe` | Tags `1.0.0` bis `1.2.6`, leichtgewichtig, GitVersion aktiv |
| Familienkonvention | Lesen von `CSharpLanguageManager` und `AesCryptFromStreamExample` | Bibliotheken stehen auf `net8.0;net10.0`, Testprojekte auf `net10.0` mit MSTest |
| Buildbarkeit | `dotnet build src/DecimalMath.sln -c Release` | 0 Warnungen, 0 Fehler, Version `1.2.7-5` |

Danach neu geschrieben mit den Abschnitten "What this is", "Build", "Code conventions",
"Known quirks", "Releasing", "Git" und "Writing style", und im Verlauf der Sitzung in den jeweiligen
Commits mitgezogen. Dokumentierte Besonderheiten: die Klasse heißt wie ihr Namespace
(`DecimalMath.DecimalMath`), die Ergebnisse sind nur nahe an `System.Math` und nicht gleich,
`MaximumIterations` ist 100, `Epsilon` ist die Ganzzahl-Toleranz und kein Maschinenepsilon,
`Atan2(0, 0)` wirft, `Power` wirft den Basistyp `Exception`, Paket-Id und Assemblyname sind
verschieden, `PackageReleaseNotes` dupliziert den Changelog.

Die Regel gegen Amend-Commits wurde als eigener Abschnitt `## Git` übernommen, mit derselben
Begründung wie im Nachbar-Repo: die Versionen kommen per GitVersion aus Tags auf exakten Commits.

Vor den Codeänderungen wurde die `CLAUDE.md` separat committet (`5f65886`).

## 3. Umstellung auf .NET 10 und NuGet-Updates

Voraussetzungen geprüft: installierte SDKs `6.0.428`, `9.0.317`, `10.0.111`, `10.0.303`, Runtime
`Microsoft.NETCore.App 10.0.11` vorhanden.

Änderungen:

- `TargetFrameworks` von `net8.0;net9.0` auf `net8.0;net10.0`. Net 9.0 ist seit Mai 2026 aus dem
  Support, das Schwester-Repo `CSharpLanguageManager` hat es aus demselben Grund am Vortag entfernt.
- `GitVersion.MsBuild` von `6.0.5` auf `6.8.2`.
- `Microsoft.SourceLink.GitHub` von `8.0.0` auf `10.0.400`.
- `NuGetAudit` und `NuGetAuditMode=all` ergänzt, wie in den Schwester-Repos.
- `README.md`: "Available for" von Net 9.0 auf Net 10.0.

Die beiden Pakete waren laut `dotnet list src/DecimalMath.sln package --outdated` die einzigen mit
Updates.

Prüfung: `dotnet build src/DecimalMath.sln -c Release` mit 0 Warnungen und 0 Fehlern. Bei
`TreatWarningsAsErrors` und aktivem Audit ist das eine belastbare Aussage. Beide Zielframeworks
werden gebaut, das Paket entsteht weiterhin bei jedem Build.

## 4. Bugfixes

Die Bibliothek hat keinen Einstiegspunkt, deshalb wurde das Verhalten vor den Änderungen mit einem
Wegwerf-Konsolenprojekt im Scratchpad erhoben, das `DecimalMath.cs` direkt einkompiliert, jede
Funktion gegen `System.Math` vergleicht und jeden Aufruf mit einem Zeitlimit von drei Sekunden in
einem eigenen Thread ausführt, um Endlosschleifen sichtbar zu machen.

Befunde und Fixes:

| Befund vorher | Nachher |
| --- | --- |
| `Power(10, 20)` und `PowerN(10, 20)` werfen `OverflowException`, obwohl `1e20` in `decimal` passt | `100000000000000000000` |
| `Power(-2, 3)` liefert `-8.000000000000000000000000008`, `Power(-2, 2)` liefert `4.0000000000000000000000000029` | exakt `-8` und `4` |
| `Log(1)` liefert `1E-28` | exakt `0`, damit auch `Log10(1)` |
| `Abs(0.0m)` liefert eine negative Null, `decimal.IsNegative` meldet `True` | `False` |
| `IsInteger` wirft `OverflowException` für Werte jenseits des `long`-Bereichs | `decimal.Truncate`, ein Check wirft nicht mehr |
| `Tan` und `IsSignOfSinusPositive` übergeben `nameof(x)` als **Meldung** der `ArgumentException`, die Meldung lautet also "x" | Meldung plus Parametername |

Ursachen:

- **PowerN.** Die Schleife quadrierte die laufende Basis auch noch nach dem letzten gesetzten Bit
  des Exponenten. Bei `PowerN(10, 20)` stand am Ende `current` auf `1e16`, das nicht mehr benötigte
  Quadrat `1e32` sprengte den Wertebereich, obwohl das Ergebnis `1e20` hineinpasst. Quadriert wird
  jetzt nur noch, solange ein weiteres Bit übrig ist.
- **Power.** Eine negative Basis mit ganzzahligem Exponenten ging über
  `Exp(pow * Log(-value))`, obwohl `PowerN` das Vorzeichen selbst korrekt behandelt und exakt
  bleibt. Die Bedingung heißt jetzt nur noch `isPowerInteger`. Damit fallen die beiden
  nachgelagerten Zweige und der unerreichbare Fall `case Zero when pow == Zero` des
  `value`-Switch weg.
- **Log.** `EInverted * E` ist in `decimal` nicht exakt eins, deshalb landete `Log(1)` bei
  `1E-28`. Eins ist jetzt ein bekannter Wert und wird direkt beantwortet.
- **Abs.** Der Vergleich lief über `<=`, das Negieren der Null setzt das Vorzeichenbit.

Ebenfalls in diesem Commit: `src/GlobalUsings.cs` gelöscht, eine Visual-Studio-Vorlagenrestdatei mit
`public class Class1`, die außerhalb des Projektverzeichnisses lag und nie kompiliert wurde, der
Doc-Kommentar von `Epsilon` (sagte "Represents PI.") korrigiert und die Signatur von `Atan2` im
Readme berichtigt, sie nimmt zwei Argumente.

Prüfung nach den Fixes: derselbe Probelauf zeigt keine Abweichung über `1e-9` gegenüber
`System.Math` für alle geprüften Argumente, keinen Hänger, und die vier Befunde oben sind behoben.

Absichtlich **nicht** geändert, weil es das Verhalten der veröffentlichten API bricht:
`Atan2(0, 0)` wirft weiter (`System.Math` liefert dort `0`), `Power` wirft weiter den Basistyp
`Exception`, `Sqrt` wirft weiter `OverflowException` statt `ArgumentException`, und die Genauigkeit
der Reihen bleibt, wie sie ist.

## 5. Testprojekt

Neu: `src/DecimalMath.Tests`, MSTest auf `net10.0`, Paketsatz wie im Vorlage-Repo
(`Microsoft.NET.Test.Sdk 18.8.1`, `MSTest.TestAdapter 4.3.3`, `MSTest.TestFramework 4.3.3`,
`coverlet.collector 10.0.1`, `GitVersion.MsBuild 6.8.2`), `IsPackable false`, `TreatWarningsAsErrors`
wie überall.

30 Tests in `DecimalMathTests.cs`, eine Testklasse für die eine Klasse unter Test. Vergleiche gegen
`System.Math` laufen über einen Helfer mit Toleranz, exakt verglichen wird nur, was die
Implementierung durch reine Multiplikation erreicht (`Cos(Pi) == -1`, `Power(-2, 3) == -8`,
`PowerN(2, 95)`). Jede Ausnahme der Bibliothek hat einen eigenen Test, inklusive der Fälle, die
absichtlich von `System.Math` abweichen.

Zwei Stolpersteine unterwegs:

- Die Klasse heißt wie ihr Namespace, im Namespace `DecimalMath.Tests` bindet der Name
  `DecimalMath` an den Namespace. Gelöst mit dem Alias
  `global using DecMath = DecimalMath.DecimalMath;` in der `GlobalUsings.cs` des Testprojekts,
  analog zum `Crypt`-Alias im Vorlage-Repo.
- `MSTEST0032` ist hier ein Fehler und verbietet `Assert.AreEqual` von zwei
  Compilezeit-Konstanten. Die Konstanten der Bibliothek werden deshalb über den Toleranz-Helfer
  geprüft. MSTest 4 kennt außerdem kein `Assert.ThrowsException` mehr, die Tests nutzen
  `Assert.ThrowsExactly<T>` mit Block-Lambda.

Prüfung der Aussagekraft: dieselben 30 Tests wurden gegen den Code **vor** den Fixes ausgeführt
(Kopie im Scratchpad, `DecimalMath.cs` aus `30d577f`). Ergebnis 4 Fehler, 26 grün, und zwar genau
`AbsReturnsAZeroWithoutTheSignBit`, `LogOfOneIsExactlyZero`,
`PowerReturnsExactResultsForIntegerPowers` und
`PowerNReturnsRepresentableResultsWithoutAnOverflow`. Gegen den gefixten Code sind alle 30 grün.

## 6. Solution aufgeräumt

`dotnet sln add` hat das Testprojekt eingetragen und dabei zusätzlich Konfigurationen für `x64` und
`x86` für beide Projekte geschrieben. Die Solution baut nur `Any CPU`, und die Solutions der
Schwester-Repos enthalten ausschließlich `Any CPU`. Die 20 Zeilen wurden in einem Folge-Commit
entfernt, geprüft per `git diff` (nur `x64`- und `x86`-Zeilen betroffen), danach Build und Tests
erneut grün.

## 7. Release 1.2.7

1. Changelog-Eintrag ergänzt:
   `* **Version 1.2.7.0 (2026-08-11)** : Removed support for Net9.0 as it is out of support, added support for Net10.0, updated NuGet packages, added a test project, fixed the unnecessary overflow of PowerN, fixed the precision of Power for negative bases with integer powers, Log returns exactly zero for one now, Abs no longer returns a negative zero.`
2. Denselben Text in `PackageReleaseNotes` der `.csproj` übernommen, im dort üblichen Einzeilerformat.
3. Commit `b4297fa`.
4. Tag-Art geprüft: `git cat-file -t` liefert für `1.2.4`, `1.2.5`, `1.2.6` jeweils `commit`, also
   leichtgewichtige Tags. `1.2.7` entsprechend leichtgewichtig auf `b4297fa` gesetzt.
5. `git push origin master` und `git push origin 1.2.7`.

Der NuGet-Upload ist auf Wunsch **nicht** gelaufen, siehe offene Punkte.

Prüfungen:

| Prüfung | Ergebnis |
| --- | --- |
| Version aus dem Tag | `SemVer 1.2.7`, `AssemblySemVer 1.2.7.0`, kein Prerelease-Suffix |
| Paketinhalt | `lib/net8.0` und `lib/net10.0` je mit `.dll` und `.xml`, dazu `Icon.png`, `License.txt`, `README.md`, `Changelog.md`, keine Test-Assembly |
| Release Notes im nuspec | Text der Version 1.2.7.0 |
| Remote | `origin/master` und Tag `1.2.7` auf `b4297fa` |
| Arbeitsverzeichnis | sauber |

## 8. Alle Prüfungen im Überblick

| # | Prüfung | Ergebnis |
| --- | --- | --- |
| 1 | Build vor allen Änderungen, `net8.0;net9.0` | 0 Warnungen, 0 Fehler |
| 2 | `dotnet list package --outdated` | zwei Pakete, beide aktualisiert |
| 3 | Installierte SDKs und Runtimes | .NET 10 vorhanden |
| 4 | Build auf `net8.0;net10.0` | 0 Warnungen, 0 Fehler |
| 5 | Verhaltensprobe vor den Fixes | vier Befunde, siehe Abschnitt 4 |
| 6 | Termination von `Sqrt` ohne Epsilon | 14 Argumente bis `decimal.MaxValue`, kein Hänger |
| 7 | Verhaltensprobe nach den Fixes | keine Abweichung über `1e-9` gegen `System.Math`, Befunde behoben |
| 8 | Build nach den Fixes | 0 Warnungen, 0 Fehler |
| 9 | `dotnet test` | 30 Tests grün |
| 10 | Dieselben Tests gegen den Code von 1.2.6.0 | 4 Fehler, 26 grün, genau die Regressionstests |
| 11 | Zeilenenden und BOM nach jeder Änderung | `.cs`, `.csproj`, `Changelog.md`, `README.md` durchgehend CRLF ohne BOM, `CLAUDE.md` LF wie in den Schwester-Repos, `.sln` unverändert BOM plus CRLF |
| 12 | Umlaut im Dateikopf des Testfiles | `Hämmer` als UTF-8, nicht umschrieben |
| 13 | Solution-Diff nach dem Aufräumen | ausschließlich `x64`- und `x86`-Zeilen entfernt |
| 14 | Version der Pakete nach dem Tag | `1.2.7`, ohne Prerelease-Suffix |
| 15 | Inhalt des `.nupkg` | beide Zielframeworks, XML-Doku, vier Wurzeldateien, keine Test-DLL |
| 16 | Remote-Zustand nach dem Push | Commits und Tag angekommen |

## 9. Commits und Tag

```
b4297fa  Update the changelog and the release notes for version 1.2.7.0   <- Tag 1.2.7
344f7e5  Remove the x64 and x86 platforms from the solution
6027fe9  Add a test project for the library
536d5b8  Fix the overflow in PowerN, the precision of Power and the negative zero
30d577f  Move to Net 10.0 and update the NuGet packages
5f65886  Add CLAUDE.md with project instructions
eb61eb5  Readme: Added new gitter badge.                                  <- Stand vor der Sitzung
```

Geänderte Dateien über die ganze Sitzung: `CLAUDE.md` (neu), `Changelog.md`, `README.md`,
`src/DecimalMath.sln`, `src/DecimalMath/DecimalMath.csproj`, `src/DecimalMath/DecimalMath.cs`,
`src/GlobalUsings.cs` (gelöscht), `src/DecimalMath.Tests/DecimalMath.Tests.csproj` (neu),
`src/DecimalMath.Tests/DecimalMathTests.cs` (neu), `src/DecimalMath.Tests/GlobalUsings.cs` (neu).

## 10. Offene Punkte und Beobachtungen

- **Der NuGet-Upload fehlt noch.** Version 1.2.7 ist getaggt und gepusht, aber nicht
  veröffentlicht. Dafür `BuildAndPushPackage.bat` ausführen, es braucht `NUGET_API_KEY` und
  `GITHUB_API_KEY` in der Umgebung und lädt `.nupkg` und `.snupkg` zu nuget.org und zu GitHub
  Packages. Der Tag existiert bereits, die veröffentlichte Version wäre also sauber `1.2.7`.
- **`Updating.md` ist unvollständig.** Die fünf Schritte nennen weder den Changelog noch die Tests
  und sagen "Build the project with Visual Studio", während der Push über die Batchdatei läuft. Die
  vollständige Reihenfolge steht jetzt in der `CLAUDE.md`, die beiden Dateien widersprechen sich
  damit halb.
- **Genauigkeit von `Log` weit weg von eins.** `MaximumIterations` ist 100, deshalb liefert
  `Log10(1e10)` den Wert `10.000000000000000000000018064`. Das ist der schlechteste beobachtete
  Wert und liegt noch weit innerhalb der `double`-Genauigkeit, aber es ist kein Fehler der Reihe,
  sondern die Iterationsgrenze. Eine höhere Grenze würde Rechenzeit gegen Ziffern tauschen.
- **`Sqrt` ohne Epsilon.** Der Default `Zero` verlangt einen exakten Fixpunkt der
  Newton-Iteration. Für alle geprüften Werte terminiert das sofort, ein Zwei-Zyklus ist theoretisch
  aber möglich. Wer das nicht riskieren will, übergibt ein Epsilon.
- **`Power` castet den Exponenten auf `int`.** Ein Exponent über `int.MaxValue` wirft eine
  `OverflowException`, bevor gerechnet wird. Für Basen außerhalb von `-1` bis `1` würde das
  Ergebnis den `decimal`-Bereich ohnehin verlassen, innerhalb liefe es gegen null.
- **Der Readme listet die Konstanten nicht.** `E`, `Epsilon`, `One`, `Pi` und `Zero` sind
  öffentlich, tauchen in der Methodenliste aber nicht auf.
- **AppVeyor-Badge ohne CI im Repo.** Der Build ist außerhalb dieses Repos konfiguriert, es gibt
  hier weder `.github` noch eine Pipelinedatei. Die 30 Tests laufen damit nur lokal.
- **Die Hilfsprojekte der Sitzung** (Verhaltensprobe und der Testlauf gegen den alten Code) liegen
  im Scratchpad unter
  `C:\Users\Nutzer\AppData\Local\Temp\claude\D--Projekte-Github-CSharpUndVB-DecimalMath`, nicht im
  Repo.
