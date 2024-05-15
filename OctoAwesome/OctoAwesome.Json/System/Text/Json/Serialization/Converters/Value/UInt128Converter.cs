﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Globalization;

namespace OctoAwesome.Json.Serialization.Converters
{
    internal sealed class UInt128Converter : JsonPrimitiveConverter<UInt128>
    {
        private const int MaxFormatLength = 39;

        public UInt128Converter()
        {
            IsInternalConverterForNumberType = true;
        }

        public override UInt128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(reader.TokenType);
            }

            return ReadCore(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, UInt128 value, JsonSerializerOptions options)
        {
            WriteCore(writer, value);
        }

        private static UInt128 ReadCore(ref Utf8JsonReader reader)
        {
            int bufferLength = reader.ValueLength;

#if NET8_0_OR_GREATER
            byte[]? rentedBuffer = null;
            Span<byte> buffer = bufferLength <= JsonConstants.StackallocByteThreshold
                ? stackalloc byte[JsonConstants.StackallocByteThreshold]
                : (rentedBuffer = ArrayPool<byte>.Shared.Rent(bufferLength));
#else
            char[]? rentedBuffer = null;
            Span<char> buffer = bufferLength <= JsonConstants.StackallocCharThreshold
                ? stackalloc char[JsonConstants.StackallocCharThreshold]
                : (rentedBuffer = ArrayPool<char>.Shared.Rent(bufferLength));
#endif
            int written = reader.CopyValue(buffer);
            if (!TryParse(buffer.Slice(0, written), out UInt128 result))
            {
                ThrowHelper.ThrowFormatException(NumericType.UInt128);
            }

            if (rentedBuffer != null)
            {
#if NET8_0_OR_GREATER
                ArrayPool<byte>.Shared.Return(rentedBuffer);
#else
                ArrayPool<char>.Shared.Return(rentedBuffer);
#endif
            }

            return result;
        }

        private static void WriteCore(Utf8JsonWriter writer, UInt128 value)
        {
#if NET8_0_OR_GREATER
            Span<byte> buffer = stackalloc byte[MaxFormatLength];
#else
            Span<char> buffer = stackalloc char[MaxFormatLength];
#endif
            Format(buffer, value, out int written);
            writer.WriteRawValue(buffer.Slice(0, written));
        }

        internal override UInt128 ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            return ReadCore(ref reader);
        }

        internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, UInt128 value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
        {
#if NET8_0_OR_GREATER
            Span<byte> buffer = stackalloc byte[MaxFormatLength];
#else
            Span<char> buffer = stackalloc char[MaxFormatLength];
#endif
            Format(buffer, value, out int written);
            writer.WritePropertyName(buffer);
        }

        internal override UInt128 ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                (JsonNumberHandling.AllowReadingFromString & handling) != 0)
            {
                return ReadCore(ref reader);
            }

            return Read(ref reader, Type, options);
        }

        internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, UInt128 value, JsonNumberHandling handling)
        {
            if ((JsonNumberHandling.WriteAsString & handling) != 0)
            {
#if NET8_0_OR_GREATER
                const byte Quote = JsonConstants.Quote;
                Span<byte> buffer = stackalloc byte[MaxFormatLength + 2];
#else
                const char Quote = (char)JsonConstants.Quote;
                Span<char> buffer = stackalloc char[MaxFormatLength + 2];
#endif
                buffer[0] = Quote;
                Format(buffer.Slice(1), value, out int written);

                int length = written + 2;
                buffer[length - 1] = Quote;
                writer.WriteRawValue(buffer.Slice(0, length));
            }
            else
            {
                WriteCore(writer, value);
            }
        }

        // UInt128.TryParse(ROS<byte>) is not available on .NET 7, only UInt128.TryParse(ROS<char>).
        private static bool TryParse(
#if NET8_0_OR_GREATER
            ReadOnlySpan<byte> buffer,
#else
            ReadOnlySpan<char> buffer,
#endif
            out UInt128 result)
        {
            return UInt128.TryParse(buffer, CultureInfo.InvariantCulture, out result);
        }

        private static void Format(
#if NET8_0_OR_GREATER
            Span<byte> destination,
#else
            Span<char> destination,
#endif
            UInt128 value, out int written)
        {
            bool formattedSuccessfully = value.TryFormat(destination, out written, provider: CultureInfo.InvariantCulture);
            Debug.Assert(formattedSuccessfully);
        }
    }
}
