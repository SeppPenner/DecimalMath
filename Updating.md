## How to update the package

1. Add your changes. Keep the build free of warnings, `TreatWarningsAsErrors` is enabled in every
   project: `dotnet build src/DecimalMath.sln -c Release`.
2. Run the tests: `dotnet test src/DecimalMath.sln`.
3. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.2.7.0 (2026-08-11)** : Short description.`
4. Copy that text into `PackageReleaseNotes` in `src/DecimalMath/DecimalMath.csproj`, in the single
   line format used there: `Version 1.2.7.0 (2026-08-11): Short description.`.
5. Commit the changes.
6. Tag the commit with the plain version number, without a `v` prefix (`1.2.7`, `1.2.6`, ...). The
   existing tags are lightweight tags, create the new one the same way.
7. Push the commits and the tag.
8. Run `BuildAndPushPackage.bat` from the repository root. It deletes every `bin` and `obj` below
   `src`, builds the release and pushes the `.nupkg` and the `.snupkg` to nuget.org and to GitHub
   Packages. It needs `NUGET_API_KEY` and `GITHUB_API_KEY` in the environment.

No version is ever written by hand, GitVersion derives it from the git tag. The tag therefore has to
exist before step 8, otherwise the published package carries a prerelease version such as `1.2.8-1`.
The version in `Changelog.md` has four parts (`1.2.7.0`), the tag has three (`1.2.7`).
