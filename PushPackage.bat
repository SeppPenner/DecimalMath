dotnet nuget push "src\DecimalMath\bin\Release\HaemmerElectronics.SeppPenner.DecimalMath.*.nupkg" -s "github" --skip-duplicate
dotnet nuget push "src\DecimalMath\bin\Release\HaemmerElectronics.SeppPenner.DecimalMath.*.nupkg" -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
PAUSE