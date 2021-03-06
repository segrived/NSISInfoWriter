# NSISInfoWriter #

[![Build status](https://ci.appveyor.com/api/projects/status/iv8ty8mctn0r5q6s?svg=true)](https://ci.appveyor.com/project/segrived/nsisinfowriter)

## Usage
Place program executable file somewhere in PATH or in script directory and just add something like this into your NSIS install script:
```nsis
!define EXECUTABLE "D:\MyProject\MyExec.exe"
!define VERSIONHEADER "VersionInfo.nsh"
!system 'nsisiw.exe -i "${EXECUTABLE}" -o "${VERSIONHEADER}"'
!include /NONFATAL "${VERSIONHEADER}"
```

or with temp file

```nsis
!define EXECUTABLE "D:\MyProject\MyExec.exe"
!tempfile VERSIONHEADER
!system 'nsisiw.exe -i "${EXECUTABLE}" -o "${VERSIONHEADER}"'
!include /NONFATAL "${VERSIONHEADER}"
```

## Available constants in output file

### Common file information (can be excluded by -c)
```
FILE_NAME          Input file name
FILE_SIZE          File size in bytes
FILE_SIZE_KB       File size in kilobytes
FILE_SIZE_MB       File size in megabytes
FILE_ARCHITECTURE  Target architecture (possible values: x86 and x64)
                   Will not written to output if invalid PE image
```

### Metainformation (can be excluded by -v)

#### For PE images:
```
VI_PRODUCTIONVERSION        Product version <sup>[1](#versionfn)</sup>
VI_FILEVERSION              File version <sup>[1](#versionfn)</sup>
VI_FMT_PRODUCTIONVERSION    Product version (formatted)
VI_FMT_FILEVERSION          File version (formatted)
VI_COPYRIGHTS               Input file copyrights
VI_DESCRIPTION              Input file description
VI_PRODUCT_NAME             Product name
```
<a name="versionfn">1</a>: [Difference between FileVersion and ProductVersion](http://stackoverflow.com/questions/752162/whats-the-difference-between-a-dlls-fileverison-and-productversion)

#### For JRE files:
For JRE files, all information from META-INF/MANIFEST.MF is available, with some modification:
- All keys started with ```VI_``` prefix
- All ```-``` chars in key replaces with ```_```
- Keys also upcased

So ```Implementation-Version``` information will be avalible with key ```VI_IMPLEMENTATION_VERSION```

### VCS information  (can be excluded by -s)

#### Git
```
GIT_LAST_COMMIT_HASH_LONG   Last commit hash (long)
GIT_LAST_COMMIT_HASH_SHORT  Last commit hash (short)
GIT_LAST_COMMIT_DATE        Last commit date
GIT_USERNAME                Git username, from git configuration
GIT_USERMAIL                Get email address, from configuration
```

#### Mercurial
```
HG_LAST_COMMIT_HASH_LONG   Last commit hash (long)
HG_LAST_COMMIT_HASH_SHORT  Last commit hash (short)
HG_LAST_COMMIT_DATE        Last commit date
HG_USERNAME                Name and email address, from configuration
```

#### Subversion
```
SVN_LAST_REVISION_DATE     Last revision date
SVN_LAST_REVISION_NUMBER   Last revision number
SVN_REPO_URL               Repository URL
```

## Command line options
**Required arguments:**
```
-i, --input           Required. Input executable or dll file name
-o, --output          Required. Output file name
```

**Optional Arguments:**
```
-s, --ex-vcs          (Default: false) Exclude version control system(s) related information from output
-c, --ex-common       (Default: false) Exclude common file information from output (size, name, etc.)
-v, --ex-version      (Default: false) Exclude version information from output
-p, --prefix          (Default: "") Constants prefix in output script
-r, --repo-path       (Default: "") Path to VCS repository
                      If not specified, input file directory will be used instead
-e, --inlcude-empty   (Default: false) Empty and zero values will be included to output
--version-format      (Default: %mj%.%mi%.%b%.%p%) Version information formation
                      Available placeholders:
                      %mj% - major version part
                      %mi% - minor version part
                      %b%  - build version part
                      %p%  - private version part
                      Example: "v%mj%.%mi% (build %b%)" => v2.7 (build 123)
                      Note: works only with PE images at this time
--date-format         (Default: yyyy-MM-dd_HH-mm-ss) Date/time format
--prepend             (Default: false) Instead of overriding output file, content will be prepended to file.
                      If output file already include prepended data, this content will be removed
--debug               Display log messages
--help                Display this help screen.
--version             Display version information.
```

## License

Copyright (c) 2015 segrived

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.