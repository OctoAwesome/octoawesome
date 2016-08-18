using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Controls
{
    internal sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;

        /// <summary>
        /// Gibt den aktuell selektierten Slot an.
        /// </summary>
        public InventorySlot Hovered { get; private set; }

        public InventoryControl(ScreenComponent manager) : base(manager)
        {
            Label headLine = new Label(manager);
            headLine.Text = Languages.OctoClient.Inventory;
            headLine.Font = Skin.Current.HeadlineFont;
            headLine.HorizontalAlignment = HorizontalAlignment.Left;
            headLine.VerticalAlignment = VerticalAlignment.Top;
            Controls.Add(headLine);

            Label infoLabel = new Label(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Controls.Add(infoLabel);

            ScrollContainer scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 50, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Controls.Add(scroll);

            Grid grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            for (int i = 0; i < COLUMNS; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            int rows = (int)System.Math.Ceiling((float)manager.Game.Player.ActorHost.Player.Inventory.Count / COLUMNS);
            for (int i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            int column = 0;
            int row = 0;
            foreach (var item in manager.Game.Player.ActorHost.Player.Inventory)
            {
                Texture2D texture;
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Bitmap bitmap = item.Definition.Icon;
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    texture = Texture2D.FromStream(ScreenManager.GraphicsDevice, stream);
                }

                var image = new Image(manager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                image.MouseEnter += (s, e) => { Hovered = item; infoLabel.Text = item.Definition.Name; };
                image.MouseLeave += (s, e) => { Hovered = null; infoLabel.Text = string.Empty; };
                var label = new Label(manager) { Text = item.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
                grid.AddControl(image, column, row);
                grid.AddControl(label, column, row);

                column++;
                if (column >= COLUMNS)
                {
                    row++;
                    column = 0;
                }
            }

            scroll.Content = grid;

            
        }
    }
}
