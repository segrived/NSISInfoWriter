# NSISInfoWriter #

[![Build status](https://ci.appveyor.com/api/projects/status/iv8ty8mctn0r5q6s?svg=true)](https://ci.appveyor.com/project/segrived/nsisinfowriter)

## Usage
Just add something like this into your NSIS install script:
```nsis
!define EXECUTABLE "D:\MyProject\MyExec.exe"
!define VERSIONHEADER "VersionInfo.nsh"
!system 'NsisInfoWriter.exe -i "${EXECUTABLE}" -o "${VERSIONHEADER}"'
!include /NONFATAL "${VERSIONHEADER}"
```

# Available constants in output file

Unprefixed versions

## Common file information
```
FILE_NAME     Input file name
FILE_SIZE     File size in bytes
FILE_SIZE_KB  File size in kilobytes
FILE_SIZE_MB  File size in megabytes
```

## Version information
```
VI_PRODUCTIONVERSION      Product version <sup>[1](#versionfn)</sup>
VI_FILEVERSION            File version <sup>[1](#versionfn)</sup>
VI_FMT_PRODUCTIONVERSION  Product version (formatted)
VI_FMT_FILEVERSION        File version (formatted)
VI_COPYRIGHTS             Input file copyrights
VI_DESCRIPTION            Input file description
``` 
## VCS information (only git at this time)
```
GIT_LAST_COMMIT_HASH_LONG   Last commit hash (long)
GIT_LAST_COMMIT_HASH_SHORT  Last commit hash (short)
GIT_LAST_COMMIT_DATE        Last commit date
GIT_USERNAME                Git username, from git configuration
GIT_USERMAIL                Get email address, from git configuration
```
<a name="versionfn">1</a>: Difference between FileVersion and ProductVersion

# Command line options
```
-i, --input           Required. Input executable or dll file name
-o, --output          Required. Output file name
-g, --ex-git          (Default: false) Exclude git related information from output
-c, --ex-common       (Default: false) Exclude common file information from output (size, name, etc.)
-v, --ex-version      (Default: false) Exclude version information from output
-p, --prefix          (Default: "") Constants prefix in output script
-e, --ignore-empty    (Default: false) Empty values will be rejected from output
-f, --format          (Default: %mj%.%mi%.%b%.%p%) Version information formation
	Available placeholders:
	%mj% - Major version part
	%mi% - ninor version part
    %b%  - build version part
    %p%  - private version part
    Example: "v%mj%.%mi% (build %b%)"
--help                Display this help screen.
--version             Display version information.
```