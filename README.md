# ggwave.net
C# bindings for [ggwave](https://github.com/ggerganov/ggwave) data-over-sound library.


## Building
### Clone repository 
```sh
git clone --recursive --depth 1 REPOSITORY_URL
```

### Build ggwave
- Linux:
    - Install dependencies
        - Ubuntu
            ```sh
            sudo apt update
            sudo apt install make cmake ninja-build\
                g++\
                g++-aarch64-linux-gnu\
                g++-mingw-w64-x86-64
            ```
        - Arch
            ```sh
            sudo pacman -Sy
            sudo pacman -S make cmake ninja\
                gcc\
                aarch64-linux-gnu-gcc\
                mingw-w64-gcc
            ```
    - Build native library (for linux)
        ```sh
        make linux
        ```
    - Build native library (for windows)
        ```sh
        make linux-cross-win
        ```

- Windows:
    - Install dependencies
        - Msys2
            ```sh
            pacman -Syu
            pacman -S mingw-w64-x86_64-make\
                mingw-w64-x86_64-cmake\
                mingw-w64-x86_64-ninja\
                mingw-w64-x86_64-gcc\
                mingw-w64-cross-mingwarm64-gcc
            ```
    - Build native library
        ```sh
        make win
        ```
    
- Restore submodule files changed by cmake
    ```sh
    make fix-git
    ```

Built libraries are copied to `src/ggwave.net/runtimes/` automatically

### Create nuget package
```sh
dotnet pack src/ggwave.net/ggwave.net.csproj -o nuget
```
