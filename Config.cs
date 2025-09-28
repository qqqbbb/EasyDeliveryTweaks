using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyDeliveryTweaks
{
    internal class Config
    {
        public static ConfigEntry<bool> fixLowResRender;
        public static ConfigEntry<bool> fixTruckTurn;

        public static void Bind()
        {
            fixLowResRender = Main.config.Bind("", "Render the world in resolution set in options", false);
            fixTruckTurn = Main.config.Bind("", "Smooth truck turning", false);

        }
    }
}
