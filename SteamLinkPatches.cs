using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace RelayLinkMenuAddon
{
    /// <summary>
    /// Steam join links reach the game two ways, both ending in
    /// ConnectionManagerController calling Client_StartClient directly:
    ///   1. steam://run/2994020//+ipAddress ... (launch / URL params)  -> Event_OnGotLaunchCommandLine
    ///   2. "Join Game" from Steam friends (rich presence)              -> Event_OnGameRichPresenceJoinRequested
    /// The relay mod only intercepts server-browser clicks, so these paths skip its menu.
    /// We intercept them and open the relay menu instead.
    /// </summary>
    internal static class SteamLinkInterceptor
    {
        private const ushort DefaultPort = 30609; // same default the game uses

        /// <returns>true to let the game connect normally, false if we showed the relay menu.</returns>
        public static bool TryShowRelayMenu(Dictionary<string, object> message, string source)
        {
            try
            {
                if (message == null || !message.TryGetValue("args", out object raw) || !(raw is string[] args))
                {
                    return true;
                }

                string ipAddress = Utils.GetCommandLineArgument("+ipAddress", args);
                if (string.IsNullOrEmpty(ipAddress))
                {
                    return true;
                }

                ushort port = ushort.TryParse(Utils.GetCommandLineArgument("+port", args), out ushort p) ? p : DefaultPort;
                string password = Utils.GetCommandLineArgument("+password", args);

                if (!RelayModBridge.IsAvailable)
                {
                    return true;
                }

                // Link already points at a relay: nothing to choose.
                if (RelayModBridge.IsRelayAddress(ipAddress))
                {
                    return true;
                }

                if (!RelayModBridge.HasRelayOptions(ipAddress, port))
                {
                    return true;
                }

                Debug.Log($"{Plugin.LogPrefix}{source}: opening relay menu for {ipAddress}:{port}");
                RelayModBridge.ShowRelayMenu(ipAddress, port, password);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return true; // never block the connection because of an addon bug
            }
        }
    }

    [HarmonyPatch(typeof(ConnectionManagerController), "Event_OnGotLaunchCommandLine")]
    internal static class LaunchCommandLinePatch
    {
        private static bool Prefix(Dictionary<string, object> message)
        {
            return SteamLinkInterceptor.TryShowRelayMenu(message, "Launch link");
        }
    }

    [HarmonyPatch(typeof(ConnectionManagerController), "Event_OnGameRichPresenceJoinRequested")]
    internal static class RichPresenceJoinPatch
    {
        private static bool Prefix(Dictionary<string, object> message)
        {
            return SteamLinkInterceptor.TryShowRelayMenu(message, "Steam friend join");
        }
    }
}
