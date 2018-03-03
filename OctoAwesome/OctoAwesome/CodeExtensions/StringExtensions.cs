using System;

namespace OctoAwesome.CodeExtensions
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Split a string.
        /// </summary>
        /// <param name="input">String to split</param>
        /// <param name="options">Options of the Split operation</param>
        /// <param name="separator">Separators</param>
        /// <returns></returns>
        public static string[] Split(this string input, StringSplitOptions options, params char[] separator)
        {
            return input.Split(separator, options);
        }
        /// <summary>
        /// Checks if the string is NUll or Empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }
    }
}
