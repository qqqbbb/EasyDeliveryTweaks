using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyDeliveryTweaks
{
    internal class Config
    {
        public static ConfigEntry<bool> fixLowResRender;
        public static ConfigEntry<bool> fixTruckTurn;
        public static ConfigEntry<bool> vertSync;

        public static void Bind()
        {
            fixLowResRender = Main.config.Bind("", "Render the world in resolution set in options", false);
            fixTruckTurn = Main.config.Bind("", "Smooth truck turning", false);
            vertSync = Main.config.Bind("", "Screen vertical syncronization", true);
            vertSync.SettingChanged += vertSyncChanged;
        }

        private static void vertSyncChanged(object sender, EventArgs e)
        {
            if (vertSync.Value)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            //Main.logger.LogDebug("vertSyncChanged " + QualitySettings.vSyncCount);
        }

    }
}
