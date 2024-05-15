using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System
{
    // Token: 0x02000004 RID: 4
    internal static class SR
    {
        // Token: 0x06000004 RID: 4 RVA: 0x000020DE File Offset: 0x000002DE
        internal static bool UsingResourceKeys()
        {
            return System.SR.s_usingResourceKeys;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000020E8 File Offset: 0x000002E8
        private static string GetResourceString(string resourceKey)
        {
            return resourceKey;

        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002124 File Offset: 0x00000324
        internal static string Format(string resourceFormat, object p1)
        {
            if (System.SR.UsingResourceKeys())
            {
                return string.Join(", ", new object[] { resourceFormat, p1 });
            }
            return string.Format(resourceFormat, p1);
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000214D File Offset: 0x0000034D
        internal static string Format(string resourceFormat, object p1, object p2)
        {
            if (System.SR.UsingResourceKeys())
            {
                return string.Join(", ", new object[] { resourceFormat, p1, p2 });
            }
            return string.Format(resourceFormat, p1, p2);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x0000217B File Offset: 0x0000037B
        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            if (System.SR.UsingResourceKeys())
            {
                return string.Join(", ", new object[] { resourceFormat, p1, p2, p3 });
            }
            return string.Format(resourceFormat, p1, p2, p3);
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000021AE File Offset: 0x000003AE
        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args == null)
            {
                return resourceFormat;
            }
            if (System.SR.UsingResourceKeys())
            {
                return resourceFormat + ", " + string.Join(", ", args);
            }
            return string.Format(resourceFormat, args);
        }



        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000021FA File Offset: 0x000003FA
        internal static string ArrayDepthTooLarge
        {
            get
            {
                return System.SR.GetResourceString("ArrayDepthTooLarge");
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600000C RID: 12 RVA: 0x00002206 File Offset: 0x00000406
        internal static string CannotReadIncompleteUTF16
        {
            get
            {
                return System.SR.GetResourceString("CannotReadIncompleteUTF16");
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600000D RID: 13 RVA: 0x00002212 File Offset: 0x00000412
        internal static string CannotReadInvalidUTF16
        {
            get
            {
                return System.SR.GetResourceString("CannotReadInvalidUTF16");
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600000E RID: 14 RVA: 0x0000221E File Offset: 0x0000041E
        internal static string CannotStartObjectArrayAfterPrimitiveOrClose
        {
            get
            {
                return System.SR.GetResourceString("CannotStartObjectArrayAfterPrimitiveOrClose");
            }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600000F RID: 15 RVA: 0x0000222A File Offset: 0x0000042A
        internal static string CannotStartObjectArrayWithoutProperty
        {
            get
            {
                return System.SR.GetResourceString("CannotStartObjectArrayWithoutProperty");
            }
        }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000010 RID: 16 RVA: 0x00002236 File Offset: 0x00000436
        internal static string CannotTranscodeInvalidUtf8
        {
            get
            {
                return System.SR.GetResourceString("CannotTranscodeInvalidUtf8");
            }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000011 RID: 17 RVA: 0x00002242 File Offset: 0x00000442
        internal static string CannotDecodeInvalidBase64
        {
            get
            {
                return System.SR.GetResourceString("CannotDecodeInvalidBase64");
            }
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000012 RID: 18 RVA: 0x0000224E File Offset: 0x0000044E
        internal static string CannotTranscodeInvalidUtf16
        {
            get
            {
                return System.SR.GetResourceString("CannotTranscodeInvalidUtf16");
            }
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000013 RID: 19 RVA: 0x0000225A File Offset: 0x0000045A
        internal static string CannotEncodeInvalidUTF16
        {
            get
            {
                return System.SR.GetResourceString("CannotEncodeInvalidUTF16");
            }
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000014 RID: 20 RVA: 0x00002266 File Offset: 0x00000466
        internal static string CannotEncodeInvalidUTF8
        {
            get
            {
                return System.SR.GetResourceString("CannotEncodeInvalidUTF8");
            }
        }

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000015 RID: 21 RVA: 0x00002272 File Offset: 0x00000472
        internal static string CannotWritePropertyWithinArray
        {
            get
            {
                return System.SR.GetResourceString("CannotWritePropertyWithinArray");
            }
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x06000016 RID: 22 RVA: 0x0000227E File Offset: 0x0000047E
        internal static string CannotWritePropertyAfterProperty
        {
            get
            {
                return System.SR.GetResourceString("CannotWritePropertyAfterProperty");
            }
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000017 RID: 23 RVA: 0x0000228A File Offset: 0x0000048A
        internal static string CannotWriteValueAfterPrimitiveOrClose
        {
            get
            {
                return System.SR.GetResourceString("CannotWriteValueAfterPrimitiveOrClose");
            }
        }

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000018 RID: 24 RVA: 0x00002296 File Offset: 0x00000496
        internal static string CannotWriteValueWithinObject
        {
            get
            {
                return System.SR.GetResourceString("CannotWriteValueWithinObject");
            }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000019 RID: 25 RVA: 0x000022A2 File Offset: 0x000004A2
        internal static string DepthTooLarge
        {
            get
            {
                return System.SR.GetResourceString("DepthTooLarge");
            }
        }

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x0600001A RID: 26 RVA: 0x000022AE File Offset: 0x000004AE
        internal static string DestinationTooShort
        {
            get
            {
                return System.SR.GetResourceString("DestinationTooShort");
            }
        }

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600001B RID: 27 RVA: 0x000022BA File Offset: 0x000004BA
        internal static string EndOfCommentNotFound
        {
            get
            {
                return System.SR.GetResourceString("EndOfCommentNotFound");
            }
        }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600001C RID: 28 RVA: 0x000022C6 File Offset: 0x000004C6
        internal static string EndOfStringNotFound
        {
            get
            {
                return System.SR.GetResourceString("EndOfStringNotFound");
            }
        }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x0600001D RID: 29 RVA: 0x000022D2 File Offset: 0x000004D2
        internal static string ExpectedEndAfterSingleJson
        {
            get
            {
                return System.SR.GetResourceString("ExpectedEndAfterSingleJson");
            }
        }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x0600001E RID: 30 RVA: 0x000022DE File Offset: 0x000004DE
        internal static string ExpectedEndOfDigitNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedEndOfDigitNotFound");
            }
        }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x0600001F RID: 31 RVA: 0x000022EA File Offset: 0x000004EA
        internal static string ExpectedFalse
        {
            get
            {
                return System.SR.GetResourceString("ExpectedFalse");
            }
        }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000020 RID: 32 RVA: 0x000022F6 File Offset: 0x000004F6
        internal static string ExpectedJsonTokens
        {
            get
            {
                return System.SR.GetResourceString("ExpectedJsonTokens");
            }
        }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000021 RID: 33 RVA: 0x00002302 File Offset: 0x00000502
        internal static string ExpectedOneCompleteToken
        {
            get
            {
                return System.SR.GetResourceString("ExpectedOneCompleteToken");
            }
        }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x06000022 RID: 34 RVA: 0x0000230E File Offset: 0x0000050E
        internal static string ExpectedNextDigitEValueNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedNextDigitEValueNotFound");
            }
        }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000023 RID: 35 RVA: 0x0000231A File Offset: 0x0000051A
        internal static string ExpectedNull
        {
            get
            {
                return System.SR.GetResourceString("ExpectedNull");
            }
        }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000024 RID: 36 RVA: 0x00002326 File Offset: 0x00000526
        internal static string ExpectedSeparatorAfterPropertyNameNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedSeparatorAfterPropertyNameNotFound");
            }
        }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000025 RID: 37 RVA: 0x00002332 File Offset: 0x00000532
        internal static string ExpectedStartOfPropertyNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedStartOfPropertyNotFound");
            }
        }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000026 RID: 38 RVA: 0x0000233E File Offset: 0x0000053E
        internal static string ExpectedStartOfPropertyOrValueNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedStartOfPropertyOrValueNotFound");
            }
        }

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x06000027 RID: 39 RVA: 0x0000234A File Offset: 0x0000054A
        internal static string ExpectedStartOfValueNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedStartOfValueNotFound");
            }
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00002356 File Offset: 0x00000556
        internal static string ExpectedTrue
        {
            get
            {
                return System.SR.GetResourceString("ExpectedTrue");
            }
        }

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x06000029 RID: 41 RVA: 0x00002362 File Offset: 0x00000562
        internal static string ExpectedValueAfterPropertyNameNotFound
        {
            get
            {
                return System.SR.GetResourceString("ExpectedValueAfterPropertyNameNotFound");
            }
        }

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x0600002A RID: 42 RVA: 0x0000236E File Offset: 0x0000056E
        internal static string FailedToGetLargerSpan
        {
            get
            {
                return System.SR.GetResourceString("FailedToGetLargerSpan");
            }
        }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x0600002B RID: 43 RVA: 0x0000237A File Offset: 0x0000057A
        internal static string FoundInvalidCharacter
        {
            get
            {
                return System.SR.GetResourceString("FoundInvalidCharacter");
            }
        }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x0600002C RID: 44 RVA: 0x00002386 File Offset: 0x00000586
        internal static string InvalidCast
        {
            get
            {
                return System.SR.GetResourceString("InvalidCast");
            }
        }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00002392 File Offset: 0x00000592
        internal static string InvalidCharacterAfterEscapeWithinString
        {
            get
            {
                return System.SR.GetResourceString("InvalidCharacterAfterEscapeWithinString");
            }
        }

        // Token: 0x17000026 RID: 38
        // (get) Token: 0x0600002E RID: 46 RVA: 0x0000239E File Offset: 0x0000059E
        internal static string InvalidCharacterWithinString
        {
            get
            {
                return System.SR.GetResourceString("InvalidCharacterWithinString");
            }
        }

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x0600002F RID: 47 RVA: 0x000023AA File Offset: 0x000005AA
        internal static string InvalidEnumTypeWithSpecialChar
        {
            get
            {
                return System.SR.GetResourceString("InvalidEnumTypeWithSpecialChar");
            }
        }

        // Token: 0x17000028 RID: 40
        // (get) Token: 0x06000030 RID: 48 RVA: 0x000023B6 File Offset: 0x000005B6
        internal static string InvalidEndOfJsonNonPrimitive
        {
            get
            {
                return System.SR.GetResourceString("InvalidEndOfJsonNonPrimitive");
            }
        }

        // Token: 0x17000029 RID: 41
        // (get) Token: 0x06000031 RID: 49 RVA: 0x000023C2 File Offset: 0x000005C2
        internal static string InvalidHexCharacterWithinString
        {
            get
            {
                return System.SR.GetResourceString("InvalidHexCharacterWithinString");
            }
        }

        // Token: 0x1700002A RID: 42
        // (get) Token: 0x06000032 RID: 50 RVA: 0x000023CE File Offset: 0x000005CE
        internal static string JsonDocumentDoesNotSupportComments
        {
            get
            {
                return System.SR.GetResourceString("JsonDocumentDoesNotSupportComments");
            }
        }

        // Token: 0x1700002B RID: 43
        // (get) Token: 0x06000033 RID: 51 RVA: 0x000023DA File Offset: 0x000005DA
        internal static string JsonElementHasWrongType
        {
            get
            {
                return System.SR.GetResourceString("JsonElementHasWrongType");
            }
        }

        // Token: 0x1700002C RID: 44
        // (get) Token: 0x06000034 RID: 52 RVA: 0x000023E6 File Offset: 0x000005E6
        internal static string DefaultTypeInfoResolverImmutable
        {
            get
            {
                return System.SR.GetResourceString("DefaultTypeInfoResolverImmutable");
            }
        }

        // Token: 0x1700002D RID: 45
        // (get) Token: 0x06000035 RID: 53 RVA: 0x000023F2 File Offset: 0x000005F2
        internal static string TypeInfoResolverChainImmutable
        {
            get
            {
                return System.SR.GetResourceString("TypeInfoResolverChainImmutable");
            }
        }

        // Token: 0x1700002E RID: 46
        // (get) Token: 0x06000036 RID: 54 RVA: 0x000023FE File Offset: 0x000005FE
        internal static string TypeInfoImmutable
        {
            get
            {
                return System.SR.GetResourceString("TypeInfoImmutable");
            }
        }

        // Token: 0x1700002F RID: 47
        // (get) Token: 0x06000037 RID: 55 RVA: 0x0000240A File Offset: 0x0000060A
        internal static string MaxDepthMustBePositive
        {
            get
            {
                return System.SR.GetResourceString("MaxDepthMustBePositive");
            }
        }

        // Token: 0x17000030 RID: 48
        // (get) Token: 0x06000038 RID: 56 RVA: 0x00002416 File Offset: 0x00000616
        internal static string CommentHandlingMustBeValid
        {
            get
            {
                return System.SR.GetResourceString("CommentHandlingMustBeValid");
            }
        }

        // Token: 0x17000031 RID: 49
        // (get) Token: 0x06000039 RID: 57 RVA: 0x00002422 File Offset: 0x00000622
        internal static string MismatchedObjectArray
        {
            get
            {
                return System.SR.GetResourceString("MismatchedObjectArray");
            }
        }

        // Token: 0x17000032 RID: 50
        // (get) Token: 0x0600003A RID: 58 RVA: 0x0000242E File Offset: 0x0000062E
        internal static string CannotWriteEndAfterProperty
        {
            get
            {
                return System.SR.GetResourceString("CannotWriteEndAfterProperty");
            }
        }

        // Token: 0x17000033 RID: 51
        // (get) Token: 0x0600003B RID: 59 RVA: 0x0000243A File Offset: 0x0000063A
        internal static string ObjectDepthTooLarge
        {
            get
            {
                return System.SR.GetResourceString("ObjectDepthTooLarge");
            }
        }

        // Token: 0x17000034 RID: 52
        // (get) Token: 0x0600003C RID: 60 RVA: 0x00002446 File Offset: 0x00000646
        internal static string PropertyNameTooLarge
        {
            get
            {
                return System.SR.GetResourceString("PropertyNameTooLarge");
            }
        }

        // Token: 0x17000035 RID: 53
        // (get) Token: 0x0600003D RID: 61 RVA: 0x00002452 File Offset: 0x00000652
        internal static string FormatDecimal
        {
            get
            {
                return System.SR.GetResourceString("FormatDecimal");
            }
        }

        // Token: 0x17000036 RID: 54
        // (get) Token: 0x0600003E RID: 62 RVA: 0x0000245E File Offset: 0x0000065E
        internal static string FormatDouble
        {
            get
            {
                return System.SR.GetResourceString("FormatDouble");
            }
        }

        // Token: 0x17000037 RID: 55
        // (get) Token: 0x0600003F RID: 63 RVA: 0x0000246A File Offset: 0x0000066A
        internal static string FormatInt32
        {
            get
            {
                return System.SR.GetResourceString("FormatInt32");
            }
        }

        // Token: 0x17000038 RID: 56
        // (get) Token: 0x06000040 RID: 64 RVA: 0x00002476 File Offset: 0x00000676
        internal static string FormatInt64
        {
            get
            {
                return System.SR.GetResourceString("FormatInt64");
            }
        }

        // Token: 0x17000039 RID: 57
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00002482 File Offset: 0x00000682
        internal static string FormatSingle
        {
            get
            {
                return System.SR.GetResourceString("FormatSingle");
            }
        }

        // Token: 0x1700003A RID: 58
        // (get) Token: 0x06000042 RID: 66 RVA: 0x0000248E File Offset: 0x0000068E
        internal static string FormatUInt32
        {
            get
            {
                return System.SR.GetResourceString("FormatUInt32");
            }
        }

        // Token: 0x1700003B RID: 59
        // (get) Token: 0x06000043 RID: 67 RVA: 0x0000249A File Offset: 0x0000069A
        internal static string FormatUInt64
        {
            get
            {
                return System.SR.GetResourceString("FormatUInt64");
            }
        }

        // Token: 0x1700003C RID: 60
        // (get) Token: 0x06000044 RID: 68 RVA: 0x000024A6 File Offset: 0x000006A6
        internal static string RequiredDigitNotFoundAfterDecimal
        {
            get
            {
                return System.SR.GetResourceString("RequiredDigitNotFoundAfterDecimal");
            }
        }

        // Token: 0x1700003D RID: 61
        // (get) Token: 0x06000045 RID: 69 RVA: 0x000024B2 File Offset: 0x000006B2
        internal static string RequiredDigitNotFoundAfterSign
        {
            get
            {
                return System.SR.GetResourceString("RequiredDigitNotFoundAfterSign");
            }
        }

        // Token: 0x1700003E RID: 62
        // (get) Token: 0x06000046 RID: 70 RVA: 0x000024BE File Offset: 0x000006BE
        internal static string RequiredDigitNotFoundEndOfData
        {
            get
            {
                return System.SR.GetResourceString("RequiredDigitNotFoundEndOfData");
            }
        }

        // Token: 0x1700003F RID: 63
        // (get) Token: 0x06000047 RID: 71 RVA: 0x000024CA File Offset: 0x000006CA
        internal static string SpecialNumberValuesNotSupported
        {
            get
            {
                return System.SR.GetResourceString("SpecialNumberValuesNotSupported");
            }
        }

        // Token: 0x17000040 RID: 64
        // (get) Token: 0x06000048 RID: 72 RVA: 0x000024D6 File Offset: 0x000006D6
        internal static string ValueTooLarge
        {
            get
            {
                return System.SR.GetResourceString("ValueTooLarge");
            }
        }

        // Token: 0x17000041 RID: 65
        // (get) Token: 0x06000049 RID: 73 RVA: 0x000024E2 File Offset: 0x000006E2
        internal static string ZeroDepthAtEnd
        {
            get
            {
                return System.SR.GetResourceString("ZeroDepthAtEnd");
            }
        }

        // Token: 0x17000042 RID: 66
        // (get) Token: 0x0600004A RID: 74 RVA: 0x000024EE File Offset: 0x000006EE
        internal static string DeserializeUnableToConvertValue
        {
            get
            {
                return System.SR.GetResourceString("DeserializeUnableToConvertValue");
            }
        }

        // Token: 0x17000043 RID: 67
        // (get) Token: 0x0600004B RID: 75 RVA: 0x000024FA File Offset: 0x000006FA
        internal static string DeserializeWrongType
        {
            get
            {
                return System.SR.GetResourceString("DeserializeWrongType");
            }
        }

        // Token: 0x17000044 RID: 68
        // (get) Token: 0x0600004C RID: 76 RVA: 0x00002506 File Offset: 0x00000706
        internal static string SerializationInvalidBufferSize
        {
            get
            {
                return System.SR.GetResourceString("SerializationInvalidBufferSize");
            }
        }

        // Token: 0x17000045 RID: 69
        // (get) Token: 0x0600004D RID: 77 RVA: 0x00002512 File Offset: 0x00000712
        internal static string InvalidComparison
        {
            get
            {
                return System.SR.GetResourceString("InvalidComparison");
            }
        }

        // Token: 0x17000046 RID: 70
        // (get) Token: 0x0600004E RID: 78 RVA: 0x0000251E File Offset: 0x0000071E
        internal static string UnsupportedFormat
        {
            get
            {
                return System.SR.GetResourceString("UnsupportedFormat");
            }
        }

        // Token: 0x17000047 RID: 71
        // (get) Token: 0x0600004F RID: 79 RVA: 0x0000252A File Offset: 0x0000072A
        internal static string ExpectedStartOfPropertyOrValueAfterComment
        {
            get
            {
                return System.SR.GetResourceString("ExpectedStartOfPropertyOrValueAfterComment");
            }
        }

        // Token: 0x17000048 RID: 72
        // (get) Token: 0x06000050 RID: 80 RVA: 0x00002536 File Offset: 0x00000736
        internal static string TrailingCommaNotAllowedBeforeArrayEnd
        {
            get
            {
                return System.SR.GetResourceString("TrailingCommaNotAllowedBeforeArrayEnd");
            }
        }

        // Token: 0x17000049 RID: 73
        // (get) Token: 0x06000051 RID: 81 RVA: 0x00002542 File Offset: 0x00000742
        internal static string TrailingCommaNotAllowedBeforeObjectEnd
        {
            get
            {
                return System.SR.GetResourceString("TrailingCommaNotAllowedBeforeObjectEnd");
            }
        }

        // Token: 0x1700004A RID: 74
        // (get) Token: 0x06000052 RID: 82 RVA: 0x0000254E File Offset: 0x0000074E
        internal static string SerializerOptionsReadOnly
        {
            get
            {
                return System.SR.GetResourceString("SerializerOptionsReadOnly");
            }
        }

        // Token: 0x1700004B RID: 75
        // (get) Token: 0x06000053 RID: 83 RVA: 0x0000255A File Offset: 0x0000075A
        internal static string SerializerOptions_InvalidChainedResolver
        {
            get
            {
                return System.SR.GetResourceString("SerializerOptions_InvalidChainedResolver");
            }
        }

        // Token: 0x1700004C RID: 76
        // (get) Token: 0x06000054 RID: 84 RVA: 0x00002566 File Offset: 0x00000766
        internal static string StreamNotWritable
        {
            get
            {
                return System.SR.GetResourceString("StreamNotWritable");
            }
        }

        // Token: 0x1700004D RID: 77
        // (get) Token: 0x06000055 RID: 85 RVA: 0x00002572 File Offset: 0x00000772
        internal static string CannotWriteCommentWithEmbeddedDelimiter
        {
            get
            {
                return System.SR.GetResourceString("CannotWriteCommentWithEmbeddedDelimiter");
            }
        }

        // Token: 0x1700004E RID: 78
        // (get) Token: 0x06000056 RID: 86 RVA: 0x0000257E File Offset: 0x0000077E
        internal static string SerializerPropertyNameConflict
        {
            get
            {
                return System.SR.GetResourceString("SerializerPropertyNameConflict");
            }
        }

        // Token: 0x1700004F RID: 79
        // (get) Token: 0x06000057 RID: 87 RVA: 0x0000258A File Offset: 0x0000078A
        internal static string SerializerPropertyNameNull
        {
            get
            {
                return System.SR.GetResourceString("SerializerPropertyNameNull");
            }
        }

        // Token: 0x17000050 RID: 80
        // (get) Token: 0x06000058 RID: 88 RVA: 0x00002596 File Offset: 0x00000796
        internal static string SerializationDataExtensionPropertyInvalid
        {
            get
            {
                return System.SR.GetResourceString("SerializationDataExtensionPropertyInvalid");
            }
        }

        // Token: 0x17000051 RID: 81
        // (get) Token: 0x06000059 RID: 89 RVA: 0x000025A2 File Offset: 0x000007A2
        internal static string SerializationDuplicateTypeAttribute
        {
            get
            {
                return System.SR.GetResourceString("SerializationDuplicateTypeAttribute");
            }
        }

        // Token: 0x17000052 RID: 82
        // (get) Token: 0x0600005A RID: 90 RVA: 0x000025AE File Offset: 0x000007AE
        internal static string ExtensionDataConflictsWithUnmappedMemberHandling
        {
            get
            {
                return System.SR.GetResourceString("ExtensionDataConflictsWithUnmappedMemberHandling");
            }
        }

        // Token: 0x17000053 RID: 83
        // (get) Token: 0x0600005B RID: 91 RVA: 0x000025BA File Offset: 0x000007BA
        internal static string SerializationNotSupportedType
        {
            get
            {
                return System.SR.GetResourceString("SerializationNotSupportedType");
            }
        }

        // Token: 0x17000054 RID: 84
        // (get) Token: 0x0600005C RID: 92 RVA: 0x000025C6 File Offset: 0x000007C6
        internal static string TypeRequiresAsyncSerialization
        {
            get
            {
                return System.SR.GetResourceString("TypeRequiresAsyncSerialization");
            }
        }

        // Token: 0x17000055 RID: 85
        // (get) Token: 0x0600005D RID: 93 RVA: 0x000025D2 File Offset: 0x000007D2
        internal static string InvalidCharacterAtStartOfComment
        {
            get
            {
                return System.SR.GetResourceString("InvalidCharacterAtStartOfComment");
            }
        }

        // Token: 0x17000056 RID: 86
        // (get) Token: 0x0600005E RID: 94 RVA: 0x000025DE File Offset: 0x000007DE
        internal static string UnexpectedEndOfDataWhileReadingComment
        {
            get
            {
                return System.SR.GetResourceString("UnexpectedEndOfDataWhileReadingComment");
            }
        }

        // Token: 0x17000057 RID: 87
        // (get) Token: 0x0600005F RID: 95 RVA: 0x000025EA File Offset: 0x000007EA
        internal static string CannotSkip
        {
            get
            {
                return System.SR.GetResourceString("CannotSkip");
            }
        }

        // Token: 0x17000058 RID: 88
        // (get) Token: 0x06000060 RID: 96 RVA: 0x000025F6 File Offset: 0x000007F6
        internal static string NotEnoughData
        {
            get
            {
                return System.SR.GetResourceString("NotEnoughData");
            }
        }

        // Token: 0x17000059 RID: 89
        // (get) Token: 0x06000061 RID: 97 RVA: 0x00002602 File Offset: 0x00000802
        internal static string UnexpectedEndOfLineSeparator
        {
            get
            {
                return System.SR.GetResourceString("UnexpectedEndOfLineSeparator");
            }
        }

        // Token: 0x1700005A RID: 90
        // (get) Token: 0x06000062 RID: 98 RVA: 0x0000260E File Offset: 0x0000080E
        internal static string JsonSerializerDoesNotSupportComments
        {
            get
            {
                return System.SR.GetResourceString("JsonSerializerDoesNotSupportComments");
            }
        }

        // Token: 0x1700005B RID: 91
        // (get) Token: 0x06000063 RID: 99 RVA: 0x0000261A File Offset: 0x0000081A
        internal static string DeserializeNoConstructor
        {
            get
            {
                return System.SR.GetResourceString("DeserializeNoConstructor");
            }
        }

        // Token: 0x1700005C RID: 92
        // (get) Token: 0x06000064 RID: 100 RVA: 0x00002626 File Offset: 0x00000826
        internal static string DeserializePolymorphicInterface
        {
            get
            {
                return System.SR.GetResourceString("DeserializePolymorphicInterface");
            }
        }

        // Token: 0x1700005D RID: 93
        // (get) Token: 0x06000065 RID: 101 RVA: 0x00002632 File Offset: 0x00000832
        internal static string SerializationConverterOnAttributeNotCompatible
        {
            get
            {
                return System.SR.GetResourceString("SerializationConverterOnAttributeNotCompatible");
            }
        }

        // Token: 0x1700005E RID: 94
        // (get) Token: 0x06000066 RID: 102 RVA: 0x0000263E File Offset: 0x0000083E
        internal static string SerializationConverterOnAttributeInvalid
        {
            get
            {
                return System.SR.GetResourceString("SerializationConverterOnAttributeInvalid");
            }
        }

        // Token: 0x1700005F RID: 95
        // (get) Token: 0x06000067 RID: 103 RVA: 0x0000264A File Offset: 0x0000084A
        internal static string SerializationConverterRead
        {
            get
            {
                return System.SR.GetResourceString("SerializationConverterRead");
            }
        }

        // Token: 0x17000060 RID: 96
        // (get) Token: 0x06000068 RID: 104 RVA: 0x00002656 File Offset: 0x00000856
        internal static string SerializationConverterNotCompatible
        {
            get
            {
                return System.SR.GetResourceString("SerializationConverterNotCompatible");
            }
        }

        // Token: 0x17000061 RID: 97
        // (get) Token: 0x06000069 RID: 105 RVA: 0x00002662 File Offset: 0x00000862
        internal static string ResolverTypeNotCompatible
        {
            get
            {
                return System.SR.GetResourceString("ResolverTypeNotCompatible");
            }
        }

        // Token: 0x17000062 RID: 98
        // (get) Token: 0x0600006A RID: 106 RVA: 0x0000266E File Offset: 0x0000086E
        internal static string ResolverTypeInfoOptionsNotCompatible
        {
            get
            {
                return System.SR.GetResourceString("ResolverTypeInfoOptionsNotCompatible");
            }
        }

        // Token: 0x17000063 RID: 99
        // (get) Token: 0x0600006B RID: 107 RVA: 0x0000267A File Offset: 0x0000087A
        internal static string SerializationConverterWrite
        {
            get
            {
                return System.SR.GetResourceString("SerializationConverterWrite");
            }
        }

        // Token: 0x17000064 RID: 100
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00002686 File Offset: 0x00000886
        internal static string NamingPolicyReturnNull
        {
            get
            {
                return System.SR.GetResourceString("NamingPolicyReturnNull");
            }
        }

        // Token: 0x17000065 RID: 101
        // (get) Token: 0x0600006D RID: 109 RVA: 0x00002692 File Offset: 0x00000892
        internal static string SerializationDuplicateAttribute
        {
            get
            {
                return System.SR.GetResourceString("SerializationDuplicateAttribute");
            }
        }

        // Token: 0x17000066 RID: 102
        // (get) Token: 0x0600006E RID: 110 RVA: 0x0000269E File Offset: 0x0000089E
        internal static string SerializeUnableToSerialize
        {
            get
            {
                return System.SR.GetResourceString("SerializeUnableToSerialize");
            }
        }

        // Token: 0x17000067 RID: 103
        // (get) Token: 0x0600006F RID: 111 RVA: 0x000026AA File Offset: 0x000008AA
        internal static string FormatByte
        {
            get
            {
                return System.SR.GetResourceString("FormatByte");
            }
        }

        // Token: 0x17000068 RID: 104
        // (get) Token: 0x06000070 RID: 112 RVA: 0x000026B6 File Offset: 0x000008B6
        internal static string FormatInt16
        {
            get
            {
                return System.SR.GetResourceString("FormatInt16");
            }
        }

        // Token: 0x17000069 RID: 105
        // (get) Token: 0x06000071 RID: 113 RVA: 0x000026C2 File Offset: 0x000008C2
        internal static string FormatSByte
        {
            get
            {
                return System.SR.GetResourceString("FormatSByte");
            }
        }

        // Token: 0x1700006A RID: 106
        // (get) Token: 0x06000072 RID: 114 RVA: 0x000026CE File Offset: 0x000008CE
        internal static string FormatUInt16
        {
            get
            {
                return System.SR.GetResourceString("FormatUInt16");
            }
        }

        // Token: 0x1700006B RID: 107
        // (get) Token: 0x06000073 RID: 115 RVA: 0x000026DA File Offset: 0x000008DA
        internal static string SerializerCycleDetected
        {
            get
            {
                return System.SR.GetResourceString("SerializerCycleDetected");
            }
        }

        // Token: 0x1700006C RID: 108
        // (get) Token: 0x06000074 RID: 116 RVA: 0x000026E6 File Offset: 0x000008E6
        internal static string InvalidLeadingZeroInNumber
        {
            get
            {
                return System.SR.GetResourceString("InvalidLeadingZeroInNumber");
            }
        }

        // Token: 0x1700006D RID: 109
        // (get) Token: 0x06000075 RID: 117 RVA: 0x000026F2 File Offset: 0x000008F2
        internal static string MetadataCannotParsePreservedObjectToImmutable
        {
            get
            {
                return System.SR.GetResourceString("MetadataCannotParsePreservedObjectToImmutable");
            }
        }

        // Token: 0x1700006E RID: 110
        // (get) Token: 0x06000076 RID: 118 RVA: 0x000026FE File Offset: 0x000008FE
        internal static string MetadataDuplicateIdFound
        {
            get
            {
                return System.SR.GetResourceString("MetadataDuplicateIdFound");
            }
        }

        // Token: 0x1700006F RID: 111
        // (get) Token: 0x06000077 RID: 119 RVA: 0x0000270A File Offset: 0x0000090A
        internal static string MetadataIdIsNotFirstProperty
        {
            get
            {
                return System.SR.GetResourceString("MetadataIdIsNotFirstProperty");
            }
        }

        // Token: 0x17000070 RID: 112
        // (get) Token: 0x06000078 RID: 120 RVA: 0x00002716 File Offset: 0x00000916
        internal static string MetadataInvalidReferenceToValueType
        {
            get
            {
                return System.SR.GetResourceString("MetadataInvalidReferenceToValueType");
            }
        }

        // Token: 0x17000071 RID: 113
        // (get) Token: 0x06000079 RID: 121 RVA: 0x00002722 File Offset: 0x00000922
        internal static string MetadataInvalidTokenAfterValues
        {
            get
            {
                return System.SR.GetResourceString("MetadataInvalidTokenAfterValues");
            }
        }

        // Token: 0x17000072 RID: 114
        // (get) Token: 0x0600007A RID: 122 RVA: 0x0000272E File Offset: 0x0000092E
        internal static string MetadataPreservedArrayFailed
        {
            get
            {
                return System.SR.GetResourceString("MetadataPreservedArrayFailed");
            }
        }

        // Token: 0x17000073 RID: 115
        // (get) Token: 0x0600007B RID: 123 RVA: 0x0000273A File Offset: 0x0000093A
        internal static string MetadataInvalidPropertyInArrayMetadata
        {
            get
            {
                return System.SR.GetResourceString("MetadataInvalidPropertyInArrayMetadata");
            }
        }

        // Token: 0x17000074 RID: 116
        // (get) Token: 0x0600007C RID: 124 RVA: 0x00002746 File Offset: 0x00000946
        internal static string MetadataStandaloneValuesProperty
        {
            get
            {
                return System.SR.GetResourceString("MetadataStandaloneValuesProperty");
            }
        }

        // Token: 0x17000075 RID: 117
        // (get) Token: 0x0600007D RID: 125 RVA: 0x00002752 File Offset: 0x00000952
        internal static string MetadataReferenceCannotContainOtherProperties
        {
            get
            {
                return System.SR.GetResourceString("MetadataReferenceCannotContainOtherProperties");
            }
        }

        // Token: 0x17000076 RID: 118
        // (get) Token: 0x0600007E RID: 126 RVA: 0x0000275E File Offset: 0x0000095E
        internal static string MetadataReferenceNotFound
        {
            get
            {
                return System.SR.GetResourceString("MetadataReferenceNotFound");
            }
        }

        // Token: 0x17000077 RID: 119
        // (get) Token: 0x0600007F RID: 127 RVA: 0x0000276A File Offset: 0x0000096A
        internal static string MetadataValueWasNotString
        {
            get
            {
                return System.SR.GetResourceString("MetadataValueWasNotString");
            }
        }

        // Token: 0x17000078 RID: 120
        // (get) Token: 0x06000080 RID: 128 RVA: 0x00002776 File Offset: 0x00000976
        internal static string MetadataInvalidPropertyWithLeadingDollarSign
        {
            get
            {
                return System.SR.GetResourceString("MetadataInvalidPropertyWithLeadingDollarSign");
            }
        }

        // Token: 0x17000079 RID: 121
        // (get) Token: 0x06000081 RID: 129 RVA: 0x00002782 File Offset: 0x00000982
        internal static string MetadataUnexpectedProperty
        {
            get
            {
                return System.SR.GetResourceString("MetadataUnexpectedProperty");
            }
        }

        // Token: 0x1700007A RID: 122
        // (get) Token: 0x06000082 RID: 130 RVA: 0x0000278E File Offset: 0x0000098E
        internal static string UnmappedJsonProperty
        {
            get
            {
                return System.SR.GetResourceString("UnmappedJsonProperty");
            }
        }

        // Token: 0x1700007B RID: 123
        // (get) Token: 0x06000083 RID: 131 RVA: 0x0000279A File Offset: 0x0000099A
        internal static string MetadataDuplicateTypeProperty
        {
            get
            {
                return System.SR.GetResourceString("MetadataDuplicateTypeProperty");
            }
        }

        // Token: 0x1700007C RID: 124
        // (get) Token: 0x06000084 RID: 132 RVA: 0x000027A6 File Offset: 0x000009A6
        internal static string MultipleMembersBindWithConstructorParameter
        {
            get
            {
                return System.SR.GetResourceString("MultipleMembersBindWithConstructorParameter");
            }
        }

        // Token: 0x1700007D RID: 125
        // (get) Token: 0x06000085 RID: 133 RVA: 0x000027B2 File Offset: 0x000009B2
        internal static string ConstructorParamIncompleteBinding
        {
            get
            {
                return System.SR.GetResourceString("ConstructorParamIncompleteBinding");
            }
        }

        // Token: 0x1700007E RID: 126
        // (get) Token: 0x06000086 RID: 134 RVA: 0x000027BE File Offset: 0x000009BE
        internal static string ObjectWithParameterizedCtorRefMetadataNotSupported
        {
            get
            {
                return System.SR.GetResourceString("ObjectWithParameterizedCtorRefMetadataNotSupported");
            }
        }

        // Token: 0x1700007F RID: 127
        // (get) Token: 0x06000087 RID: 135 RVA: 0x000027CA File Offset: 0x000009CA
        internal static string SerializerConverterFactoryReturnsNull
        {
            get
            {
                return System.SR.GetResourceString("SerializerConverterFactoryReturnsNull");
            }
        }

        // Token: 0x17000080 RID: 128
        // (get) Token: 0x06000088 RID: 136 RVA: 0x000027D6 File Offset: 0x000009D6
        internal static string SerializationNotSupportedParentType
        {
            get
            {
                return System.SR.GetResourceString("SerializationNotSupportedParentType");
            }
        }

        // Token: 0x17000081 RID: 129
        // (get) Token: 0x06000089 RID: 137 RVA: 0x000027E2 File Offset: 0x000009E2
        internal static string ExtensionDataCannotBindToCtorParam
        {
            get
            {
                return System.SR.GetResourceString("ExtensionDataCannotBindToCtorParam");
            }
        }

        // Token: 0x17000082 RID: 130
        // (get) Token: 0x0600008A RID: 138 RVA: 0x000027EE File Offset: 0x000009EE
        internal static string BufferMaximumSizeExceeded
        {
            get
            {
                return System.SR.GetResourceString("BufferMaximumSizeExceeded");
            }
        }

        // Token: 0x17000083 RID: 131
        // (get) Token: 0x0600008B RID: 139 RVA: 0x000027FA File Offset: 0x000009FA
        internal static string CannotSerializeInvalidType
        {
            get
            {
                return System.SR.GetResourceString("CannotSerializeInvalidType");
            }
        }

        // Token: 0x17000084 RID: 132
        // (get) Token: 0x0600008C RID: 140 RVA: 0x00002806 File Offset: 0x00000A06
        internal static string SerializeTypeInstanceNotSupported
        {
            get
            {
                return System.SR.GetResourceString("SerializeTypeInstanceNotSupported");
            }
        }

        // Token: 0x17000085 RID: 133
        // (get) Token: 0x0600008D RID: 141 RVA: 0x00002812 File Offset: 0x00000A12
        internal static string JsonIncludeOnInaccessibleProperty
        {
            get
            {
                return System.SR.GetResourceString("JsonIncludeOnInaccessibleProperty");
            }
        }

        // Token: 0x17000086 RID: 134
        // (get) Token: 0x0600008E RID: 142 RVA: 0x0000281E File Offset: 0x00000A1E
        internal static string CannotSerializeInvalidMember
        {
            get
            {
                return System.SR.GetResourceString("CannotSerializeInvalidMember");
            }
        }

        // Token: 0x17000087 RID: 135
        // (get) Token: 0x0600008F RID: 143 RVA: 0x0000282A File Offset: 0x00000A2A
        internal static string CannotPopulateCollection
        {
            get
            {
                return System.SR.GetResourceString("CannotPopulateCollection");
            }
        }

        // Token: 0x17000088 RID: 136
        // (get) Token: 0x06000090 RID: 144 RVA: 0x00002836 File Offset: 0x00000A36
        internal static string ConstructorContainsNullParameterNames
        {
            get
            {
                return System.SR.GetResourceString("ConstructorContainsNullParameterNames");
            }
        }

        // Token: 0x17000089 RID: 137
        // (get) Token: 0x06000091 RID: 145 RVA: 0x00002842 File Offset: 0x00000A42
        internal static string DefaultIgnoreConditionAlreadySpecified
        {
            get
            {
                return System.SR.GetResourceString("DefaultIgnoreConditionAlreadySpecified");
            }
        }

        // Token: 0x1700008A RID: 138
        // (get) Token: 0x06000092 RID: 146 RVA: 0x0000284E File Offset: 0x00000A4E
        internal static string DefaultIgnoreConditionInvalid
        {
            get
            {
                return System.SR.GetResourceString("DefaultIgnoreConditionInvalid");
            }
        }

        // Token: 0x1700008B RID: 139
        // (get) Token: 0x06000093 RID: 147 RVA: 0x0000285A File Offset: 0x00000A5A
        internal static string DictionaryKeyTypeNotSupported
        {
            get
            {
                return System.SR.GetResourceString("DictionaryKeyTypeNotSupported");
            }
        }

        // Token: 0x1700008C RID: 140
        // (get) Token: 0x06000094 RID: 148 RVA: 0x00002866 File Offset: 0x00000A66
        internal static string IgnoreConditionOnValueTypeInvalid
        {
            get
            {
                return System.SR.GetResourceString("IgnoreConditionOnValueTypeInvalid");
            }
        }

        // Token: 0x1700008D RID: 141
        // (get) Token: 0x06000095 RID: 149 RVA: 0x00002872 File Offset: 0x00000A72
        internal static string NumberHandlingOnPropertyInvalid
        {
            get
            {
                return System.SR.GetResourceString("NumberHandlingOnPropertyInvalid");
            }
        }

        // Token: 0x1700008E RID: 142
        // (get) Token: 0x06000096 RID: 150 RVA: 0x0000287E File Offset: 0x00000A7E
        internal static string ConverterCanConvertMultipleTypes
        {
            get
            {
                return System.SR.GetResourceString("ConverterCanConvertMultipleTypes");
            }
        }

        // Token: 0x1700008F RID: 143
        // (get) Token: 0x06000097 RID: 151 RVA: 0x0000288A File Offset: 0x00000A8A
        internal static string MetadataReferenceOfTypeCannotBeAssignedToType
        {
            get
            {
                return System.SR.GetResourceString("MetadataReferenceOfTypeCannotBeAssignedToType");
            }
        }

        // Token: 0x17000090 RID: 144
        // (get) Token: 0x06000098 RID: 152 RVA: 0x00002896 File Offset: 0x00000A96
        internal static string DeserializeUnableToAssignValue
        {
            get
            {
                return System.SR.GetResourceString("DeserializeUnableToAssignValue");
            }
        }

        // Token: 0x17000091 RID: 145
        // (get) Token: 0x06000099 RID: 153 RVA: 0x000028A2 File Offset: 0x00000AA2
        internal static string DeserializeUnableToAssignNull
        {
            get
            {
                return System.SR.GetResourceString("DeserializeUnableToAssignNull");
            }
        }

        // Token: 0x17000092 RID: 146
        // (get) Token: 0x0600009A RID: 154 RVA: 0x000028AE File Offset: 0x00000AAE
        internal static string SerializerConverterFactoryReturnsJsonConverterFactory
        {
            get
            {
                return System.SR.GetResourceString("SerializerConverterFactoryReturnsJsonConverterFactory");
            }
        }

        // Token: 0x17000093 RID: 147
        // (get) Token: 0x0600009B RID: 155 RVA: 0x000028BA File Offset: 0x00000ABA
        internal static string SerializerConverterFactoryInvalidArgument
        {
            get
            {
                return System.SR.GetResourceString("SerializerConverterFactoryInvalidArgument");
            }
        }

        // Token: 0x17000094 RID: 148
        // (get) Token: 0x0600009C RID: 156 RVA: 0x000028C6 File Offset: 0x00000AC6
        internal static string NodeElementWrongType
        {
            get
            {
                return System.SR.GetResourceString("NodeElementWrongType");
            }
        }

        // Token: 0x17000095 RID: 149
        // (get) Token: 0x0600009D RID: 157 RVA: 0x000028D2 File Offset: 0x00000AD2
        internal static string NodeElementCannotBeObjectOrArray
        {
            get
            {
                return System.SR.GetResourceString("NodeElementCannotBeObjectOrArray");
            }
        }

        // Token: 0x17000096 RID: 150
        // (get) Token: 0x0600009E RID: 158 RVA: 0x000028DE File Offset: 0x00000ADE
        internal static string NodeAlreadyHasParent
        {
            get
            {
                return System.SR.GetResourceString("NodeAlreadyHasParent");
            }
        }

        // Token: 0x17000097 RID: 151
        // (get) Token: 0x0600009F RID: 159 RVA: 0x000028EA File Offset: 0x00000AEA
        internal static string NodeCycleDetected
        {
            get
            {
                return System.SR.GetResourceString("NodeCycleDetected");
            }
        }

        // Token: 0x17000098 RID: 152
        // (get) Token: 0x060000A0 RID: 160 RVA: 0x000028F6 File Offset: 0x00000AF6
        internal static string NodeUnableToConvert
        {
            get
            {
                return System.SR.GetResourceString("NodeUnableToConvert");
            }
        }

        // Token: 0x17000099 RID: 153
        // (get) Token: 0x060000A1 RID: 161 RVA: 0x00002902 File Offset: 0x00000B02
        internal static string NodeUnableToConvertElement
        {
            get
            {
                return System.SR.GetResourceString("NodeUnableToConvertElement");
            }
        }

        // Token: 0x1700009A RID: 154
        // (get) Token: 0x060000A2 RID: 162 RVA: 0x0000290E File Offset: 0x00000B0E
        internal static string NodeValueNotAllowed
        {
            get
            {
                return System.SR.GetResourceString("NodeValueNotAllowed");
            }
        }

        // Token: 0x1700009B RID: 155
        // (get) Token: 0x060000A3 RID: 163 RVA: 0x0000291A File Offset: 0x00000B1A
        internal static string NodeWrongType
        {
            get
            {
                return System.SR.GetResourceString("NodeWrongType");
            }
        }

        // Token: 0x1700009C RID: 156
        // (get) Token: 0x060000A4 RID: 164 RVA: 0x00002926 File Offset: 0x00000B26
        internal static string NodeParentWrongType
        {
            get
            {
                return System.SR.GetResourceString("NodeParentWrongType");
            }
        }

        // Token: 0x1700009D RID: 157
        // (get) Token: 0x060000A5 RID: 165 RVA: 0x00002932 File Offset: 0x00000B32
        internal static string NodeDuplicateKey
        {
            get
            {
                return System.SR.GetResourceString("NodeDuplicateKey");
            }
        }

        // Token: 0x1700009E RID: 158
        // (get) Token: 0x060000A6 RID: 166 RVA: 0x0000293E File Offset: 0x00000B3E
        internal static string SerializerContextOptionsReadOnly
        {
            get
            {
                return System.SR.GetResourceString("SerializerContextOptionsReadOnly");
            }
        }

        // Token: 0x1700009F RID: 159
        // (get) Token: 0x060000A7 RID: 167 RVA: 0x0000294A File Offset: 0x00000B4A
        internal static string NoMetadataForType
        {
            get
            {
                return System.SR.GetResourceString("NoMetadataForType");
            }
        }

        // Token: 0x170000A0 RID: 160
        // (get) Token: 0x060000A8 RID: 168 RVA: 0x00002956 File Offset: 0x00000B56
        internal static string AmbiguousMetadataForType
        {
            get
            {
                return System.SR.GetResourceString("AmbiguousMetadataForType");
            }
        }

        // Token: 0x170000A1 RID: 161
        // (get) Token: 0x060000A9 RID: 169 RVA: 0x00002962 File Offset: 0x00000B62
        internal static string CollectionIsReadOnly
        {
            get
            {
                return System.SR.GetResourceString("CollectionIsReadOnly");
            }
        }

        // Token: 0x170000A2 RID: 162
        // (get) Token: 0x060000AA RID: 170 RVA: 0x0000296E File Offset: 0x00000B6E
        internal static string ArrayIndexNegative
        {
            get
            {
                return System.SR.GetResourceString("ArrayIndexNegative");
            }
        }

        // Token: 0x170000A3 RID: 163
        // (get) Token: 0x060000AB RID: 171 RVA: 0x0000297A File Offset: 0x00000B7A
        internal static string ArrayTooSmall
        {
            get
            {
                return System.SR.GetResourceString("ArrayTooSmall");
            }
        }

        // Token: 0x170000A4 RID: 164
        // (get) Token: 0x060000AC RID: 172 RVA: 0x00002986 File Offset: 0x00000B86
        internal static string NodeJsonObjectCustomConverterNotAllowedOnExtensionProperty
        {
            get
            {
                return System.SR.GetResourceString("NodeJsonObjectCustomConverterNotAllowedOnExtensionProperty");
            }
        }

        // Token: 0x170000A5 RID: 165
        // (get) Token: 0x060000AD RID: 173 RVA: 0x00002992 File Offset: 0x00000B92
        internal static string NoMetadataForTypeProperties
        {
            get
            {
                return System.SR.GetResourceString("NoMetadataForTypeProperties");
            }
        }

        // Token: 0x170000A6 RID: 166
        // (get) Token: 0x060000AE RID: 174 RVA: 0x0000299E File Offset: 0x00000B9E
        internal static string FieldCannotBeVirtual
        {
            get
            {
                return System.SR.GetResourceString("FieldCannotBeVirtual");
            }
        }

        // Token: 0x170000A7 RID: 167
        // (get) Token: 0x060000AF RID: 175 RVA: 0x000029AA File Offset: 0x00000BAA
        internal static string MissingFSharpCoreMember
        {
            get
            {
                return System.SR.GetResourceString("MissingFSharpCoreMember");
            }
        }

        // Token: 0x170000A8 RID: 168
        // (get) Token: 0x060000B0 RID: 176 RVA: 0x000029B6 File Offset: 0x00000BB6
        internal static string FSharpDiscriminatedUnionsNotSupported
        {
            get
            {
                return System.SR.GetResourceString("FSharpDiscriminatedUnionsNotSupported");
            }
        }

        // Token: 0x170000A9 RID: 169
        // (get) Token: 0x060000B1 RID: 177 RVA: 0x000029C2 File Offset: 0x00000BC2
        internal static string Polymorphism_DerivedConverterDoesNotSupportMetadata
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_DerivedConverterDoesNotSupportMetadata");
            }
        }

        // Token: 0x170000AA RID: 170
        // (get) Token: 0x060000B2 RID: 178 RVA: 0x000029CE File Offset: 0x00000BCE
        internal static string Polymorphism_TypeDoesNotSupportPolymorphism
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_TypeDoesNotSupportPolymorphism");
            }
        }

        // Token: 0x170000AB RID: 171
        // (get) Token: 0x060000B3 RID: 179 RVA: 0x000029DA File Offset: 0x00000BDA
        internal static string Polymorphism_DerivedTypeIsNotSupported
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_DerivedTypeIsNotSupported");
            }
        }

        // Token: 0x170000AC RID: 172
        // (get) Token: 0x060000B4 RID: 180 RVA: 0x000029E6 File Offset: 0x00000BE6
        internal static string Polymorphism_DerivedTypeIsAlreadySpecified
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_DerivedTypeIsAlreadySpecified");
            }
        }

        // Token: 0x170000AD RID: 173
        // (get) Token: 0x060000B5 RID: 181 RVA: 0x000029F2 File Offset: 0x00000BF2
        internal static string Polymorphism_TypeDicriminatorIdIsAlreadySpecified
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_TypeDicriminatorIdIsAlreadySpecified");
            }
        }

        // Token: 0x170000AE RID: 174
        // (get) Token: 0x060000B6 RID: 182 RVA: 0x000029FE File Offset: 0x00000BFE
        internal static string Polymorphism_InvalidCustomTypeDiscriminatorPropertyName
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_InvalidCustomTypeDiscriminatorPropertyName");
            }
        }

        // Token: 0x170000AF RID: 175
        // (get) Token: 0x060000B7 RID: 183 RVA: 0x00002A0A File Offset: 0x00000C0A
        internal static string Polymorphism_ConfigurationDoesNotSpecifyDerivedTypes
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_ConfigurationDoesNotSpecifyDerivedTypes");
            }
        }

        // Token: 0x170000B0 RID: 176
        // (get) Token: 0x060000B8 RID: 184 RVA: 0x00002A16 File Offset: 0x00000C16
        internal static string Polymorphism_UnrecognizedTypeDiscriminator
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_UnrecognizedTypeDiscriminator");
            }
        }

        // Token: 0x170000B1 RID: 177
        // (get) Token: 0x060000B9 RID: 185 RVA: 0x00002A22 File Offset: 0x00000C22
        internal static string Polymorphism_RuntimeTypeNotSupported
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_RuntimeTypeNotSupported");
            }
        }

        // Token: 0x170000B2 RID: 178
        // (get) Token: 0x060000BA RID: 186 RVA: 0x00002A2E File Offset: 0x00000C2E
        internal static string Polymorphism_RuntimeTypeDiamondAmbiguity
        {
            get
            {
                return System.SR.GetResourceString("Polymorphism_RuntimeTypeDiamondAmbiguity");
            }
        }

        // Token: 0x170000B3 RID: 179
        // (get) Token: 0x060000BB RID: 187 RVA: 0x00002A3A File Offset: 0x00000C3A
        internal static string InvalidJsonTypeInfoOperationForKind
        {
            get
            {
                return System.SR.GetResourceString("InvalidJsonTypeInfoOperationForKind");
            }
        }

        // Token: 0x170000B4 RID: 180
        // (get) Token: 0x060000BC RID: 188 RVA: 0x00002A46 File Offset: 0x00000C46
        internal static string CreateObjectConverterNotCompatible
        {
            get
            {
                return System.SR.GetResourceString("CreateObjectConverterNotCompatible");
            }
        }

        // Token: 0x170000B5 RID: 181
        // (get) Token: 0x060000BD RID: 189 RVA: 0x00002A52 File Offset: 0x00000C52
        internal static string JsonPropertyInfoBoundToDifferentParent
        {
            get
            {
                return System.SR.GetResourceString("JsonPropertyInfoBoundToDifferentParent");
            }
        }

        // Token: 0x170000B6 RID: 182
        // (get) Token: 0x060000BE RID: 190 RVA: 0x00002A5E File Offset: 0x00000C5E
        internal static string JsonSerializerOptionsNoTypeInfoResolverSpecified
        {
            get
            {
                return System.SR.GetResourceString("JsonSerializerOptionsNoTypeInfoResolverSpecified");
            }
        }

        // Token: 0x170000B7 RID: 183
        // (get) Token: 0x060000BF RID: 191 RVA: 0x00002A6A File Offset: 0x00000C6A
        internal static string JsonSerializerIsReflectionDisabled
        {
            get
            {
                return System.SR.GetResourceString("JsonSerializerIsReflectionDisabled");
            }
        }

        // Token: 0x170000B8 RID: 184
        // (get) Token: 0x060000C0 RID: 192 RVA: 0x00002A76 File Offset: 0x00000C76
        internal static string JsonPolymorphismOptionsAssociatedWithDifferentJsonTypeInfo
        {
            get
            {
                return System.SR.GetResourceString("JsonPolymorphismOptionsAssociatedWithDifferentJsonTypeInfo");
            }
        }

        // Token: 0x170000B9 RID: 185
        // (get) Token: 0x060000C1 RID: 193 RVA: 0x00002A82 File Offset: 0x00000C82
        internal static string JsonPropertyRequiredAndNotDeserializable
        {
            get
            {
                return System.SR.GetResourceString("JsonPropertyRequiredAndNotDeserializable");
            }
        }

        // Token: 0x170000BA RID: 186
        // (get) Token: 0x060000C2 RID: 194 RVA: 0x00002A8E File Offset: 0x00000C8E
        internal static string JsonPropertyRequiredAndExtensionData
        {
            get
            {
                return System.SR.GetResourceString("JsonPropertyRequiredAndExtensionData");
            }
        }

        // Token: 0x170000BB RID: 187
        // (get) Token: 0x060000C3 RID: 195 RVA: 0x00002A9A File Offset: 0x00000C9A
        internal static string JsonRequiredPropertiesMissing
        {
            get
            {
                return System.SR.GetResourceString("JsonRequiredPropertiesMissing");
            }
        }

        // Token: 0x170000BC RID: 188
        // (get) Token: 0x060000C4 RID: 196 RVA: 0x00002AA6 File Offset: 0x00000CA6
        internal static string ObjectCreationHandlingPopulateNotSupportedByConverter
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPopulateNotSupportedByConverter");
            }
        }

        // Token: 0x170000BD RID: 189
        // (get) Token: 0x060000C5 RID: 197 RVA: 0x00002AB2 File Offset: 0x00000CB2
        internal static string ObjectCreationHandlingPropertyMustHaveAGetter
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyMustHaveAGetter");
            }
        }

        // Token: 0x170000BE RID: 190
        // (get) Token: 0x060000C6 RID: 198 RVA: 0x00002ABE File Offset: 0x00000CBE
        internal static string ObjectCreationHandlingPropertyValueTypeMustHaveASetter
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyValueTypeMustHaveASetter");
            }
        }

        // Token: 0x170000BF RID: 191
        // (get) Token: 0x060000C7 RID: 199 RVA: 0x00002ACA File Offset: 0x00000CCA
        internal static string ObjectCreationHandlingPropertyCannotAllowPolymorphicDeserialization
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyCannotAllowPolymorphicDeserialization");
            }
        }

        // Token: 0x170000C0 RID: 192
        // (get) Token: 0x060000C8 RID: 200 RVA: 0x00002AD6 File Offset: 0x00000CD6
        internal static string ObjectCreationHandlingPropertyCannotAllowReadOnlyMember
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyCannotAllowReadOnlyMember");
            }
        }

        // Token: 0x170000C1 RID: 193
        // (get) Token: 0x060000C9 RID: 201 RVA: 0x00002AE2 File Offset: 0x00000CE2
        internal static string ObjectCreationHandlingPropertyCannotAllowReferenceHandling
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyCannotAllowReferenceHandling");
            }
        }

        // Token: 0x170000C2 RID: 194
        // (get) Token: 0x060000CA RID: 202 RVA: 0x00002AEE File Offset: 0x00000CEE
        internal static string ObjectCreationHandlingPropertyDoesNotSupportParameterizedConstructors
        {
            get
            {
                return System.SR.GetResourceString("ObjectCreationHandlingPropertyDoesNotSupportParameterizedConstructors");
            }
        }

        // Token: 0x170000C3 RID: 195
        // (get) Token: 0x060000CB RID: 203 RVA: 0x00002AFA File Offset: 0x00000CFA
        internal static string FormatInt128
        {
            get
            {
                return System.SR.GetResourceString("FormatInt128");
            }
        }

        // Token: 0x170000C4 RID: 196
        // (get) Token: 0x060000CC RID: 204 RVA: 0x00002B06 File Offset: 0x00000D06
        internal static string FormatUInt128
        {
            get
            {
                return System.SR.GetResourceString("FormatUInt128");
            }
        }

        // Token: 0x170000C5 RID: 197
        // (get) Token: 0x060000CD RID: 205 RVA: 0x00002B12 File Offset: 0x00000D12
        internal static string FormatHalf
        {
            get
            {
                return System.SR.GetResourceString("FormatHalf");
            }
        }

        // Token: 0x060000CE RID: 206 RVA: 0x00002B20 File Offset: 0x00000D20
        // Note: this type is marked as 'beforefieldinit'.
        static SR()
        {
            bool flag;
            System.SR.s_usingResourceKeys = AppContext.TryGetSwitch("System.Resources.UseSystemResourceKeys", out flag) && flag;
        }

        // Token: 0x04000001 RID: 1
        private static readonly bool s_usingResourceKeys;

    }
}
