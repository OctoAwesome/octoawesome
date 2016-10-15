﻿using System.Drawing;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für eine Blockdefinition
    /// </summary>
    public interface IBlockDefinition : IItemDefinition
    {
        /// <summary>
        /// Geplante Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="block">Der Block-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        /// <summary>
        /// Array, das alle Texturen für alle Seiten des Blocks enthält
        /// </summary>
        string[] Textures { get; }

        /// <summary>
        /// Zeigt, ob der Block-Typ Metadaten besitzt
        /// </summary>
        bool HasMetaData { get; }

        /// <summary>
        /// Liefert die Kollisionsbox für den Block. Da ein Array zurück gegeben wird, lässt sich die 
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Ein Array von Kollisionsboxen</returns>
        BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z);

        /*/// <summary>
        /// Liefert die Physikalischen Paramerter, wie härte, dichte und bruchzähigkeit
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Die physikalischen Parameter</returns>
        PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);*/

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetTopTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Unterseite (Negativ Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Nordseite (Positiv Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Südseite (Negativ Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Westseite (Negativ X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Ostseite (Positiv X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Unterseite (Negativ Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Ostseite (Positiv X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Westseite (Negativ X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Nordseite (Positiv Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Südseite (Negativ Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Oberseite (Positiv Z) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsTopSolidWall(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Unterseite (Negativ Z) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsBottomSolidWall(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Nordseite (Positiv Y) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsNorthSolidWall(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Südseite (Negativ Y) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsSouthSolidWall(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Westseite (Positiv X) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsWestSolidWall(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gibt an, ob die Ostseite (Negativ X) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        bool IsEastSolidWall(ILocalChunkCache manager, int x, int y, int z);
    }
}
