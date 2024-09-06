# BMX Streets™ Scene Loader Mod

Welcome to the BMX Streets™ Scene Loader Mod, a [MelonLoader](https://melonwiki.xyz/#/) mod for the [BMX Streets™](https://store.steampowered.com/app/871540/BMX_Streets/) game available on Steam. This mod aims to improve some of the current issues with loading custom scenes including: 

- Lighting from other scenes enterfering with current scene
- Occlusion from other scenes enterfering with current scene 

## Features

- **Enhanced Logging**: Provides detailed logs for both standard operations and errors, helping in troubleshooting issues.
- **Harmony Patch Integration**: Uses Harmony to patch game methods seamlessly, enhancing the game's functionality without altering anything directly.

## Installation

1. **Download the Mod**:
   - Download the [latest release](https://github.com/thejeffreyallen/MapDLLInjector/releases) from the GitHub repository.
2. **Install MelonLoader**:
   - [Download MelonLoader](https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.Installer.exe) and ensure that it is installed in your BMX Streets game directory. MelonLoader is necessary for loading mods in BMX Streets™.
3. **Place the Mod Files**:
   - Copy the downloaded mod files into the `Mods` folder in your BMX Streets game directory typically located at `C:\Program Files (x86)\Steam\steamapps\common\BMX Streets\`.

## Usage

Once installed, the mod operates automatically:
- **Error Handling**: Errors during DLL loading are logged to the MelonLoader console with detailed messages.

## Helpful Resources

- **[Volution Modding Discord](https://discord.gg/CX56NRS87D)**

## Mod Components

### Mod.cs

An entry point class extending `MelonMod` that applies Harmony patches at the game's startup.

### Logger.cs

A utility class that provides colored logging capabilities:
- Green for standard messages.
- Red for error messages.

## Contributing

Contributions are welcome! If you have improvements or bug fixes, please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

Happy modding
