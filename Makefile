BUILD_CONFIGURATION:=Release
SOURCE_DIR:=./ggwave
BUILD_DIR:=./ggwave/build
RUNTIMES_DIR:=./src/ggwave.net/runtimes
CMAKE_ARGS_CONFIGURE:=-S $(SOURCE_DIR) \
	-G "Ninja" \
	-DCMAKE_BUILD_TYPE=$(BUILD_CONFIGURATION) \
	-DBUILD_SHARED_LIBS=ON \
	-DGGWAVE_BUILD_TESTS=OFF \
	-DGGWAVE_BUILD_EXAMPLES=OFF \
	-DCMAKE_POLICY_VERSION_MINIMUM=3.5

export CMAKE_POLICY_VERSION_MINIMUM=3.5

all: help
help:
	@echo "=====================================[help]===================================="
	@echo "Targets:"
	@echo "linux           - compile on linux for linux"
	@echo "    x64"
	@echo "    arm64"
	@echo "linux-cross-win - compile on linux for windows"
	@echo "    x64"
	@echo "win             - compile on windows for windows"
	@echo "    x64"
	@echo "    arm64 (disabled by default)"

fix-git:
	# cmake changes README.md files in submodules (don't ask why)
	git submodule update --force --recursive

clean:
	rm -rf $(BUILD_DIR)
	rm -rf $(BUILD_DIR)-*
	rm -rf $(RUNTIMES_DIR)
	rm -rf nuget

linux: linux-x64 linux-arm64

linux-x64:
	@echo "==================================[linux-x64]=================================="
	cmake $(CMAKE_ARGS_CONFIGURE) \
		-B $(BUILD_DIR)-linux-x64 
		-DCMAKE_C_COMPILER=gcc \
		-DCMAKE_CXX_COMPILER=g++ \
		-DCMAKE_SYSTEM_NAME=Linux \
		-DCMAKE_SYSTEM_PROCESSOR=x86_64
	cmake --build $(BUILD_DIR)-linux-x64 --config $(BUILD_CONFIGURATION)
	@bash ./find-and-copy.sh \
		$(BUILD_DIR)-linux-x64 \
		libggwave.so \
		$(RUNTIMES_DIR)/linux-x64/natives
	@echo "libggwave dependencies:"
	@objdump -p src/ggwave.net/runtimes/linux-x64/natives/libggwave.so | grep 'NEEDED'

linux-arm64:
	@echo "=================================[linux-arm64]================================="
	cmake $(CMAKE_ARGS_CONFIGURE) \
		-B $(BUILD_DIR)-linux-arm64 \
		-DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc \
		-DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++ \
		-DCMAKE_SYSTEM_NAME=Linux \
		-DCMAKE_SYSTEM_PROCESSOR=aarch64
	cmake --build $(BUILD_DIR)-linux-arm64 --config $(BUILD_CONFIGURATION)
	@bash ./find-and-copy.sh \
		$(BUILD_DIR)-linux-arm64 \
		libggwave.so \
		$(RUNTIMES_DIR)/linux-arm64/natives
	@echo "libggwave dependencies:"
	@aarch64-linux-gnu-objdump -p src/ggwave.net/runtimes/linux-arm64/natives/libggwave.so | grep 'NEEDED'

linux-cross-win: linux-cross-win-x64

linux-cross-win-x64:
	@echo "=============================[linux-cross-win-x64]============================="
	cmake $(CMAKE_ARGS_CONFIGURE) \
		-B $(BUILD_DIR)-linux-cross-win-x64 \
		-DCMAKE_C_COMPILER=x86_64-w64-mingw32-gcc \
		-DCMAKE_CXX_COMPILER=x86_64-w64-mingw32-g++ \
		-DCMAKE_SYSTEM_NAME=Windows \
		-DCMAKE_SYSTEM_PROCESSOR=x86_64
	cmake --build $(BUILD_DIR)-linux-cross-win-x64 --config $(BUILD_CONFIGURATION)
	@bash ./find-and-copy.sh \
		$(BUILD_DIR)-linux-cross-win-x64 \
		libggwave.dll \
		$(RUNTIMES_DIR)/win-x64/natives
	@echo "libggwave dependencies:"
	@x86_64-w64-mingw32-objdump -p src/ggwave.net/runtimes/win-x64/natives/libggwave.dll | grep 'DLL Name'

win: win-x64

win-x64:
	@echo "===================================[win-x64]==================================="
	cmake $(CMAKE_ARGS_CONFIGURE) \
		-B $(BUILD_DIR)-win-x64 \
		-DCMAKE_C_COMPILER=gcc \
		-DCMAKE_CXX_COMPILER=g++ \
		-DCMAKE_SYSTEM_NAME=Windows \
		-DCMAKE_SYSTEM_PROCESSOR=x86_64 \
		-DCMAKE_SHARED_LINKER_FLAGS='-static'
	cmake --build $(BUILD_DIR)-win-x64 --config $(BUILD_CONFIGURATION)
	@bash ./find-and-copy.sh \
		$(BUILD_DIR)-win-x64 \
		libggwave.dll \
		$(RUNTIMES_DIR)/win-x64/natives
	@echo "libggwave dependencies:"
	@objdump -p src/ggwave.net/runtimes/win-x64/natives/libggwave.dll | grep 'DLL Name'

win-arm64:
	@echo "==================================[win-arm64]=================================="
	cmake $(CMAKE_ARGS_CONFIGURE) \
		-B $(BUILD_DIR)-win-arm64 \
		-DCMAKE_C_COMPILER=aarch64-w64-mingw32-gcc \
		-DCMAKE_CXX_COMPILER=aarch64-w64-mingw32-g++ \
		-DCMAKE_SYSTEM_NAME=Windows \
		-DCMAKE_SYSTEM_PROCESSOR=aarch64 \
		-DCMAKE_SHARED_LINKER_FLAGS='-static'
	cmake --build $(BUILD_DIR)-win-arm64 --config $(BUILD_CONFIGURATION)
	@bash ./find-and-copy.sh \
		$(BUILD_DIR)-win-arm64 \
		libggwave.dll \
		$(RUNTIMES_DIR)/win-arm64/natives
	@echo "libggwave dependencies:"
	@aarch64-w64-mingw32-objdump -p src/ggwave.net/runtimes/win-arm64/natives/libggwave.dll | grep 'DLL Name'