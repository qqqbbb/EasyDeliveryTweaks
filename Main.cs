using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "qqqbbb.EasyDelivery.tweaks";
        public const string PLUGIN_NAME = "Tweaks";
        public const string PLUGIN_VERSION = "1.1.0";

        public static ConfigFile config;
        public static ManualLogSource logger;


        private void Awake()
        {
            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
            config = this.Config;
            EasyDeliveryTweaks.Config.Bind();
            logger = Logger;
            Logger.LogInfo($"Plugin {PLUGIN_NAME} {PLUGIN_VERSION} is loaded");
            //Logger.LogInfo("SceneName " + SceneManager.GetActiveScene().name);
        }

    }
}
