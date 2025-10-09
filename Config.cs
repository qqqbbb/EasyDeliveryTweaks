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
        public static ConfigEntry<bool> warnings;
        public static ConfigEntry<float> coldModifier;
        public static ConfigEntry<float> energyModifier;
        public static ConfigEntry<float> fuelModifier;

        public static AcceptableValueRange<float> energyModifierRange = new AcceptableValueRange<float>(0f, 2f);

        public static void Bind()
        {
            fixLowResRender = Main.config.Bind("", "Render the world in resolution set in options", false, "Restart the game if after toggling this the picture looks too wide");
            fixTruckTurn = Main.config.Bind("", "Smooth truck turning", false);
            warnings = Main.config.Bind("", "Show warnings", true);
            vertSync = Main.config.Bind("", "Screen vertical synchronization", true);
            coldModifier = Main.config.Bind("", "Body temperature loss modifier", 1f, new ConfigDescription("Amount of body temperature you lose when outside will be multiplied by this", energyModifierRange));
            energyModifier = Main.config.Bind("", "Energy loss modifier", 1f, new ConfigDescription("Energy amount you lose will be multiplied by this", energyModifierRange));
            fuelModifier = Main.config.Bind("", "Fuel consumption modifier", 1f, new ConfigDescription("Fuel consumed by your truck will be multiplied by this", energyModifierRange));
            vertSync.SettingChanged += VertSyncChanged;
            fixLowResRender.SettingChanged += FixLowResRenderChanged;
            fixTruckTurn.SettingChanged += FixTruckTurnChanged;
        }

        private static void FixLowResRenderChanged(object sender, EventArgs e)
        {
            LowResRenderFix.FixLowResRender();
        }

        private static void FixTruckTurnChanged(object sender, EventArgs e)
        {
            TruckTurnFix.FixTruckTurning();
        }

        private static void VertSyncChanged(object sender, EventArgs e)
        {
            if (vertSync.Value)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            //Main.logger.LogDebug("vertSyncChanged " + QualitySettings.vSyncCount);
        }

    }
}
