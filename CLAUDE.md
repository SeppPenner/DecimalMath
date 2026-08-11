# Project rules for Claude

## What this is

DecimalMath is a class library that reimplements the functions of `System.Math` for the `decimal`
data type, so that a calculation keeps 28 significant decimal digits instead of the 15 to 17 of
`double`. The repository is published as the NuGet package
[HaemmerElectronics.SeppPenner.DecimalMath](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/),
so every change ends in a package, not in an application.

One solution `src/DecimalMath.sln` with exactly one project
`src/DecimalMath/DecimalMath.csproj`, a library that multi targets `net8.0;net10.0`.

Layout inside `src/DecimalMath`:

- `DecimalMath.cs`: the whole library. One `public static class DecimalMath` holding the public
  constants `E`, `Epsilon`, `One`, `Pi`, `Zero`, the private constants `EInverted`, `Half`,
  `Log10Inv`, `MaximumIterations`, `HalfPi`, `QuarterPi`, `TwoPi`, the public functions `Abs`,
  `Acos`, `Asin`, `Atan`, `Atan2`, `Cos`, `Cosh`, `Exp`, `Log`, `Log10`, `Power`, `PowerN`, `Sign`,
  `Sin`, `Sinh`, `Sqrt`, `Tan`, `Tanh` and the private helpers `IsInteger` and
  `IsSignOfSinusPositive`. There is no second file and no folder structure, keep new functions in
  this file in the existing alphabetical order.
- `GlobalUsings.cs`: currently only the `#pragma warning disable IDE0065` block with no using in
  it, because the library needs nothing beyond `ImplicitUsings`.

Everything else in `src`:

- `src/.editorconfig`: the code conventions, see below.
- `src/Directory.Build.props`: sets `GenerateDocumentationFile` for every project below `src` and
  nothing else.
- `src/DecimalMath.sln.DotSettings`: a ReSharper user dictionary (`arcus`, `cosinus`, `Haemmer`,
  `H_00E4mmer`, `Sinh`, `tangens`, `Tanh`). Leave it alone.
- `src/GlobalUsings.cs`: a leftover from a Visual Studio template, a `public class Class1` in a file
  named `GlobalUsings.cs`. It is tracked, but it sits outside the project directory and is therefore
  not compiled. It has no purpose.

Repository root: `README.md` (the user documentation, with the badge list and the method list),
`Changelog.md`, `Updating.md` (the five release steps), `License.txt` (MIT), `Icon.png` (the package
icon), `BuildAndPushPackage.bat`, `Delete-BIN-OBJ-Folders.bat`, `.editorconfig` is only in `src`.
There is no test project, no `HowToUse.md`, no `.github` folder and no pipeline file.

## Build

```powershell
dotnet build src/DecimalMath.sln -c Release
```

- `GeneratePackageOnBuild` is on, so every build writes
  `src/DecimalMath/bin/Release/HaemmerElectronics.SeppPenner.DecimalMath.<version>.nupkg` and the
  matching `.snupkg`. A build is therefore also the package build, there is no separate pack step.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `Directory.Build.props` sets `GenerateDocumentationFile`, which turns a missing XML doc comment
  into `CS1591` and therefore into a build error. That is the mechanism behind the documentation
  rule below, not a style preference.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.2.7-5` for the fifth
  commit after tag `1.2.6`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/DecimalMath.sln --source https://api.nuget.org/v3/index.json`.
- There is nothing to run. The library has no entry point, so a behaviour change is verified by
  calling the function in question and comparing the result against `System.Math`, not by starting
  a program.

## Code conventions

Follow the surrounding code, `DecimalMath.cs` is consistent throughout:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members and private constants included,
  no exceptions. The summary text is indented by five spaces inside `///` in `DecimalMath.cs`, the
  two private helpers at the end of the file use one space. Both exist, do not reformat the file
  over it.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`). `DecimalMath` itself is static, so
  the rule only becomes visible in code that has instance members.
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- The functions are written as pure static functions over `decimal` and use the constants of the
  class (`Zero`, `One`, `Half`) instead of literals. Keep that, a `0m` in a new function reads as
  foreign code here.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **The class is named like its namespace.** `DecimalMath.DecimalMath` is a static class inside the
  namespace `DecimalMath`. From a nested namespace such as `DecimalMath.Tests` the plain name
  `DecimalMath` binds to the namespace, not to the class, so calling code needs an alias in its
  `GlobalUsings.cs`. Renaming the class would be a breaking change for every consumer.
- **The results are close to `System.Math`, not equal to it.** The functions are Taylor series and
  Newton iterations over `decimal`, so `Log10(10)` returns `1.0000000000000000000000000003`,
  `Exp(Log(7))` returns `7.0000000000000000000000000014` and `Sin(0.7)^2 + Cos(0.7)^2` returns
  `0.9999999999999999999999999999`. Compare with a tolerance, never with `==`, and do not "fix" the
  last digits, that is what a 28 digit series costs.
- **`MaximumIterations` is 100.** `Cos` and `Log` stop after 100 series terms even if the value has
  not converged yet, `Asin`, `Exp` and `Sqrt` instead run until two consecutive values are equal.
  That is why the accuracy of `Log` drops for arguments far away from `1`.
- **`Epsilon` is not a machine epsilon.** The public constant `Epsilon` (`1e-19`) is the tolerance
  that `IsInteger` uses to decide whether a power is an integer. Its doc comment says
  "Represents PI.", which is a copy and paste error from `Pi`.
- **`Sqrt` iterates to an exact fixed point by default.** The `epsilon` parameter defaults to `Zero`,
  so the Newton loop runs until two iterations produce exactly the same `decimal`. It terminates for
  every value tried so far, including `decimal.MaxValue`, but the parameter exists for the case
  where it does not.
- **`Atan2(0, 0)` throws.** `System.Math.Atan2(0, 0)` returns `0`, this implementation throws an
  `ArgumentException` with the message `invalid atan2 arguments`. Changing it would be a breaking
  change.
- **`Power` throws the base `Exception` type.** For a zero base with a negative power and for a
  negative base with a non-integer power the message is `Invalid Operation: ...` and the type is
  `System.Exception`, not `DivideByZeroException` or `ArgumentException`. Consumers may catch on
  that, so it stays.
- **The package id is not the assembly name.** The assembly and the namespace are `DecimalMath`, the
  package id is `HaemmerElectronics.SeppPenner.DecimalMath`, and `Icon.png`, `License.txt`,
  `README.md` and `Changelog.md` are packed into it from the repository root via `..\..` includes in
  the project file.
- **`PackageReleaseNotes` duplicates the changelog.** The project file carries the text of the
  newest `Changelog.md` entry as a single line. Both have to be updated for a release, GitVersion
  does not touch either of them.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.
- **`.gitattributes` sets `* text=auto`**, every rule of the Visual Studio template below it is
  commented out. There is no binary file in the repository that needs its own rule, `Icon.png` is
  covered by the automatic detection.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.2.7.0 (2026-08-11)** : Short description.`
3. Copy that text into `PackageReleaseNotes` in `src/DecimalMath/DecimalMath.csproj`, in the
   existing single line format `Version 1.2.7.0 (2026-08-11): Short description.`.
4. Commit that.
5. Tag the commit with the plain version number, no `v` prefix (`1.2.6`, `1.2.5`, ...). The existing
   tags are lightweight tags, create new ones the same way.
6. Push the commits and the tag.
7. Only then run `BuildAndPushPackage.bat` if the package is to be published. It deletes every `bin`
   and `obj` below `src`, builds `-c Release` and pushes the `.nupkg` and the `.snupkg` to nuget.org
   and to GitHub Packages, using `NUGET_API_KEY` and `GITHUB_API_KEY` from the environment. The tag
   has to exist first, otherwise GitVersion burns a prerelease version into the published package.

The version in `Changelog.md` has four parts (`1.2.7.0`), the tag has three (`1.2.7`). GitVersion
turns the tag into the assembly and package version, so an untagged commit produces something like
`1.2.7-5`. There is no installer to build.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
