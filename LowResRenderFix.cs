using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class LowResRenderFix
    {
        const float defaultRenderWidth = 456f;
        const float defaultRenderHeight = 256f;
        static RenderTexture newRT;
        static RenderTexture defaultRT;
        static RenderTexture newRearVeiwRT;
        static RenderTexture defaultRearVeiwRT;
        static MeshRenderer screenMR;
        static MeshRenderer rearVeiwMR;
        static Camera mainCamera;
        static Camera rearVeiwCamera;
        static float resWidthRatio;
        static float resHeightRatio;
        static Resolution currentRes;

        [HarmonyPatch(typeof(sCameraController))]
        public class CameraPatch
        {
            [HarmonyPostfix, HarmonyPatch("Awake")]
            static void AwakePostfix(sCameraController __instance)
            {
                mainCamera = __instance.cam;
                //Main.logger.LogMessage(" targetFrameRate " + Application.targetFrameRate);
                //Main.logger.LogMessage("sCameraController Awake ");
            }
        }

        static void CreateNewRenderTexture(Camera cam)
        { // 456 256
            if (cam.targetTexture == null)
                return;

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
            //Main.logger.LogMessage($"CreateNewRenderTexture {newWidth} {newHeight}");
        }

        [HarmonyPatch(typeof(ScreenSystem))]
        public class ScreenSystemPatch
        {
            [HarmonyPostfix, HarmonyPatch("Awake")]
            static void AwakePostfix(ScreenSystem __instance)
            {
                Transform t = __instance.transform.Find("Camera Persp/ScreenPivot/Screen");
                if (t != null)
                    screenMR = t.GetComponent<MeshRenderer>();
            }
        }

        public static void FixLowResRender()
        {
            //if (newRT == null || newRearVeiwRT == null || screenMR == null || rearVeiwMR == null)
            //    return;
            //Main.logger.LogMessage($"FixLowResRender");
            currentRes = Screen.currentResolution;
            resWidthRatio = currentRes.width / defaultRenderWidth;
            resHeightRatio = currentRes.height / defaultRenderHeight;
            CreateNewRenderTexture(mainCamera);
            CreateNewRearViewRenderTexture(rearVeiwCamera);

            if (Config.fixLowResRender.Value)
            {
                screenMR.material.mainTexture = newRT;
                mainCamera.targetTexture = newRT;
                rearVeiwCamera.targetTexture = newRearVeiwRT;
                rearVeiwMR.material.mainTexture = newRearVeiwRT;
            }
            else
            {
                screenMR.material.mainTexture = defaultRT;
                mainCamera.targetTexture = defaultRT;
                rearVeiwCamera.targetTexture = defaultRearVeiwRT;
                rearVeiwMR.material.mainTexture = defaultRearVeiwRT;
            }
        }

        static void CreateNewRearViewRenderTexture(Camera cam)
        {
            if (cam.targetTexture == null)
                return;

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
            //Main.logger.LogMessage($"CreateNewRearViewRenderTexture {newWidth} {newHeight}");
        }

        [HarmonyPatch(typeof(sCarController))]
        public class sCarControllerPatch
        {
            [HarmonyPostfix, HarmonyPatch("Awake")]
            static void AwakePostfix(sCarController __instance)
            {
                currentRes = default;
                if (rearVeiwMR)
                    return;

                Transform t = __instance.transform.Find("carInt/RearViewMirror");
                if (t)
                    rearVeiwMR = t.GetComponent<MeshRenderer>();

                t = __instance.transform.Find("RearViewCam");
                if (t)
                    rearVeiwCamera = t.GetComponent<Camera>();
            }
        }

        [HarmonyPatch(typeof(sHUD))]
        public class sHUDPatch
        {
            //[HarmonyPostfix, HarmonyPatch("Start")]
            static void StartPostfix(sHUD __instance)
            {
                //Main.logger.LogMessage($"sHUD Start");
                //FixLowResRender();
            }
            [HarmonyPostfix, HarmonyPatch("Update")]
            static void UpdatePostfix(sHUD __instance)
            { // runs after saved game loaded
                if (currentRes.Equals(Screen.currentResolution) == false)
                    FixLowResRender();
            }
            [HarmonyPostfix, HarmonyPatch("WorldToHUDPoint")]
            static void WorldToHUDPointPostfix(sHUD __instance, ref Vector2 __result, Vector3 worldPoint)
            {
                if (Config.fixLowResRender.Value == false || resWidthRatio == 0 || __result.x == -100f)
                    return;

                //Main.logger.LogMessage($"WorldToHUDPoint worldPoint {worldPoint} result {__result}");
                //Util.DisplayText($"result {__result}");
                //Util.DisplayText($"resRatio {resRatio}");
                __result = __instance.navigation.car.carCamera.WorldToScreenPoint(worldPoint);
                __result = new Vector2(__result.x / resWidthRatio, __result.y / resHeightRatio);
                __result.y = __instance.R.height - __result.y;
            }
        }

        //[HarmonyPatch(typeof(MainMenuHUD))]
        public class MainMenuHUDPatch
        {
            //[HarmonyPostfix, HarmonyPatch("FrameUpdate")]
            static void UpdatePostfix(MainMenuHUD __instance)
            {
                if (Input.GetKey(KeyCode.V))
                    Main.logger.LogDebug("MainMenuHUD FrameUpdate");
            }
        }

        //[HarmonyPatch(typeof(PauseSystem))]
        public class PauseSystemPatch
        {
            //[HarmonyPostfix, HarmonyPatch("Update")]
            static void UpdatePostfix(PauseSystem __instance)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Main.logger.LogDebug(" InterSceneData  ");
                    foreach (var kv in InterSceneData.instance.data)
                    {
                        Main.logger.LogDebug($"{kv.Key} ,value: {kv.Value} ");
                    }
                }
            }
            //[HarmonyPostfix, HarmonyPatch("GotoMainMenu")]
            static void GotoMainMenuPostfix(PauseSystem __instance)
            {
                currentRes = default;
                Main.logger.LogDebug("PauseSystem GotoMainMenu ");
            }
        }


    }
}
