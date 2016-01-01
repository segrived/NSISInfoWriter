msbuild /p:Configuration=Release NSISInfoWriter.sln
ilmerge /targetplatform:4.0,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" ^
    "%~dp0\NSISInfoWriter\bin\Release\NSISInfoWriter.exe" ^
    "%~dp0\NSISInfoWriter\bin\Release\CommandLine.dll" ^
    /out:target\nsisinfowriter.exe