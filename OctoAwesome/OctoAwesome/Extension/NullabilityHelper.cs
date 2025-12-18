using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OctoAwesome.Extension;

/// <summary>
/// Helper class for handling nullable types.
/// </summary>
public static class NullabilityHelper
{
    /// <summary>
    /// Checks a value for null and asserts if it was null, otherwise returns the non-null value.
    /// </summary>
    /// <param name="message">
    /// The message to display, or <c>null</c> to create default message using <paramref name="parameterName"/>.
    /// </param>
    /// <param name="value">The value to check.></param>
    /// <param name="parameterName">The name of the parameter to be checked(used for the assert message).</param>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <returns>The non null value.</returns>
    public static T NotNullAssert<T>(T? value, string? message = null, [CallerMemberName] string? parameterName = null)
        where T : struct
    {
        Debug.Assert(value.HasValue, message ?? parameterName + " != null");
        return value.Value;
    }
    /// <summary>
    /// Checks a value for null and asserts if it was null, otherwise returns the non-null value.
    /// </summary>
    /// <param name="message">
    /// The message to display, or <c>null</c> to create default message using <paramref name="parameterName"/>.
    /// </param>
    /// <param name="value">The value to check.></param>
    /// <param name="parameterName">The name of the parameter to be checked(used for the assert message).</param>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <returns>The non null value.</returns>
    public static T NotNullAssert<T>(T? value, string? message = null, [CallerMemberName] string? parameterName = null)
        where T : class
    {
        if(value != null)
        {
            Debug.Assert(true, message ?? parameterName + " != null");
        }
        return value;
    }
}