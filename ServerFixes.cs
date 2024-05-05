using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerFixes.patch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServerFixes
{
    [BepInPlugin("ServerFixes", "ServerFixes", "1.0.0.0")]
    public class ServerFixes : BaseUnityPlugin
    {
        public static ServerFixes Instance;

        private static string loglevel = "INFO";

        public enum Logs
        {
            DEBUG = 1,
            ERROR = 2,
            INFO = 0,
        }

        public static void log(string line, Logs level)
        {
            //Debug.Log((int)Enum.Parse(typeof(Logs), loglevel));

            if ((int)Enum.Parse(typeof(Logs), loglevel) - (int)level >= 0)
            {
                Debug.Log("[" + level + "    :   ServerFixes] " + line);
            }
        }


        public static Dictionary<string, object> fmainconfig = new Dictionary<string, object>();
        public static Dictionary<string, object> fconfigEvents = new Dictionary<string, object>();
        private Dictionary<string, object> mainconfigs = new Dictionary<string, object>();


        private void Awake()
        {
            try
            {
                log("Start - ServerFixes", Logs.INFO);
                Instance = this;
                //Harmony.DEBUG = true;
                Handleconfig();
                if (bool.Parse(StaticAttributes.configs["EnabledMod"].ToString()))
                {
                    var harmony = new Harmony("net.pch91.stationeers.ServerFixes.patch");
                        log("Load - MainServerFixesPatch", Logs.INFO);
                        harmony.PatchAll(typeof(MainServerFixesPatch));
                }
                log("ServerFixes - Finish patch", Logs.INFO);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Handleconfig()
        { 
            mainconfigs.Add("LogEnabled", Config.Bind("0 - General configuration", "Log Level", "info", "Enable or disable logs. values can be debug , info or error"));
            mainconfigs.Add("EnabledMod", Config.Bind("0 - General configuration", "Eneble mod", true, "Enable or disable mod. values can be false or true"));

            loglevel = (mainconfigs["LogEnabled"] as ConfigEntry<string>).Value.ToUpper();

            StaticConfig();
        }

        private void StaticConfig()
        {
            StaticAttributes.configs.Add("EnabledMod", (mainconfigs["EnabledMod"] as ConfigEntry<bool>).Value);
        }
    }
}