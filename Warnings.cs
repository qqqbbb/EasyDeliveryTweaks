using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDeliveryTweaks
{
    internal class Warnings
    {
        [HarmonyPatch(typeof(sHUD))]
        public class sHUDPatch
        {
            [HarmonyPrefix, HarmonyPatch("AddWarning")]
            static bool AddWarningPrefix(sHUD __instance)
            {
                return Config.warnings.Value;
            }
        }




    }
}
