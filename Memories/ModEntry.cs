using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace Memories
{
    public class ModEntry : Mod
    {
        int trigger = 0;

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {

            Helper.Events.GameLoop.DayStarted += this.TimeEvents_AfterDayStarted;

            /*
            // Change Marnie's purchase stock
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;*/
        }

        /*********
        ** Private methods
        *********/
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /*this.Monitor.Log("Welcome to Pokémod!");
            Game1.activeClickableMenu = (IClickableMenu)new MyPurchaseAnimalsMenu(getMyAnimalStock(), this.Helper);*/

            this.Monitor.Log("Events seen:");
            foreach (int i in Game1.player.eventsSeen)
            {
                this.Monitor.Log(i.ToString());
            }
            
            Game1.activeClickableMenu = (IClickableMenu)new MemoriesMenu(Game1.player.eventsSeen, this.Monitor);
        }

        // Change Marnie's purchase stock
        public void MenuEvents_MenuChanged(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is PurchaseAnimalsMenu animalsMenu)
            {
                this.Monitor.Log("Changing Marnie's stock.");
                //Game1.activeClickableMenu = (IClickableMenu)new MyPurchaseAnimalsMenu(getMyAnimalStock(), this.Helper);
                
                // TODO für neue animals -> PurchaseAnimalsMenu überschreiben, sodass receiveLeftClick statt
                // this.animalBeingPurchased = new FarmAnimal(textureComponent.hoverText, Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
                // ... new MyFarmAnimal aufruft (und eigene Strings in getAnimalDescription  / getAnimaTitle aufruft)
                // -> MyFarmAnimal als Kopie von FarmAnimal schreiben, sodass im Konstruktor und weiteren Verlauf eigene animals eingebaut werden können
            }
        }

    }
}
