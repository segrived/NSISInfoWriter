msbuild /p:Configuration=Release NSISInfoWriter.sln
ilmerge /targetplatform:4.0,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" ^
    "D:\Projects\Visual Studio\NSISInfoWriter\NSISInfoWriter\bin\Debug\NSISInfoWriter.exe" ^
    "D:\Projects\Visual Studio\NSISInfoWriter\NSISInfoWriter\bin\Debug\CommandLine.dll" ^
    /out:target\nsisinfowriter.exe