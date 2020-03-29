using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Noise
{
    public interface INoise
    {
        /// <summary>
        /// Gibt den Seed des Noisegenerators zurück
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Gibt ein float-Array einer 1D-Noise im angegebenem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition, ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Anzahl der gewollten Noise-Werte</param>
        /// <returns>Gibt ein float-Array einer 1D Noise zurück</returns>
        float[] GetNoiseMap(int startX, int width);
        /// <summary>
        /// Gibt ein 2D-float-Array einer 2D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <returns>Gibt ein 2D-float-Array einer 2D-Noise zurück</returns>
        float[,] GetNoiseMap2D(int startX, int startY, int width, int height);
        /// <summary>
        /// Gibt ein 2D-float-Array einer 2D-Noise im angegebem Bereich zurück, welche kachelbar ist
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein 2D-float-Array einer 2D-Noise zurück, welche kachelbar ist</returns>
        float[,] GetTileableNoiseMap2D(int startX, int startY, int width, int height, int tileSizeX, int tileSizeY, float donutSth = 2f);

        /// <summary>
        /// Gibt ein 3D-float-Array einer 3D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <returns>Gibt ein 3D-float-Array einer 3D-Noise zurück</returns>
        float[, ,] GetNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth);
        /// <summary>
        /// Gibt ein 3D-float-Array einer 3D-Noise im angegebem Bereich zurück, welche in X und Y Richtung kachelbar ist
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein 3D-float-Array einer 3D-Noise zurück, welche in X und Y Richtung kachelbar ist</returns>
        float[, ,] GetTileableNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gibt ein 4D-float-Array einer 4D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startW">Startposition auf der W-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <param name="thickness">Dicke(Tiefe 2.Grades) der Noise-Map</param>
        /// <returns>Gibt ein 4D-float-Array einer 4D-Noise zurück</returns>
        float[, , ,] GetNoiseMap4D(int startX, int startY, int startZ, int startW, int width, int height, int depth, int thickness);

        /// <summary>
        /// Gibt ein float-Wert einer 1D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 1D Noise zurück</returns>
        float GetNoise(int x);

        /// <summary>
        /// Gibt ein float-Wert einer 2D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 2D Noise zurück</returns>
        float GetNoise2D(int x, int y);
        /// <summary>
        /// Gibt ein float-Wert einer 2D-Noise an gegebener Position zurück, welche kachelbar ist
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein float-Wert einer 2D Noise zurück, welche kachelbar ist</returns>
        float GetTileableNoise2D(int x, int y, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gibt ein float-Wert einer 3D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 3D Noise zurück</returns>
        float GetNoise3D(int x, int y, int z);
        /// <summary>
        /// Gibt ein float-Wert einer 3D-Noise an gegebener Position zurück, welche in X und Y Richtung kachelbar ist
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein float-Wert einer 3D Noise zurück, welche in X und Y Richtung kachelbar ist</returns>
        float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gibt ein float-Wert einer 4D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="w">W-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 4D Noise zurück</returns>
        float GetNoise4D(int x, int y, int z, int w);
    }
}
