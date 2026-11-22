using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RelayLinkMenuAddon
{
    /// <summary>
    /// Talks to John's Universal Relay Mod (workshop 3707769349) through reflection.
    /// No compile-time reference: if the base mod is missing or disabled,
    /// IsAvailable is false and the game behaves normally.
    /// </summary>
    internal static class RelayModBridge
    {
        private const string HelpersType = "JohnRelayMod.RelayRouterHelpers";
        private const string UiType = "JohnRelayMod.RelaySelectionUI";

        private static MethodInfo findServerEntry;      // (string, ushort) -> ServerRelayEntry
        private static MethodInfo isConfiguredRelay;    // (string) -> bool
        private static MethodInfo showRelaySelection;   // (string, ushort, string) -> void

        /// <summary>Re-resolves every call: the base mod can be enabled after this one.</summary>
        public static bool IsAvailable
        {
            get
            {
                if (findServerEntry != null && showRelaySelection != null)
                {
                    return true;
                }

                Type helpers = AccessTools.TypeByName(HelpersType);
                Type ui = AccessTools.TypeByName(UiType);
                if (helpers == null || ui == null)
                {
                    return false;
                }

                findServerEntry = AccessTools.Method(helpers, "FindServerEntry", new[] { typeof(string), typeof(ushort) });
                isConfiguredRelay = AccessTools.Method(helpers, "IsConfiguredRelayAddress", new[] { typeof(string) });
                showRelaySelection = AccessTools.Method(ui, "ShowRelaySelectionForServer", new[] { typeof(string), typeof(ushort), typeof(string) });

                bool ok = findServerEntry != null && showRelaySelection != null;
                if (!ok)
                {
                    Debug.LogWarning(Plugin.LogPrefix + "Relay mod found but its API changed; addon inactive.");
                }
                return ok;
            }
        }

        public static bool HasRelayOptions(string ipAddress, ushort port)
        {
            return findServerEntry.Invoke(null, new object[] { ipAddress, port }) != null;
        }

        public static bool IsRelayAddress(string ipAddress)
        {
            return isConfiguredRelay != null && (bool)isConfiguredRelay.Invoke(null, new object[] { ipAddress });
        }

        public static void ShowRelayMenu(string ipAddress, ushort port, string password)
        {
            showRelaySelection.Invoke(null, new object[] { ipAddress, port, password });
        }
    }
}
