using engenious;
using engenious.Graphics;
using engenious.Input;
using MonoGameUi;
using OctoAwesome.Basics.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Controls
{
    class ToolbarInventoryControl : Panel
    {
        private Dictionary<string, Texture2D> toolTextures = new Dictionary<string, Texture2D>();

        private Image[] images;

        private Brush backgroundBrush;

        private Brush hoverBrush;

        private ToolBarComponent bar;

        public ToolbarInventoryControl(BaseScreenComponent screenmanager, IUserInterfaceExtensionManager manager,
            ToolBarComponent bar) : base(screenmanager)
        {
            Texture2D panelBackground = manager.LoadTextures(manager.GetType(), "panel");
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);
            Margin = new Border(0, 0, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            this.bar = bar;
            backgroundBrush = new BorderBrush(Color.Black);
            hoverBrush = new BorderBrush(Color.Brown);

            foreach (var item in bar.Service.DefinitionManager.GetDefinitions())
            {
                Texture2D texture = manager.LoadTextures(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            Grid toolbar = new Grid(ScreenManager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Controls.Add(toolbar);

            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            
            for (int i = 0; i < bar.Tools.Length; i++)
                toolbar.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            images = new Image[bar.Tools.Length];
            for (int i = 0; i < bar.Tools.Length; i++)
            {
                Image image = images[i] = new Image(screenmanager)
                {
                    Width = 42,
                    Height = 42,
                    Background = backgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = i,
                    Padding = Border.All(2),
                };
                
                image.StartDrag += (e) =>
                {
                    InventorySlot slot = bar.Tools[(int) image.Tag];
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = toolTextures[slot.Definition.GetType().FullName];
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (e) => { image.Background = hoverBrush; };
                image.DropLeave += (e) => { image.Background = backgroundBrush; };
                image.EndDrop += (e) =>
                {
                    e.Handled = true;

                    //TODO: wieder hinzufügen, oder anders lösen
                    if (e.Sender is Grid) // && ShiftPressed
                    {
                        // Swap
                        int targetIndex = (int) image.Tag;
                        InventorySlot targetSlot = bar.Tools[targetIndex];

                        InventorySlot sourceSlot = e.Content as InventorySlot;
                        int sourceIndex = bar.GetSlotIndex(sourceSlot);

                        bar.SetTool(sourceSlot, targetIndex);
                        bar.SetTool(targetSlot, sourceIndex);
                    }
                    else
                    {
                        // Inventory Drop
                        InventorySlot slot = e.Content as InventorySlot;
                        bar.SetTool(slot, (int) image.Tag);
                    }
                };

                toolbar.AddControl(image, 0, i);
            }
        }
        protected override void OnUpdate(GameTime gameTime)
        {
            //Aktualisierung des aktiven Buttons
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                if (bar.Tools != null &&
                    bar.Tools.Length > i &&
                    bar.Tools[i] != null &&
                    bar.Tools[i].Definition != null)
                {
                    images[i].Texture = toolTextures[bar.Tools[i].Definition.GetType().FullName];
                }
                else
                {
                    images[i].Texture = null;
                }
            }
        }
        protected override void OnEndDrop(DragEventArgs args)
        {
            if (args.Content is InventorySlot slot)
            {
                args.Handled = true;
                bar.RemoveSlot(slot);
            }
        }
        protected override void OnKeyDown(KeyEventArgs args)
        {
            // TODO: lösung finden
            //Tool neu zuweisen
            //if ((int) args.Key >= (int) Keys.D0 && (int) args.Key <= (int) Keys.D9)
            //{
            //    int offset = (int) args.Key - (int) Keys.D0;
            //    player.Toolbar.SetTool(inventory.HoveredSlot, offset);
            //    args.Handled = true;
            //}

        }
        public override string ToString()
        {
            // TODO: resx
            return "Werkzeuge";
        }
    }
}
