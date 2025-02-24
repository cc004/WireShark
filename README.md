## Wireshark

Wireshark is a wire accelarator for Terraria, which can run in both **JIT** and **AOT** mode.

### Usage

#### JIT Mode  
Simply load the WireShark mod into the game and enjoy the enhanced wire performance.  

#### AOT Mode  
Currently, AOT mode is only supported on Windows, but porting it to other platforms is straightforward.  

1. **Code Generation**  
   * Load the WireShark mod into the game and enter the map once.
   * The mod will generate an `impl.cpp` file in your mod folder.

2. **Compilation**  
   * Move `impl.cpp` to the `WireSharkLib` folder.
   * Run CMake (release mode is recommended).
   * If using Visual Studio, install the [C++ CMake tools for Windows](https://learn.microsoft.com/en-us/cpp/build/cmake-projects-in-visual-studio) before proceeding.

3. **Running the Mod**  
   * Move `libWireSharkLib.dll` to your *mod* folder.
   * **Disable** the WireShark mod.
   * Load the *WireSharkRuntime* mod into the game, and enter the same map where you ran the code generation step.
