using System;
using System.Collections.Generic;
using System.Text;
using PaddleOCRJson.Attributes;
using PaddleOCRJson.Enums;

namespace PaddleOCRJson.Extensions
{
    public static class EnumRealStringExtensions
    {
        public static string RealToString<T>(this T obj) where T : Enum
        {
            var origStr = obj.ToString();
            var field = obj.GetType().GetField(origStr);
            var realStringAttr =
                (EnumRealStringAttribute)Attribute.GetCustomAttribute(field, typeof(EnumRealStringAttribute));
            return realStringAttr?.ToString() ?? origStr;
        }
    }
}