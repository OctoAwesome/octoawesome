using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class RenderControl : UserControl
    {
        public Game Game { get; set; }

        private Image grass;

        public RenderControl()
        {
            InitializeComponent();

            grass = Image.FromFile("Assets/grass.jpg");
        }

        protected override void OnResize(EventArgs e)
        {
            if (Game != null)
            {
                Game.PlaygroundSize = new Point(ClientSize.Width, ClientSize.Height);
            }
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.CornflowerBlue);

            e.Graphics.DrawImage(grass, new Rectangle(0, 0,ClientSize.Width, ClientSize.Height));

            if (Game == null)
                return;

            using (Brush brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillEllipse(brush, new Rectangle(Game.Position.X, Game.Position.Y, 100, 100));
            }
        }
    }
}
