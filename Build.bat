@echo off
IF EXIST Tools\FAKE GOTO BUILD
:: Install FAKE via nuget
Tools\nuget.exe Install FAKE -OutputDirectory Tools -ExcludeVersion

:BUILD
Tools\FAKE\tools\Fake.exe Build.fsx %1