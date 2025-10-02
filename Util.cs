using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

namespace EasyDeliveryTweaks
{
    internal class Util
    {
        public static void DisplayText(string text, float displayTime = 1)
        {
            if (Main.hud == null)
            {
                Main.logger.LogError("Main.hud == null");
                return;
            }
            Main.hud.DisplayText(text, displayTime);
        }

        public static bool IsInMainMenu()
        {
            return SceneManager.GetActiveScene().name == "TitleScreen";
        }

        public static bool IsLoadingScene()
        {
            return SceneTransition.loadingScene;
        }


    }
}
