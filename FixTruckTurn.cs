using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class FixTruckTurn
    {
        static RotationRounding truckRR;

        [HarmonyPatch(typeof(sCarController))]
        public class sCarControllerPatch
        {
            [HarmonyPostfix, HarmonyPatch("Update")]
            static void UpdatePostfix(sCarController __instance)
            {
                if (truckRR == null)
                {
                    Transform t = __instance.transform.Find("Model");
                    if (t != null)
                        truckRR = t.GetComponent<RotationRounding>();
                }
                FixTruckTurning();
            }

            private static void FixTruckTurning()
            {
                if (truckRR == null)
                    return;

                if (Config.fixTruckTurn.Value && truckRR.isActiveAndEnabled)
                {
                    truckRR.enabled = false;
                }
                else if (Config.fixTruckTurn.Value == false && truckRR.isActiveAndEnabled == false)
                {
                    truckRR.enabled = true;
                }
            }
        }


    }
}
