## How to update the package

Every new version is released completely: changelog, README, nuget.org, GitHub Packages and a GitHub
release. None of these steps is optional.

1. Add your changes. Keep the build free of warnings, `TreatWarningsAsErrors` is enabled in every
   project: `dotnet build src/DecimalMath.sln -c Release`.
2. Run the tests: `dotnet test src/DecimalMath.sln`.
3. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.2.7.0 (2026-08-11)** : Short description.`
4. Copy that text into `PackageReleaseNotes` in `src/DecimalMath/DecimalMath.csproj`, in the single
   line format used there: `Version 1.2.7.0 (2026-08-11): Short description.`.
5. Update `README.md`. The `## Available for` list has to match the target frameworks of the
   library, the `## Available methods` block has to match the public functions.
6. Commit the changes.
7. Tag the commit with the plain version number, without a `v` prefix (`1.2.7`, `1.2.6`, ...). The
   existing tags are lightweight tags, create the new one the same way.
8. Push the commits and the tag.
9. Run `BuildAndPushPackage.bat` from the repository root while the tag is still the newest commit.
   It deletes every `bin` and `obj` below `src`, builds the release and pushes the `.nupkg` and the
   `.snupkg` to nuget.org and to the NuGet source named `github`. It needs `NUGET_API_KEY` and
   `GITHUB_API_KEY` in the environment. A push to nuget.org cannot be taken back.
10. Check that the `.nupkg` arrived on GitHub Packages,
    `https://nuget.pkg.github.com/SeppPenner/index.json`. The script only reaches it if a NuGet
    source named `github` is registered (`dotnet nuget list source`). Otherwise push the `.nupkg`
    from `src/DecimalMath/bin/Release` by hand, with a GitHub token that has the scope
    `write:packages` as API key. GitHub Packages takes no symbol packages.
11. Create the GitHub release for the tag, with the changelog sentence and the link to the version
    on nuget.org as notes:
    `gh release create 1.2.7 -R SeppPenner/DecimalMath --verify-tag --latest --title "Version 1.2.7.0 (2026-08-11)" --notes-file <file>`.

No version is ever written by hand, GitVersion derives it from the git tag plus the distance to it.
The tag therefore has to exist and be the newest commit when the package is built in step 9,
otherwise the published package carries a prerelease version such as `1.2.8-1`. The version in
`Changelog.md` has four parts (`1.2.7.0`), the tag has three (`1.2.7`).
