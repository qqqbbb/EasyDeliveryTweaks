using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class FuelConsumptionModifier
    {
        [HarmonyPatch(typeof(sHUD))]
        public class sHUDPatch
        {
            static float fuelBefore;

            [HarmonyPrefix, HarmonyPatch("DoFuelMath")]
            static void DoFuelMathPrefix(sHUD __instance)
            {
                fuelBefore = __instance.fuel;
            }

            [HarmonyPostfix, HarmonyPatch("DoFuelMath")]
            static void DoFuelMathPostfix(sHUD __instance)
            {
                if (Config.fuelModifier.Value == 1)
                    return;
                else if (Config.fuelModifier.Value == 0)
                {
                    __instance.fuel = fuelBefore;
                    return;
                }
                float difference = __instance.fuel - fuelBefore;
                difference *= Config.fuelModifier.Value;
                __instance.fuel = fuelBefore + difference;
            }


        }
    }
}
