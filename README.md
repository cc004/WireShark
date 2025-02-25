## Wireshark

Wireshark is a wire accelarator for Terraria, which can run in both **JIT** and **AOT** mode.

### Usage

#### JIT Mode  
Simply load the WireShark mod into the game and enjoy the enhanced wire performance.  

#### AOT Mode  
Currently, AOT mode is only supported on Windows, but porting it to other platforms is straightforward.  

1. **Code Generation**  
   * Load the WireShark mod into the game and enable AOT mode from mod config.
   * Enter the map once. The mod will generate an `impl.cpp` file in your mod folder.

2. **Compilation**  
   * Move `impl.cpp` to the `WireSharkLib` folder.
   * Run CMake (release mode is recommended).
   * If using Visual Studio, install the [C++ CMake tools for Windows](https://learn.microsoft.com/en-us/cpp/build/cmake-projects-in-visual-studio) before proceeding.

3. **Running the Mod**  
   * Move `libWireSharkLib.dll` to your *mod* folder.
   * **Disable** the WireShark mod.
   * Load the *WireSharkRuntime* mod into the game, and enter the same map where you ran the code generation step.

### Configuration

- **Parallel Thread Count** (default: `1x`):  
  Set to `1` if there are bugs. A higher value requires more memory during preprocessing.  
  *Only available when "No Order in Wires" is set to false.*

- **No Order in Wires** (default: `on`):  
  If the order on the same wire matters, enabling this option will speed up preprocessing, but the circuit might behave slightly differently than in vanilla logic.

- **Enable AOT Mode** (default: `off`):  
  When enabled, the game will perform additional caching and generate an `impl.cpp` file for AOT mode. This process is slow and not required for JIT mode.
