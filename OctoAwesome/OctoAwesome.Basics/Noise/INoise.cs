namespace OctoAwesome.Basics.Noise
{
    /// <summary>
    /// Interface for seeded noise generators.
    /// </summary>
    public interface INoise
    {
        /// <summary>
        /// Gets the seed value of this noise generator.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Gets a 1D noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise.</param>
        /// <param name="width">The number of noise values to sample.</param>
        /// <returns>The 1D noise values.</returns>
        float[] GetNoiseMap(int startX, int width);

        /// <summary>
        /// Gets a 2D noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise on the x-axis.</param>
        /// <param name="startY">The starting point for the noise on the y-axis.</param>
        /// <param name="width">
        /// The number of noise values to sample on the x-axis. Corresponds to the array size of the first dimension.
        /// </param>
        /// <param name="height">
        /// The number of noise values to sample on the y-axis. Corresponds to the array size of the second dimension.
        /// </param>
        /// <returns>The 2D noise values.</returns>
        float[,] GetNoiseMap2D(int startX, int startY, int width, int height);

        /// <summary>
        /// Gets a 2D tile-able noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise on the x-axis.</param>
        /// <param name="startY">The starting point for the noise on the y-axis.</param>
        /// <param name="width">
        ///     The number of noise values to sample on the x-axis. Corresponds to the array size of the first dimension.
        /// </param>
        /// <param name="height">
        ///     The number of noise values to sample on the y-axis. Corresponds to the array size of the second dimension.
        /// </param>
        /// <param name="tileSizeX">The number of values on the x-axis after which the wraparound tiling should happen.</param>
        /// <param name="tileSizeY">The number of values on the y-axis after which the wraparound tiling should happen.</param>
        /// <param name="noiseArray">The array to write the 2D noise values to.</param>
        void FillTileableNoiseMap2D(int startX, int startY, int width, int height, int tileSizeX, int tileSizeY,
            float[] noiseArray);

        /// <summary>
        /// Gets a 3D noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise on the x-axis.</param>
        /// <param name="startY">The starting point for the noise on the y-axis.</param>
        /// <param name="startZ">The starting point for the noise on the z-axis.</param>
        /// <param name="width">
        ///     The number of noise values to sample on the x-axis. Corresponds to the array size of the first dimension.
        /// </param>
        /// <param name="height">
        ///     The number of noise values to sample on the y-axis. Corresponds to the array size of the second dimension.
        /// </param>
        /// <param name="depth">
        ///     The number of noise values to sample on the z-axis. Corresponds to the array size of the third dimension.
        /// </param>
        /// <returns>The 3D noise values.</returns>
        float[,,] GetNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth);

        /// <summary>
        /// Gets a 3D tile-able noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise on the x-axis.</param>
        /// <param name="startY">The starting point for the noise on the y-axis.</param>
        /// <param name="startZ">The starting point for the noise on the z-axis.</param>
        /// <param name="width">
        ///     The number of noise values to sample on the x-axis. Corresponds to the array size of the first dimension.
        /// </param>
        /// <param name="height">
        ///     The number of noise values to sample on the y-axis. Corresponds to the array size of the second dimension.
        /// </param>
        /// <param name="depth">
        ///     The number of noise values to sample on the z-axis. Corresponds to the array size of the third dimension.
        /// </param>
        /// <param name="tileSizeX">The number of values on the x-axis after which the wraparound tiling should happen.</param>
        /// <param name="tileSizeY">The number of values on the y-axis after which the wraparound tiling should happen.</param>
        /// <returns>The 3D noise values.</returns>
        /// <remarks>Tiles on the x and y axis.</remarks>
        float[,,] GetTileableNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gets a 4D noise float array for a given range.
        /// </summary>
        /// <param name="startX">The starting point for the noise on the x-axis.</param>
        /// <param name="startY">The starting point for the noise on the y-axis.</param>
        /// <param name="startZ">The starting point for the noise on the z-axis.</param>
        /// <param name="startW">The starting point for the noise on the w-axis.</param>
        /// <param name="width">
        ///     The number of noise values to sample on the x-axis. Corresponds to the array size of the first dimension.
        /// </param>
        /// <param name="height">
        ///     The number of noise values to sample on the y-axis. Corresponds to the array size of the second dimension.
        /// </param>
        /// <param name="depth">
        ///     The number of noise values to sample on the z-axis. Corresponds to the array size of the third dimension.
        /// </param>
        /// <param name="thickness">
        ///     The number of noise values to sample on the w-axis. Corresponds to the array size of the fourth dimension.
        /// </param>
        /// <returns>The 4D noise values.</returns>
        float[,,,] GetNoiseMap4D(int startX, int startY, int startZ, int startW, int width, int height, int depth, int thickness);

        /// <summary>
        /// Gets a noise value for a given seeding value.
        /// </summary>
        /// <param name="x">Position to sample the value at.</param>
        /// <returns>The sampled noise value.</returns>
        float GetNoise(int x);

        /// <summary>
        /// Gets a noise value for a given 2D coordinate.
        /// </summary>
        /// <param name="x">X position to sample the value at.</param>
        /// <param name="y">Y position to sample the value at.</param>
        /// <returns>The sampled noise value.</returns>
        float GetNoise2D(int x, int y);

        /// <summary>
        /// Gets a tiled noise value for a given 2D coordinate.
        /// </summary>
        /// <param name="x">X position to sample the value at.</param>
        /// <param name="y">Y position to sample the value at.</param>
        /// <param name="tileSizeX">The number of values on the x-axis after which the wraparound tiling should happen.</param>
        /// <param name="tileSizeY">The number of values on the y-axis after which the wraparound tiling should happen.</param>
        /// <returns>The sampled noise value.</returns>
        float GetTileableNoise2D(int x, int y, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gets a noise value for a given 3D coordinate.
        /// </summary>
        /// <param name="x">X position to sample the value at.</param>
        /// <param name="y">Y position to sample the value at.</param>
        /// <param name="z">Z position to sample the value at.</param>
        /// <returns>The sampled noise value.</returns>
        float GetNoise3D(int x, int y, int z);

        /// <summary>
        /// Gets a tiled noise value for a given 3D coordinate.
        /// </summary>
        /// <param name="x">X position to sample the value at.</param>
        /// <param name="y">Y position to sample the value at.</param>
        /// <param name="z">Z position to sample the value at.</param>
        /// <param name="tileSizeX">The number of values on the x-axis after which the wraparound tiling should happen.</param>
        /// <param name="tileSizeY">The number of values on the y-axis after which the wraparound tiling should happen.</param>
        /// <returns>The sampled noise value.</returns>
        /// <remarks>Tiles on the x and y axis.</remarks>
        float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY);

        /// <summary>
        /// Gets a noise value for a given 4D coordinate.
        /// </summary>
        /// <param name="x">X position to sample the value at.</param>
        /// <param name="y">Y position to sample the value at.</param>
        /// <param name="z">Z position to sample the value at.</param>
        /// <param name="w">W position to sample the value at.</param>
        /// <returns>The sampled noise value.</returns>
        float GetNoise4D(int x, int y, int z, int w);
    }
}
