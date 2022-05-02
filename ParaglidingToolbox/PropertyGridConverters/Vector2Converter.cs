using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.PropertyGridConverters
{
    public class Vector2Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            try
            {
                string[] tokens = ((string)value).Split(';');
                return new Vector2(float.Parse(tokens[0]), float.Parse(tokens[1]));
            }
            catch
            {
                if (context != null)
                {
                    return context.PropertyDescriptor.GetValue(context.Instance);
                }
                else return null!;
            }
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value != null)
            {
                Vector2 p = (Vector2)value;
                return $"{p.X:n4}; {p.Y:n4}";
            }
            return "";
        }
    }
}
