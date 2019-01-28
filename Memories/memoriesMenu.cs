using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace Memories
{
    class MemoriesMenu : IClickableMenu
    {
        public static int menuHeight = 320;
        public static int menuWidth = 448;
        public List<ClickableTextureComponent> memoryToPlay = new List<ClickableTextureComponent>();
        public const int region_okButton = 101;
        public ClickableTextureComponent okButton;
        public ClickableTextureComponent hovered;

        private IMonitor monitor;
        private NetIntList eventsSeen;

        public MemoriesMenu(NetIntList eventsSeen, IMonitor monitor) : base(Game1.viewport.Width / 2 - MemoriesMenu.menuWidth / 2 - IClickableMenu.borderWidth * 2, Game1.viewport.Height / 2 - MemoriesMenu.menuHeight - IClickableMenu.borderWidth * 2, MemoriesMenu.menuWidth + IClickableMenu.borderWidth * 2, MemoriesMenu.menuHeight + IClickableMenu.borderWidth, false)
        {
            this.monitor = monitor;
            this.eventsSeen = eventsSeen;

            monitor.Log("Creating memories menu ...");

            // Iterate through events seen and create the photo album
            for (int index = 0; index < eventsSeen.Count; ++index)
            {
                List<ClickableTextureComponent> memoryToPlay = this.memoryToPlay;

                ClickableTextureComponent textureComponent = new ClickableTextureComponent(index.ToString(), // name
                    // bounds
                    new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + index % 3 * 64 * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + index / 3 * 85, 128, 64),
                    (string)null,
                    // Hover text
                    "Memory " + index.ToString(),
                    // Texture file to load
                    Game1.mouseCursors,
                    // Source rectangle of the texture file to load
                    // TODO: Replace this with my own asset file containing photo previews
                    new Microsoft.Xna.Framework.Rectangle(index % 3 * 16 * 2, 448 + index / 3 * 16, 32, 16),
                    4f,
                    false);

                textureComponent.myID = index;
                // alternate ID = event ID
                textureComponent.myAlternateID = eventsSeen.ElementAt(index);
                textureComponent.rightNeighborID = index % 3 == 2 ? -1 : index + 1;
                textureComponent.leftNeighborID = index % 3 == 0 ? -1 : index - 1;
                textureComponent.downNeighborID = index + 3;
                textureComponent.upNeighborID = index - 3;
                memoryToPlay.Add(textureComponent);

                monitor.Log("Created memory texture component with ID " + textureComponent.myID.ToString()+" and event ID "+textureComponent.myAlternateID.ToString());
            }

            // OK Button
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            textureComponent1.myID = 101;
            textureComponent1.upNeighborID = 103;
            textureComponent1.leftNeighborID = 103;
            this.okButton = textureComponent1;

            if (!Game1.options.SnappyMenus)
                return;

            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;

            // Close when player clicks on OK button
            if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }

            foreach(ClickableTextureComponent textureComponent in this.memoryToPlay)
            {
                if(textureComponent.containsPoint(x, y))
                {
                    monitor.Log("Player clicked memory with index " + textureComponent.myID + " and event ID " + textureComponent.myAlternateID);

                    /*
                     * 
                     * -> Read out event data and play event
                     * 
                    e.NewLocation.currentEvent = new Event("WizardSong/0 5/Wizard 8 5 0 farmer 8 15 0/move farmer 0 -8 0/speak Wizard \"TODO#$b#Find one of the five altars and learn some magic.#$b#Q to start casting, then 1-4 to choose the spell.#$b#TAB to switch between spell sets.\"/textAboveHead Wizard \"MAGIC\"/pause 750/fade 750/end", 90000);
                    Game1.eventUp = true;
                    Game1.displayHUD = false;
                    Game1.player.CanMove = false;
                    Game1.player.showNotCarrying();

                    Game1.player.addMagicExp(Game1.player.getMagicExpForNextLevel());
                    Game1.player.addMana(Game1.player.getMaxMana());
                    Game1.player.eventsSeen.Add(90000);
                    */

                    // or simply Game1.player.currentLocation.startEvent ? .... yes, probably
                
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            this.hovered = (ClickableTextureComponent)null;

            if (Game1.globalFade)
                return;

            if(this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                    this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
                else
                    this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
            }

            foreach(ClickableTextureComponent textureComponent in this.memoryToPlay)
            {
                if(textureComponent.containsPoint(x, y))
                {
                    textureComponent.scale = Math.Min(textureComponent.scale + 0.05f, 4.1f);
                    this.hovered = textureComponent;
                }
                else
                    textureComponent.scale = Math.Max(4f, textureComponent.scale - 0.025f);            
            }
        }

        public override void draw(SpriteBatch b)
        {
            if(!Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                foreach (ClickableTextureComponent textureComponent in this.memoryToPlay)
                    textureComponent.draw(b); //, (textureComponent.item as StardewValley.Object).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);                
            }

            if (!Game1.globalFade && this.okButton != null)
                this.okButton.draw(b);

            if(this.hovered != null)
            {
                IClickableMenu.drawHoverText(b, hovered.myAlternateID.ToString(), Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1, (CraftingRecipe)null);
            }

            Game1.mouseCursorTransparency = 1f;
            this.drawMouse(b);
        }
    }
}
