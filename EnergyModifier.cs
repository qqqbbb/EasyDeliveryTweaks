using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyDeliveryTweaks
{
    internal class EnergyModifier
    {
        [HarmonyPatch(typeof(sHUD))]
        public class sHUDPatch
        {
            [HarmonyPrefix, HarmonyPatch("LowerEnergy")]
            static void LowerEnergyPrefix(sHUD __instance, ref float delta)
            {
                delta *= Config.energyModifier.Value;
            }
        }
    }
}
