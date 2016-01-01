msbuild /p:Configuration=Release NSISInfoWriter.sln
ilrepack /targetplatform:v4 ^
    /out:target\NSISInfoWriter.exe ^
    /parallel ^
    "%~dp0\NSISInfoWriter\bin\Release\NSISInfoWriter.exe" ^
    "%~dp0\NSISInfoWriter\bin\Release\CommandLine.dll"