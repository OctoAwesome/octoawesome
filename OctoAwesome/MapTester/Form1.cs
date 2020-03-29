using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OctoAwesome.Noise;
namespace MapTester
{
    public partial class Form1 : Form
    {
        private float[,] map1;
        private Bitmap map1Bmp;
        private float[,] map2;
        private Bitmap map2Bmp;
        private float[,] merged;
        private Bitmap mergedBmp;
        private int seedMap1;
        Random r = new Random();

        public Form1()
        {
            InitializeComponent();
            seedMap1 = 1234;
            GenerateNoise();
            pictureBox1.Image = mergedBmp;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            seedMap1 = r.Next(0, int.MaxValue);
            GenerateNoise();
            pictureBox1.Refresh();
        }

        private void GenerateNoise()
        {
            float zoomFactor = (((trackBar1.Maximum - trackBar1.Value) / (trackBar1.Maximum - (float)trackBar1.Minimum))) * 2;
            var gen1 = new SimplexNoiseGenerator(seedMap1, 1 / 100f, 1 / 100f, 1 / 100f);
            var gen2 = new SimplexNoiseGenerator(seedMap1+2, 1 / 300f, 1 / 300f, 1 / 100f);

            int startX = 0;
            int startY = 0;
            int width = 512;
            int height = 512;
            int tileSize = 512;


            map1 = gen1.GetTileableNoiseMap2D(startX, startY, width, height, tileSize, tileSize, zoomFactor);
            map2 = gen2.GetTileableNoiseMap2D(startX, startY, width, height, tileSize, tileSize, zoomFactor);

            const int iterations = 10000;

            double averageHeight = 0;

            Random r = new Random(0);

            for (int i = 0; i < iterations; i++)
            {
                averageHeight += gen2.GetTileableNoise2D(r.Next(width), r.Next(height), tileSize, tileSize);
            }

            averageHeight /= iterations;

            float avg = (float)averageHeight;
            Debug.WriteLine(avg);

            if (map1Bmp == default)
                map1Bmp = new Bitmap(width, height);
            if (map2Bmp == default)
                map2Bmp = new Bitmap(width, height);
            if (mergedBmp == default)
                mergedBmp = new Bitmap(width, height);

            merged = new float[width, height];
            for (int i7 = 0; i7 < width; i7++)
                for (int i2 = 0; i2 < height; i2++)
                {
                    var m2a = map2[i7, i2];
                    var m1b = map1[i7, i2];
                    var m2 = 0.5f * (1 + m2a);
                    var m1 = m1b;
                    var m3 = m2a + m1b * (1 - m2);

                    var m4 = avg + m3 * (m3 < 0 ? 1 + avg : 1 - avg);

                    merged[i7, i2] = Math.Max(0, m4);
                }
            FillBitMap(width, height, startX, startY, map1Bmp, map1);
            FillBitMap(width, height, startX, startY, map2Bmp, map2);
            FillBitMap(width, height, startX, startY, mergedBmp, merged);
        }

        private void FillBitMap(int width, int height, int startX, int startY, Bitmap bmp1, float[,] map1)
        {
            for (int x = startX; x < width; x += 1)
                for (int y = startY; y < height; y += 1)
                {
                    var col = (int)(255 * Math.Max(0, (map1[x, y])));
                    bmp1.SetPixel(x, y, Color.FromArgb(255, col, col, col));
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = map1Bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = map2Bmp;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = mergedBmp;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            GenerateNoise();
            pictureBox1.Refresh();
        }
    }
}
