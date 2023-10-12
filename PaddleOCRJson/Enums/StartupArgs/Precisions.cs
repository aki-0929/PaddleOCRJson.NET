using PaddleOCRJson.Attributes;

namespace PaddleOCRJson.Enums.StartupArgs;

public enum Precisions
{
    [EnumRealString("fp8")] Fp8,
    [EnumRealString("fp16")] Fp16,
    [EnumRealString("fp32")] Fp32
}