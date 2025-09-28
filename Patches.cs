using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class Patches
    {
        static RenderTexture newRT;
        static RenderTexture defaultRT;
        static RenderTexture newRearVeiwRT;
        static RenderTexture defaultRearVeiwRT;
        static bool usingNewRenderTexture;
        static RotationRounding truckRR;
        static MeshRenderer screenMR;
        static MeshRenderer rearVeiwMR;
        static Camera mainCamera;
        static Camera rearVeiwCamera;

        [HarmonyPatch(typeof(sCameraController))]
        public class CameraPatch
        {
            [HarmonyPostfix, HarmonyPatch("Start")]
            static void StartPostfix(sCameraController __instance)
            {
                if (newRT == null)
                    CreateNewRenderTexture(__instance.cam);
            }

            static IEnumerator FixResolutionNextFrame(Camera cam)
            {
                yield return null; // Wait one frame
                CreateNewRenderTexture(cam);
            }

            static void CreateNewRenderTexture(Camera cam)
            {
                if (cam.targetTexture == null)
                    return;

                mainCamera = cam;
                defaultRT = cam.targetTexture;
                int newWidth = Screen.currentResolution.width;
                int newHeight = Screen.currentResolution.height;

                if (defaultRT.width == newWidth && defaultRT.height == newHeight)
                {
                    newRT = defaultRT;
                    return;
                }
                newRT = new RenderTexture(newWidth, newHeight, defaultRT.depth);
                newRT.format = defaultRT.format;
                newRT.antiAliasing = defaultRT.antiAliasing;
                newRT.filterMode = defaultRT.filterMode;
                newRT.Create();
            }

            //[HarmonyPatch(typeof(setSkyColor))]
            public class setSkyColorPatch
            {
                [HarmonyPostfix, HarmonyPatch("Start")]
                static void StartPostfix(setSkyColor __instance)
                {
                    //Camera camera = __instance.GetComponent<Camera>();
                    Main.logger.LogInfo("setSkyColor Start " + __instance.name);
                    if (__instance.name == "RearViewCam")
                    {
                        if (__instance.cam.targetTexture != null)
                        {
                            Main.logger.LogInfo("setSkyColor Start targetTexture " + __instance.cam.targetTexture.name);
                            __instance.cam.targetTexture = null;
                        }
                        else
                            Main.logger.LogInfo("setSkyColor Start targetTexture null");
                    }
                }
            }

        }

        [HarmonyPatch(typeof(ScreenSystem))]
        public class ScreenSystemPatch
        {
            [HarmonyPostfix, HarmonyPatch("Update")]
            static void InitializePostfix(ScreenSystem __instance)
            {
                if (screenMR == null)
                {
                    Transform t = __instance.transform.Find("Camera Persp/ScreenPivot/Screen");
                    if (t != null)
                        screenMR = t.GetComponent<MeshRenderer>();
                }
                FixLowResRender();
            }

            private static void FixLowResRender()
            {
                if (newRT == null || newRearVeiwRT == null || screenMR == null || rearVeiwMR == null)
                    return;

                if (Config.fixLowResRender.Value && usingNewRenderTexture == false)
                {
                    screenMR.material.mainTexture = newRT;
                    mainCamera.targetTexture = newRT;
                    rearVeiwCamera.targetTexture = newRearVeiwRT;
                    rearVeiwMR.material.mainTexture = newRearVeiwRT;
                    usingNewRenderTexture = true;
                }
                else if (Config.fixLowResRender.Value == false && usingNewRenderTexture)
                {
                    screenMR.material.mainTexture = defaultRT;
                    mainCamera.targetTexture = defaultRT;
                    rearVeiwCamera.targetTexture = defaultRearVeiwRT;
                    rearVeiwMR.material.mainTexture = defaultRearVeiwRT;
                    usingNewRenderTexture = false;
                }
            }
        }

        static void CreateNewRearViewRenderTexture(Camera cam)
        {
            if (cam.targetTexture == null)
                return;

            rearVeiwCamera = cam;
            defaultRearVeiwRT = cam.targetTexture;
            int newWidth = (int)(Screen.currentResolution.width / 4.5f);
            int newHeight = (int)(Screen.currentResolution.height / 8f);

            if (defaultRearVeiwRT.width == newWidth && defaultRearVeiwRT.height == newHeight)
            {
                newRearVeiwRT = defaultRearVeiwRT;
                return;
            }
            newRearVeiwRT = new RenderTexture(newWidth, newHeight, defaultRearVeiwRT.depth);
            newRearVeiwRT.format = defaultRearVeiwRT.format;
            newRearVeiwRT.antiAliasing = defaultRearVeiwRT.antiAliasing;
            newRearVeiwRT.filterMode = defaultRearVeiwRT.filterMode;
            newRearVeiwRT.Create();
        }

        [HarmonyPatch(typeof(sCarController))]
        public class sCarControllerPatch
        {
            [HarmonyPostfix, HarmonyPatch("Update")]
            static void UpdatePostfix(sCarController __instance)
            {
                if (rearVeiwMR == null)
                {
                    Transform t = __instance.transform.Find("carInt/RearViewMirror");
                    if (t != null)
                        rearVeiwMR = t.GetComponent<MeshRenderer>();
                }
                if (truckRR == null)
                {
                    Transform t = __instance.transform.Find("Model");
                    if (t != null)
                        truckRR = t.GetComponent<RotationRounding>();
                }
                if (rearVeiwCamera == null)
                {
                    Transform t = __instance.transform.Find("RearViewCam");
                    if (t)
                        rearVeiwCamera = t.GetComponent<Camera>();

                    CreateNewRearViewRenderTexture(rearVeiwCamera);
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
