# Relay Link Menu Addon

A client-side [Puck](https://store.steampowered.com/app/2994020) mod. Addon for [Universal Relay Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3707769349).

Universal Relay Mod shows its relay picker when you click a server in the server browser. Steam join links and "Join Game" from a Steam friend skipped that picker and connected directly. This addon opens the relay picker for those too.

- Requires Universal Relay Mod. Without it, the addon does nothing and you connect normally.
- Only servers Universal Relay Mod supports are affected.
- No network requests, no file access, no data collection.

## How it works
Harmony prefixes on `ConnectionManagerController.Event_OnGotLaunchCommandLine` and `Event_OnGameRichPresenceJoinRequested`. If the link's server has relay options, the addon calls the relay mod's `RelaySelectionUI.ShowRelaySelectionForServer` (via reflection, so there is no compile-time dependency) and skips the direct connect.

## Building
1. Install the .NET 8 SDK.
2. `dotnet build -c Release`
3. The DLL is copied to `Puck\Plugins\RelayLinkMenuAddon\`. If Puck is not in the default Steam folder: `dotnet build -c Release -p:PuckDir="D:\path\to\Puck"`.

## License
MIT
