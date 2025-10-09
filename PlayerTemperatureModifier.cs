using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class PlayerTemperatureModifier
    {
        static float defaultTemperatureRate;

        [HarmonyPatch(typeof(sHUD))]
        public class sHUDPatch
        {
            [HarmonyPostfix, HarmonyPatch("Start")]
            static void StartPostfix(sHUD __instance)
            {
                if (defaultTemperatureRate == 0)
                    defaultTemperatureRate = __instance.temperatureRate;
            }

            [HarmonyPrefix, HarmonyPatch("DoTemperature")]
            static void DoTemperaturePrefix(sHUD __instance)
            {
                __instance.temperatureRate = defaultTemperatureRate * Config.coldModifier.Value;
                //if (Input.GetKeyDown(KeyCode.V))
                //    Util.DisplayText("temperatureBuff " + __instance.dayNightCycle.temperatureBuff);
            }

        }
    }
}
