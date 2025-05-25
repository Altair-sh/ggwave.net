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
3. make


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
```
make
```

or with ninja if cmake created ./ninja.build
```
ninja
```

Hooray! look at your `bin/libggwave.*`


