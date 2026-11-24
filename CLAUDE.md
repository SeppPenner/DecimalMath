# Project rules for Claude

## What this is

DecimalMath is a class library that reimplements the functions of `System.Math` for the `decimal`
data type, so that a calculation keeps 28 significant decimal digits instead of the 15 to 17 of
`double`. The repository is published as the NuGet package
[HaemmerElectronics.SeppPenner.DecimalMath](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/),
so every change ends in a package, not in an application.

One solution `src/DecimalMath.sln` with two projects:

- `src/DecimalMath/DecimalMath.csproj`, the library itself, multi targets `net10.0;net11.0`.
- `src/DecimalMath.Tests/DecimalMath.Tests.csproj`, MSTest, single target `net11.0`, added in
  version 1.2.7.0.

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

Layout inside `src/DecimalMath.Tests`:

- `DecimalMathTests.cs`: the only test class, 30 tests. The functions are checked against their
  counterpart in `System.Math` with a tolerance, the values that the implementation reaches by
  multiplication alone are checked exactly, and every exception of the library has its own test.
  Four of the tests are regression tests for the bugs fixed in version 1.2.7.0, they fail against
  the code of 1.2.6.0: `AbsReturnsAZeroWithoutTheSignBit`, `LogOfOneIsExactlyZero`,
  `PowerReturnsExactResultsForIntegerPowers` and `PowerNReturnsRepresentableResultsWithoutAnOverflow`.
- `GlobalUsings.cs`: the usings of the test project, including the alias `DecMath`.

Everything else in `src`:

- `src/.editorconfig`: the code conventions, see below.
- `src/Directory.Build.props`: sets `GenerateDocumentationFile` for every project below `src` and
  nothing else.
- `src/DecimalMath.sln.DotSettings`: a ReSharper user dictionary (`arcus`, `cosinus`, `Haemmer`,
  `H_00E4mmer`, `Sinh`, `tangens`, `Tanh`). Leave it alone.

Repository root: `README.md` (the user documentation, with the badge list and the method list),
`Changelog.md`, `Updating.md` (the five release steps), `License.txt` (MIT), `Icon.png` (the package
icon), `BuildAndPushPackage.bat`, `Delete-BIN-OBJ-Folders.bat`, `.editorconfig` is only in `src`.
There is no `HowToUse.md`, no `.github` folder and no pipeline file.

## Build

Install the .NET 11 SDK to build the solution. The library targets `net10.0` and `net11.0`, the test
project `net11.0`. Keep the test project on the newest framework the library targets.

```powershell
dotnet build src/DecimalMath.sln -c Release
```

```powershell
dotnet test src/DecimalMath.sln
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
- `dotnet test` runs 30 tests, they need no network and no file outside the repository. Never claim a
  test run happened without running it.
- The MSTest analyzers are active and their diagnostics are errors here as well. `MSTEST0032`
  rejects an `Assert.AreEqual` of two compile time constants, which is why the constants of the
  library are checked through the helper `AssertMatchesSystemMath` and not directly. MSTest 4 no
  longer has `Assert.ThrowsException`, the tests use `Assert.ThrowsExactly<T>` with a block bodied
  lambda.
- There is nothing to run. The library has no entry point, so a behaviour change beyond the tests is
  verified by calling the function in question and comparing the result against `System.Math`, not
  by starting a program.

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
  `DecimalMath` binds to the namespace, not to the class, which is why the test project aliases it as
  `global using DecMath = DecimalMath.DecimalMath;` and calls `DecMath.Sin(x)`. The same applies to a
  cref in an XML doc comment, so the test file names the class in plain text. Renaming the class would
  be a breaking change for every consumer.
- **The results are close to `System.Math`, not equal to it.** The functions are Taylor series and
  Newton iterations over `decimal`, so `Log10(10)` returns `1.0000000000000000000000000003`,
  `Exp(Log(7))` returns `7.0000000000000000000000000014` and `Sin(0.7)^2 + Cos(0.7)^2` returns
  `0.9999999999999999999999999999`. Compare with a tolerance, never with `==`, and do not "fix" the
  last digits, that is what a 28 digit series costs. The exception is `Power` with an integer power,
  it goes through `PowerN`, which only multiplies and therefore stays exact, while a non-integer
  power goes through `Exp(pow * Log(value))` and inherits the error of both series.
- **`MaximumIterations` is 100.** `Cos` and `Log` stop after 100 series terms even if the value has
  not converged yet, `Asin`, `Exp` and `Sqrt` instead run until two consecutive values are equal.
  That is why the accuracy of `Log` drops for arguments far away from `1`.
- **`Epsilon` is not a machine epsilon.** The public constant `Epsilon` (`1e-19`) is the tolerance
  that `IsInteger` uses to decide whether a power is an integer.
- **`Power` casts an integer power to `int`.** A power above `int.MaxValue` throws an
  `OverflowException` before anything is calculated. Such a result exceeds the range of `decimal`
  anyway, unless the base lies between `-1` and `1`.
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

**Every new version is released completely.** Changelog, README, nuget.org, GitHub Packages and a
GitHub release belong to every version, none of these steps is optional and none of them waits for
a separate request.

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.2.7.0 (2026-08-11)** : Short description.`
3. Copy that text into `PackageReleaseNotes` in `src/DecimalMath/DecimalMath.csproj`, in the
   existing single line format `Version 1.2.7.0 (2026-08-11): Short description.`. It is the only
   place in the build files that names a version, and it is duplicated from the changelog by hand.
4. Update `README.md` for every version. The `## Available for` list has to match the
   `TargetFrameworks` of the library, the `## Available methods` block has to match the public
   functions, and nothing there may describe a state the package has left behind.
5. Commit that.
6. Tag the commit with the plain version number, no `v` prefix (`1.2.6`, `1.2.5`, ...). The existing
   tags are lightweight tags, create new ones the same way.
7. Push the commits and the tag.
8. Only then build the package, and build it while the tag is the newest commit. GitVersion takes
   the version from the tag plus the distance to it, so a build before the tag or after another
   commit produces a prerelease version like `1.2.8-3+Branch.master.Sha...`, and that version would
   end up on nuget.org. `git describe --tags` has to answer with the plain tag.
9. Publish to nuget.org with `BuildAndPushPackage.bat`, started from the repository root, its first
   line is `cd .\src`. It deletes every `bin` and `obj` below `src`, builds `-c Release` and pushes
   the `.nupkg` and the `.snupkg` to nuget.org with `%NUGET_API_KEY%` and to the NuGet source named
   `github` with `%GITHUB_API_KEY%`. It ends with `PAUSE`, so it wants a console. Call it with its
   full path: the Claude Code shell sets `NoDefaultCurrentDirectoryInExePath=1`, so cmd does not
   find a bare `BuildAndPushPackage.bat` in the current directory. Because it always rebuilds, it is
   only correct as long as no commit sits on top of the tag. If one does, push the package that was
   built from the tagged state instead, by its file name and not with the wildcard:
   `dotnet nuget push HaemmerElectronics.SeppPenner.DecimalMath.1.2.7.nupkg -s "nuget.org" --skip-duplicate -k %NUGET_API_KEY%`
   in `src/DecimalMath/bin/Release`. That command takes the matching `.snupkg` with it. A push to
   nuget.org cannot be taken back, the version is burned even after unlisting. nuget.org needs a few
   minutes to validate a new version before it is listed.
10. Publish the same `.nupkg` to GitHub Packages,
    `https://nuget.pkg.github.com/SeppPenner/index.json`. The script only reaches it if a NuGet
    source named `github` is registered on the machine, `dotnet nuget list source` shows that. On
    2026-11-24 only nuget.org was registered here, so the two `github` lines of the script fail and
    the push has to be done by hand in `src/DecimalMath/bin/Release`:
    `dotnet nuget push HaemmerElectronics.SeppPenner.DecimalMath.1.2.7.nupkg -s https://nuget.pkg.github.com/SeppPenner/index.json --skip-duplicate --api-key <token>`.
    The API key is a GitHub token with the scope `write:packages`, for example from
    `gh auth token`. If `gh auth status` lists no `write:packages`, the user has to run
    `gh auth refresh -h github.com -s write:packages,read:packages`, it needs a browser login.
    GitHub Packages takes no symbol packages, push the `.nupkg` only.
11. Create the GitHub release:
    `gh release create 1.2.7 -R SeppPenner/DecimalMath --verify-tag --latest --title "Version 1.2.7.0 (2026-08-11)" --notes-file <file>`.
    The notes are the changelog sentence plus the line
    `NuGet: https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/1.2.7`. Write
    credits as `` `@name` `` in backticks, a bare `@name` notifies that person again.

`Updating.md` holds the same steps for the reader of the repository, both files have to stay in sync.
The four push lines in the batch are prefixed with `@`, because `ECHO` is on at that point and cmd
echoes a command line after the variable expansion, which would write both API keys into the console
output. Do not remove those prefixes. Deleting `bin` before a push matters for the same reason it is
in the batch: `GeneratePackageOnBuild` leaves a `.nupkg` of every intermediate build in there, and the
push uses the wildcard `*.nupkg`.

The version in `Changelog.md` has four parts (`1.2.7.0`), the tag has three (`1.2.7`). GitVersion
turns the tag into the assembly and package version, so an untagged commit produces something like
`1.2.7-5`. There is no installer to build.

nuget.org holds every tagged version from 1.0.0 to 1.2.7. On 2026-11-24 no tag had a GitHub release
yet, and GitHub Packages held 1.1.0 to 1.2.0 and 1.2.4 to 1.2.7, the older history there is
incomplete.

## Git

- Include the full GitHub pull request or issue URL in the body of each related commit on a `Refs:`
  line. Use the [GitHub issue skill](.claude/skills/github-issue/SKILL.md) for issue work and closeout.
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
