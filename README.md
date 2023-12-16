## Wireshark

Wireshark is a wire accelarator for terraria, which can run in both jit and aot mode.

### Usage

#### JIT mode

Just load WireShark mod into the game and enjoy

#### AOT mode

Now that only Windows platform is supported, but migrating to other platforms is easy.

1. Codegen

Load WireShark mod into game, and enter the map once. The mod will write `impl.cpp` into your mod folder.

2. Compile

move the file `impl.cpp` to WireSharkLib folder and run cmake, release mode is suggested.

3. Run

move the `libWireSharkLib.dll` to your mod folder, **disable the WireShark mod** and load WireSharkRuntime mod into the game and enter the same map as which you run codegen on. 