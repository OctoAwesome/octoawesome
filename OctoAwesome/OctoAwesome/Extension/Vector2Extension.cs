using engenious;
using OctoAwesome.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension
{
    public static class Vector2Extension
    {
        /// <summary>
        /// Normalizes a given integer to a given maximum value and prevents negative numbers.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <param name="size">The maximum value to normalize to.</param>
        /// <returns>The normalized result.</returns>
        public static float NormalizeAxis(float value, float size)
        {
#if DEBUG
            if (size < 1)
                throw new ArgumentException("Size darf nicht kleiner als 1 sein");
#endif
            value %= size;

            if (value < 0)
                value += size;

            return value;
        }


        public static void NormalizeXY(this Vector3 source, Vector3 size) => NormalizeXY(source, new Vector2(size.X, size.Y));

        public static void NormalizeXY(this Vector3 source, Vector2 size)
        {
            source.X = NormalizeAxis(source.X, size.X);
            source.Y = NormalizeAxis(source.Y, size.Y);
        }


        /// <summary>
        /// Calculates the shortest distance from a source to a destination point on a normalized axis.
        /// </summary>
        /// <param name="source">The source point to calculate the distance from.</param>
        /// <param name="destination">The destination point to calculate the distance to.</param>
        /// <param name="size">The maximum axis value to normalize to.</param>
        /// <returns>The shortest distance on the axis.</returns>
        /// <remarks>
        /// This can be negative and source + distance can even be below zero, to describe the shortest distance by a wraparound.
        /// </remarks>
        public static float ShortestDistanceOnAxis(float source, float destination, float size)
        {
            source = NormalizeAxis(source, size);
            destination = NormalizeAxis(destination, size);

            float half = size / 2;
            float distance = destination - source;

            if (distance > half)
                distance -= size;
            else if (distance < -half)
                distance += size;

            return distance;
        }

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to.</param>
        /// <param name="size">The maximum size to componentwise normalize the x and y axis to.</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>

        public static Vector2 ShortestDistanceXY(this Vector2 source, Vector2 destination, Vector2 size)
    => new Vector2(
        ShortestDistanceOnAxis(source.X, destination.X, size.X),
        ShortestDistanceOnAxis(source.Y, destination.Y, size.Y));
        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to.</param>
        /// <param name="size">The maximum size to componentwise normalize the x and y axis to.</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>

        public static Vector3 ShortestDistanceXY(this Vector3 source, Vector3 destination, Vector3 size)
    => new Vector3(
        ShortestDistanceOnAxis(source.X, destination.X, size.X),
        ShortestDistanceOnAxis(source.Y, destination.Y, size.Y),
        destination.Z - source.Z);
    }
}
