using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class TruckTurnFix
    {
        static RotationRounding truckRR;

        public static void FixTruckTurning()
        {
            if (truckRR)
                truckRR.enabled = !Config.fixTruckTurn.Value;
        }

        [HarmonyPatch(typeof(sCarController))]
        public class sCarControllerPatch
        {
            [HarmonyPostfix, HarmonyPatch("Awake")]
            static void UpdatePostfix(sCarController __instance)
            {
                //Main.logger.LogMessage($"sCarController Awake");
                Transform t = __instance.transform.Find("Model");
                if (t != null)
                    truckRR = t.GetComponent<RotationRounding>();

                FixTruckTurning();
            }
        }


    }
}
