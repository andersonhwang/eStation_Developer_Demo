using System.Windows.Markup;

namespace Demo_WPF.Extension
{
    /// <summary>
    /// Enumerate source extension
    /// </summary>
    internal class EnumSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get => _enumType;
            private set {
                if (value != _enumType && value != null)
                {
                    var enumType = Nullable.GetUnderlyingType(value) ?? value;
                    if (enumType.IsEnum == false)
                    {
                        throw new ArgumentException("Type must be for an Enum.");
                    }
                }
                _enumType = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EnumSourceExtension() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">Enum type</param>
        public EnumSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        /// <summary>
        /// Proveide value
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == _enumType)
            {
                throw new InvalidOperationException("Enum_Type_Null");
            }

            var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            var enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == _enumType)
            {
                return enumValues;
            }

            var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);

            return tempArray;
        }
    }
}
