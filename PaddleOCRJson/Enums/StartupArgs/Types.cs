using PaddleOCRJson.Attributes;

namespace PaddleOCRJson.Enums.StartupArgs;

public enum Types
{
    [EnumRealString("ocr")] Ocr,
    [EnumRealString("structure")] Structure,
}