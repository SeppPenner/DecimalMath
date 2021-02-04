DecimalMath
====================================

DecimalMath is a library to calculate mathematic functions with the decimal data type.
The assembly was written and tested in .Net 5.0.

[![Build status](https://ci.appveyor.com/api/projects/status/w79tkhi5jy9c2uih?svg=true)](https://ci.appveyor.com/project/SeppPenner/decimalmath)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/DecimalMath.svg)](https://github.com/SeppPenner/DecimalMath/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/DecimalMath.svg)](https://github.com/SeppPenner/DecimalMath/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/DecimalMath.svg)](https://github.com/SeppPenner/DecimalMath/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/DecimalMath/master/License.txt)
[![Nuget](https://img.shields.io/badge/DecimalMath-Nuget-brightgreen.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/HaemmerElectronics.SeppPenner.DecimalMath.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/DecimalMath/badge.svg)](https://snyk.io/test/github/SeppPenner/DecimalMath)
[![Gitter](https://badges.gitter.im/DecimalMath/community.svg)](https://gitter.im/DecimalMath/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

## Available for
* NetFramework 4.6
* NetFramework 4.6.2
* NetFramework 4.7
* NetFramework 4.7.2
* NetFramework 4.8
* NetStandard 2.0
* NetStandard 2.1
* NetCore 2.1
* NetCore 3.1
* Net 5.0

## Net Core and Net Framework latest and LTS versions
* https://dotnet.microsoft.com/download/dotnet-framework
* https://dotnet.microsoft.com/download/dotnet-core
* https://dotnet.microsoft.com/download/dotnet/5.0

## Available methods:

```csharp
decimal Abs(decimal x);
decimal Acos(decimal x);
decimal Asin(decimal x);
decimal Atan(decimal x);
decimal Atan2(decimal x);
decimal Cos(decimal x);
decimal Cosh(decimal x);
decimal Exp(decimal x);
decimal Log(decimal x);
decimal Log10(decimal x);
decimal Power(decimal value, decimal pow);
decimal PowerN(decimal value, int power);
int Sign(decimal x);
decimal Sin(decimal x);
decimal Sinh(decimal x);
decimal Sqrt(decimal x, decimal epsilon = Zero);
decimal Tan(decimal x);
decimal Tanh(decimal x);
```

The project can be found on [nuget](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.DecimalMath/).

## Install

```bash
dotnet add package HaemmerElectronics.SeppPenner.DecimalMath
```

Change history
--------------

See the [Changelog](https://github.com/SeppPenner/DecimalMath/blob/master/Changelog.md).