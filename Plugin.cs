using System;
using HarmonyLib;
using UnityEngine;

namespace RelayLinkMenuAddon
{
    /// <summary>
    /// Entry point. Puck's loader picks the first class implementing IPuckPlugin.
    /// </summary>
    public class Plugin : IPuckPlugin
    {
        internal const string Id = "RelayLinkMenuAddon";
        internal const string LogPrefix = "[RelayLinkMenuAddon] ";

        private static readonly Harmony harmony = new Harmony(Id);

        public bool OnEnable()
        {
            try
            {
                harmony.PatchAll(typeof(Plugin).Assembly);
                Debug.Log(LogPrefix + "Enabled.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(LogPrefix + "Harmony patch failed: " + e);
                return false;
            }
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                Debug.Log(LogPrefix + "Disabled.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(LogPrefix + "Harmony unpatch failed: " + e);
                return false;
            }
        }
    }
}
