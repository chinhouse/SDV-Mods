using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using Netcode;
using System.Collections.Generic;
using StardewValley.Menus;

namespace AutoPetMassager
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // print button presses to the console window

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (!e.Button.ToString().Equals("OemPeriod"))
                return;

            Farmer who = Game1.player;

            int petCnt = 0;
            foreach (FarmAnimal farmAn in Game1.getFarm().getAllFarmAnimals())
            {

                if (!(bool)farmAn.wasPet)
                {
                    farmAn.wasPet.Value = true;
                    farmAn.friendshipTowardFarmer.Value = Math.Min(1000, (int)farmAn.friendshipTowardFarmer + 15);
                    if (who.professions.Contains(3) && !farmAn.isCoopDweller() || who.professions.Contains(2) && farmAn.isCoopDweller())
                    {
                        farmAn.friendshipTowardFarmer.Value = Math.Min(1000, (int)(NetFieldBase<int, NetInt>)farmAn.friendshipTowardFarmer + 15);
                        farmAn.happiness.Value = (byte)Math.Min((int)byte.MaxValue, (int)(byte)(NetFieldBase<byte, NetByte>)farmAn.happiness + Math.Max(5, 40 - (int)(byte)(NetFieldBase<byte, NetByte>)farmAn.happinessDrain));
                    }
                    farmAn.doEmote((int)(NetFieldBase<int, NetInt>)farmAn.moodMessage == 4 ? 12 : 20, true);
                    farmAn.happiness.Value = (byte)Math.Min((int)byte.MaxValue, (int)(byte)(NetFieldBase<byte, NetByte>)farmAn.happiness + Math.Max(5, 40 - (int)(byte)(NetFieldBase<byte, NetByte>)farmAn.happinessDrain));
                    farmAn.makeSound();
                    who.gainExperience(0, 5);

                    if (!(!farmAn.type.Value.Equals("Sheep") || (int)(NetFieldBase<int, NetInt>)farmAn.friendshipTowardFarmer < 900))
                        farmAn.daysToLay.Value = (byte)2;

                    petCnt++;
                }
            }

            if (petCnt > 0)
            {
                HUDMessage pettedMsg = new HUDMessage(petCnt.ToString() + " animal(s) were petted.", "false");
                Game1.addHUDMessage(pettedMsg);
            }
        }
    }
}
