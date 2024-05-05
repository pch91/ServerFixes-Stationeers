using Assets.Scripts;
using Assets.Scripts.Localization2;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Serialization;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ServerFixes.patch
{
    [HarmonyPatch]
    class MainServerFixesPatch
    {
        /**
         * for able load mod after PlayerPrefs in some time the game load modconfig.xml before this and give a error.
         */
        [HarmonyPatch(typeof(ModConfigUpgrader), "Upgrade")]
        [HarmonyPrefix]
        private static bool Testemodup(ref string path,ref ModConfig __result)
        {
            PlayerPrefs.SetInt("updated-mod-config", 1);
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " updatemod path of load mods: " + Environment.CurrentDirectory+"/"+path, ServerFixes.Logs.INFO);
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " updatemod Exists: " + File.Exists(Environment.CurrentDirectory + "/" + path), ServerFixes.Logs.INFO);

            OldModConfig oldModConfig = XmlSerialization.Deserialize<OldModConfig>(Environment.CurrentDirectory + "/" + path, "ModConfig");
            
            ModConfig modConfig = new ModConfig();

            if (oldModConfig == null)
            {
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " oldModConfig null. ", ServerFixes.Logs.INFO);
            __result = modConfig;
            }
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " oldModConfig ModEnable SIZE: " + oldModConfig.EnabledMods.Count, ServerFixes.Logs.INFO);

            modConfig.Mods = new List<ModData>();
            foreach (ulong enabledMod in oldModConfig.EnabledMods)
            {
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " ModEnable " + enabledMod, ServerFixes.Logs.INFO);

            modConfig.Mods.Add(new ModData
                {
                    Id = enabledMod,
                    IsEnabled = true
                });
            }
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " oldModConfig ModDisable SIZE: " + oldModConfig.DisabledMods.Count, ServerFixes.Logs.INFO);

            foreach (ulong disabledMod in oldModConfig.DisabledMods)
            {
            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " ModDisable " + disabledMod, ServerFixes.Logs.INFO);

            modConfig.Mods.Add(new ModData
                {
                    Id = disabledMod,
                    IsEnabled = false
                });
            }

            if (File.Exists(Environment.CurrentDirectory + "/" + path) && oldModConfig.EnabledMods.Count < 1 && oldModConfig.DisabledMods.Count < 1)
            {
                ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " load mods from default. ", ServerFixes.Logs.INFO);
                modConfig = XmlSerialization.Deserialize<ModConfig>("modconfig.xml", "");
            }

            ServerFixes.log("MainServerFixesPatch :: ModConfigUpgrader --> " + " updatemod saindo ", ServerFixes.Logs.INFO);

            modConfig.SaveXml(path);
            __result = modConfig;

            return false;
        }


    }
}