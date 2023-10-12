using PaddleOCRJson.Attributes;

namespace PaddleOCRJson.Enums.StartupArgs;

public enum DetDbScoreModes
{
    [EnumRealString("slow")] Slow,
    [EnumRealString("fast")] Fast
}