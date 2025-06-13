using System;
using UnityEngine;

namespace jp.zabaglione.ui.core.binding
{
    /// <summary>
    /// Base interface for value converters
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Converts a value from source to target type
        /// </summary>
        object Convert(object value, Type targetType, object parameter);

        /// <summary>
        /// Converts a value from target back to source type
        /// </summary>
        object ConvertBack(object value, Type targetType, object parameter);
    }

    /// <summary>
    /// Base class for value converters
    /// </summary>
    public abstract class ValueConverterBase : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter);

        public virtual object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException($"{GetType().Name} does not support ConvertBack");
        }
    }

    /// <summary>
    /// Converts boolean values to visibility
    /// </summary>
    public class BoolToVisibilityConverter : ValueConverterBase
    {
        public bool InvertVisibility { get; set; }

        public override object Convert(object value, Type targetType, object parameter)
        {
            if (value is bool boolValue)
            {
                return InvertVisibility ? !boolValue : boolValue;
            }
            return false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (value is bool boolValue)
            {
                return InvertVisibility ? !boolValue : boolValue;
            }
            return false;
        }
    }

    /// <summary>
    /// Converts numeric values to formatted strings
    /// </summary>
    public class NumberToStringConverter : ValueConverterBase
    {
        public string Format { get; set; } = "0";

        public override object Convert(object value, Type targetType, object parameter)
        {
            string format = parameter as string ?? Format;

            return value switch
            {
                int intValue => intValue.ToString(format),
                float floatValue => floatValue.ToString(format),
                double doubleValue => doubleValue.ToString(format),
                decimal decimalValue => decimalValue.ToString(format),
                _ => value?.ToString() ?? string.Empty
            };
        }

        public override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (value is string stringValue)
            {
                if (targetType == typeof(int) && int.TryParse(stringValue, out int intResult))
                    return intResult;
                if (targetType == typeof(float) && float.TryParse(stringValue, out float floatResult))
                    return floatResult;
                if (targetType == typeof(double) && double.TryParse(stringValue, out double doubleResult))
                    return doubleResult;
                if (targetType == typeof(decimal) && decimal.TryParse(stringValue, out decimal decimalResult))
                    return decimalResult;
            }
            return null;
        }
    }

    /// <summary>
    /// Converts color values to hex strings
    /// </summary>
    public class ColorToHexConverter : ValueConverterBase
    {
        public bool IncludeAlpha { get; set; } = true;

        public override object Convert(object value, Type targetType, object parameter)
        {
            if (value is Color color)
            {
                return IncludeAlpha 
                    ? ColorUtility.ToHtmlStringRGBA(color) 
                    : ColorUtility.ToHtmlStringRGB(color);
            }
            return "#000000";
        }

        public override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (value is string hexString)
            {
                if (ColorUtility.TryParseHtmlString(hexString, out Color color))
                {
                    return color;
                }
            }
            return Color.black;
        }
    }

    /// <summary>
    /// Inverts boolean values
    /// </summary>
    public class InverseBooleanConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }

    /// <summary>
    /// Converts enum values to strings
    /// </summary>
    public class EnumToStringConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            if (value != null && value.GetType().IsEnum)
            {
                return value.ToString();
            }
            return string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (value is string stringValue && targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, stringValue);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Chains multiple converters together
    /// </summary>
    public class ChainedConverter : IValueConverter
    {
        public IValueConverter[] Converters { get; set; }

        public object Convert(object value, Type targetType, object parameter)
        {
            if (Converters == null || Converters.Length == 0)
                return value;

            object result = value;
            for (int i = 0; i < Converters.Length; i++)
            {
                result = Converters[i].Convert(result, i == Converters.Length - 1 ? targetType : typeof(object), parameter);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            if (Converters == null || Converters.Length == 0)
                return value;

            object result = value;
            for (int i = Converters.Length - 1; i >= 0; i--)
            {
                result = Converters[i].ConvertBack(result, i == 0 ? targetType : typeof(object), parameter);
            }
            return result;
        }
    }
}