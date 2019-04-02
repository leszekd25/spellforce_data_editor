# SpellforceDataEditor with viewer
would be a fitting name for this branch before i added more features, like script builder or mod manager. Now it's more like All-in-one Spellforce Modding Toolbox. It's got many useful features, most of which are described in a [slowly expanding wiki](https://github.com/leszekd25/spellforce_data_editor/wiki). Short outline here:

# GameData Editor
Allows browsing and editing Spellforce's CFF gamedata files. This file includes info such as in-game text (in many supported languages), spell/unit/item data, and more.

# Asset Viewer
This tool enables browsing all game assets (3D models, animations, sounds and music) found in game PAK files. It requires preloading data first, which might take a short while, but it's a one time requirement.

# Script Builder
An experimental visual creator of LUA scripts. Supports all available commands in game. Sadly, untested and lacking documentation.

# Mod Manager
This feature is currently in development. It allows creating and managing mods in an intuitive way.

# Map Editor
Initial support for map viewing implemented, many bugs though. Some known bugs:

- Tile assignment is incorrect in some cases

- Camera occasionally sinks under the ground

- RAM usage might be very high on most maps

# How to use
To use the application, simply unzip it wherever you feel like.

Latest version: 23.03.2019.1
