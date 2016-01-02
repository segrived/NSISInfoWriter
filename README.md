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

## Global information (always includes)
```
SCRIPT_GENERATE_TIME   Script generation time
```

## Common file information (can be excluded by -c)
```
FILE_NAME     Input file name
FILE_SIZE     File size in bytes
FILE_SIZE_KB  File size in kilobytes
FILE_SIZE_MB  File size in megabytes
FILE_ARCH     Target architecture (possible values: x86 and x64)
              Will not written to output if invalid PE image
```

## Version information (can be excluded by -v)
```
VI_PRODUCTIONVERSION        Product version <sup>[1](#versionfn)</sup>
VI_FILEVERSION              File version <sup>[1](#versionfn)</sup>
VI_FMT_PRODUCTIONVERSION    Product version (formatted)
VI_FMT_FILEVERSION          File version (formatted)
VI_COPYRIGHTS               Input file copyrights
VI_DESCRIPTION              Input file description
```
<a name="versionfn">1</a>: Difference between FileVersion and ProductVersion

## VCS information  (can be excluded by -s)

### Git
```
GIT_LAST_COMMIT_HASH_LONG   Last commit hash (long)
GIT_LAST_COMMIT_HASH_SHORT  Last commit hash (short)
GIT_LAST_COMMIT_DATE        Last commit date
GIT_USERNAME                Git username, from git configuration
GIT_USERMAIL                Get email address, from configuration
```

### Mercurial
```
HG_LAST_COMMIT_HASH_LONG   Last commit hash (long)
HG_LAST_COMMIT_HASH_SHORT  Last commit hash (short)
HG_LAST_COMMIT_DATE        Last commit date
HG_USERNAME                Name and email address, from configuration
```

### Subversion
```
SVN_LAST_REVISION_DATE     Last revision date
SVN_LAST_REVISION_NUMBER   Last revision number
SVN_REPO_URL               Repository URL
```

# Command line options
```
-i, --input           Required. Input executable or dll file name
-o, --output          Required. Output file name
-s, --ex-vcs          (Default: false) Exclude version control system(s) related information from output
-c, --ex-common       (Default: false) Exclude common file information from output (size, name, etc.)
-v, --ex-version      (Default: false) Exclude version information from output
-p, --prefix          (Default: "") Constants prefix in output script
-r, --repo-path       (Default: "") Path to VCS repository
                      If not specified, input file directory will be used instead
-e, --ignore-empty    (Default: false) Empty values will be rejected from output
-f, --version-format  (Default: %mj%.%mi%.%b%.%p%) Version information formation
                      Available placeholders:
                      %mj% - major version part
                      %mi% - minor version part
                      %b%  - build version part
                      %p%  - private version part
                      Example: "v%mj%.%mi% (build %b%)" => v2.7 (build 123)
-d, --date-format     (Default: yyyyMMddHHmmss) Date/time format
--help                Display this help screen.
--version             Display version information.
```