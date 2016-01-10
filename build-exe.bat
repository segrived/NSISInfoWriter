msbuild /p:Configuration=Release NSISInfoWriter.sln
ilrepack /targetplatform:v4 ^
    /out:target\nsisiw.exe ^
    /parallel ^
    /ndebug ^
    "%~dp0\NSISInfoWriter\bin\Release\nsisiw.exe" ^
    "%~dp0\NSISInfoWriter\bin\Release\CommandLine.dll"