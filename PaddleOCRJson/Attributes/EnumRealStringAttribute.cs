using System;

namespace PaddleOCRJson.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumRealStringAttribute : Attribute
{
    private readonly object _value;

    public EnumRealStringAttribute(object value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value?.ToString();
    }
}