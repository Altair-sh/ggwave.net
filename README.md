# ggwave.net
C# bindings for [ggwave](https://github.com/ggerganov/ggwave) data-over-sound library.


## Building
### Clone repository 
```sh
git clone --recursive --depth 1 REPOSITORY_URL
```

### Install tools and dependencies
1. gcc or clang
2. cmake
3. make or ninja


### Build ggwave
```sh
cd ggwave
mkdir build
cd build
```
If you are using cmake 4, execute this to enable compatibility with cmake 3
```sh
export CMAKE_POLICY_VERSION_MINIMUM=3.5
```

Build shared libraries
Linux:
```sh
cmake .. -DCMAKE_BUILD_TYPE=Release\
    -DBUILD_SHARED_LIBS=ON\
    -DGGWAVE_BUILD_EXAMPLES=OFF\
    -DGGWAVE_BUILD_TESTS=OFF
```

Windows:
```sh
cmake .. -DCMAKE_BUILD_TYPE=Release\
    -DBUILD_SHARED_LIBS=ON\
    -DGGWAVE_BUILD_EXAMPLES=OFF\
    -DGGWAVE_BUILD_TESTS=OFF\
    -DCMAKE_SHARED_LINKER_FLAGS="-static"
```

Build with make if your cmake produced ./Makefile
```sh
make
```

or with ninja if cmake created ./ninja.build
```sh
ninja
```

Hooray! look at your `bin/libggwave.*`

### Copy shared libraries to C# project
Copy *.so or *.dll to `src/ggwave.net/runtimes/<platform-dir>/native` where `platform-dir` is `win-x64` or whatever (see [documentation](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids))


### Create nuget package
```sh
dotnet pack src/ggwave.net/ggwave.net.csproj -o nuget
```
