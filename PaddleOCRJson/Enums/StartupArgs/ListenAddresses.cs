using PaddleOCRJson.Attributes;

namespace PaddleOCRJson.Enums.StartupArgs;

public enum ListenAddresses
{
    [EnumRealString("loopback")] Loopback,
    [EnumRealString("any")] Any
}