﻿using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics
{
    public class ComplexPlanetGenerator : IMapGenerator
    {

        public IUniverse GenerateUniverse(int id)
        {
            return new Universe(id, "Milchstrasse");
        }

        public IPlanet GeneratePlanet(int universe, int seed)
        {
            Index3 size = new Index3(5000, 5000, 5);

            ComplexPlanet planet = new ComplexPlanet(0, universe, size, this, seed);

            return planet;
        }

        public IChunk[] GenerateChunk(IPlanet planet, Index2 index)
        {

            if (!(planet is ComplexPlanet))
                throw new ArgumentException("planet is not a Type of ComplexPlanet");

            ComplexPlanet localPlanet = (ComplexPlanet)planet;

            float[,] localHeightmap = localPlanet.BiomeGenerator.GetHeightmap(index);

            IChunk[] chunks = new IChunk[planet.Size.Z];
            for (int i = 0; i < planet.Size.Z; i++)
                chunks[i] = new Chunk(new Index3(index, i), planet.Id);

            int obersteSchicht;
            bool surfaceBlock;
            bool ozeanSurface;

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    obersteSchicht = 5;
                    surfaceBlock = true;
                    ozeanSurface = false;

                    for (int i = chunks.Length - 1; i >= 0; i--)
                    {
                        for (int z = Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
                        {
                            int absoluteZ = (z + (i * Chunk.CHUNKSIZE_Z));
                            if (absoluteZ <= localHeightmap[x, y] * localPlanet.Size.Z * Chunk.CHUNKSIZE_Z)
                            {
                                if (obersteSchicht > 0)
                                {
                                    float temp = localPlanet.ClimateMap.GetTemperature(new Index3(index.X * Chunk.CHUNKSIZE_X + x, index.Y * Chunk.CHUNKSIZE_Y + y, i * Chunk.CHUNKSIZE_Z + z));

                                    if ((ozeanSurface || surfaceBlock) && (absoluteZ <= (localPlanet.BiomeGenerator.SeaLevel + 2)) && (absoluteZ >= (localPlanet.BiomeGenerator.SeaLevel - 2)))
                                    {
                                        chunks[i].SetBlock(x, y, z, new SandBlock());
                                    }
                                    else if (temp >= 35)
                                    {
                                        chunks[i].SetBlock(x, y, z, new SandBlock());
                                    }
                                    else if (absoluteZ >= localPlanet.Size.Z * Chunk.CHUNKSIZE_Z * 0.6f)
                                    {
                                        if (temp > 12)
                                            chunks[i].SetBlock(x, y, z, new GroundBlock());
                                        else
                                            chunks[i].SetBlock(x, y, z, new StoneBlock());
                                    }
                                    else if (temp >= 8)
                                    {
                                        if (surfaceBlock && !ozeanSurface)
                                        {
                                            chunks[i].SetBlock(x, y, z, new GrassBlock());
                                            surfaceBlock = false;
                                        }
                                        else
                                        {
                                            chunks[i].SetBlock(x, y, z, new GroundBlock());
                                        }
                                    }
                                    else
                                    {
                                        chunks[i].SetBlock(x, y, z, new GroundBlock());
                                    }
                                    obersteSchicht--;
                                }
                                else
                                {
                                    chunks[i].SetBlock(x, y, z, new StoneBlock());
                                }


                            }
                            else if ((z + (i * Chunk.CHUNKSIZE_Z)) <= localPlanet.BiomeGenerator.SeaLevel)
                            {

                                chunks[i].SetBlock(x, y, z, new WaterBlock());
                                ozeanSurface = true;
                            }

                        }
                    }
                }
            }

            //float[,] cloudmap = null;
            ////Biomes.BiomeBlockValue[, ,] blockValues = localPlanet.BiomeGenerator.GetBlockValues(index,heightmap,0f,1f);

            ////for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            //Parallel.For(0, Chunk.CHUNKSIZE_X, x =>
            //{
            //    for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            //    {
            //        bool grass = true, sand = false;

            //        for (int i = chunks.Length - 1; i >= 0; i--)
            //        {

            //            for (int z = Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
            //            {
            //                //Biomes.BiomeBlockValue blockValue = blockValues[x, y, z + i * Chunk.CHUNKSIZE_Z];
            //                int blockHeight = Math.Max(z + Chunk.CHUNKSIZE_Z * (i), 0);
            //                //float density = heightmap[x,y] * (Chunk.CHUNKSIZE_Z * (planet.Size.Z)) - blockHeight;
            //                Index3 blockIndex = new Index3(index.X * Chunk.CHUNKSIZE_X + x, index.Y * Chunk.CHUNKSIZE_Y + y, i * Chunk.CHUNKSIZE_Z + z);


            //                if (blockValue.Density > 0.6f || (z < 3 && i == 0))
            //                {
            //                    if (blockValue.IsDessert || (grass | sand) && (z + (i * Chunk.CHUNKSIZE_Z)) <= localPlanet.BiomeGenerator.SeaLevel && (z + (i * Chunk.CHUNKSIZE_Z)) >= localPlanet.BiomeGenerator.SeaLevel - 2)
            //                    {
            //                        chunks[i].SetBlock(new Index3(x, y, z), new SandBlock());
            //                        grass = false;
            //                        sand = true;
            //                    }
            //                    else if (grass && planet.ClimateMap.GetTemperature(blockIndex) > 18.0f)
            //                    {
            //                        chunks[i].SetBlock(new Index3(x, y, z), new GrassBlock());
            //                        grass = false;
            //                    }
            //                    //else if (z < Chunk.CHUNKSIZE_Z - 1 && noiseplus >= resDensity)
            //                    //{
            //                    //    chunks[i].SetBlock(new Index3(x, y, z), new StoneBlock());
            //                    //}
            //                    else
            //                    {
            //                        chunks[i].SetBlock(new Index3(x, y, z), new GroundBlock());
            //                        grass = false;
            //                    }

            //                }
            //                else if ((z + (i * Chunk.CHUNKSIZE_Z)) <= localPlanet.BiomeGenerator.SeaLevel)
            //                {
            //                    grass = false;
            //                    sand = true;
            //                    chunks[i].SetBlock(new Index3(x, y, z), new WaterBlock());
            //                }
            //            }
            //        }
            //    }
            //});


            return chunks;
        }
    }
}
