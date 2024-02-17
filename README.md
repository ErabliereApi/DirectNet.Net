# DirectNet.Net

C# DirectNet serial client. Use to communicate with automation direct PLC such as DL 06 and many more.

> ASCII mode is not supported.

## Installation

```
<PackageReference Include="DirectNet.Net" Version="1.0.0" />
```

## Build a release version of the GUI app.

```
dotnet publish --sc=true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
```

## References

https://support.automationdirect.com/docs/an-misc-029.pdf

https://github.com/cuchac/directnet
