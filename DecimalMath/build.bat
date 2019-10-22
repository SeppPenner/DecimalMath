dotnet build DecimalMath.sln -c Release
xcopy /s .\DecimalMath\bin\Release ..\Nuget\Source\
pause