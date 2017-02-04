using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VSCoverageViewer
{
    /// <summary>
    /// Helper for <see cref="EnumSource"/>.
    /// </summary>
    internal class EnumMemberInfo
    {
        public object Value { get; set; }
        public string Display { get; set; }
    }

    /// <summary>
    /// A XAML markup extension which provides a collection containing the set of defined values
    /// for an enum type.
    /// </summary>
    internal class EnumSource : MarkupExtension
    {
        private Type _enumType;


        /// <summary>
        /// Gets or sets the enum type.
        /// </summary>
        [ConstructorArgument("enumType")]
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                Contract.RequiresNotNull(value, nameof(value));
                Contract.Requires(value.IsEnum, "Argument must be an enum type.", nameof(value));

                _enumType = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="EnumSource"/> which provides values from the
        /// given enum type.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        public EnumSource(Type enumType)
        {
            EnumType = enumType;
        }


        /// <summary>
        /// Gets the collection of values defined for the enum type. 
        /// </summary>
        /// <param name="serviceProvider">Unused.</param>
        /// <returns>A collection of values defined for the enum type.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType)
                       .OfType<object>()
                       .Where(ValueIsBindable)
                       .Select(v => new EnumMemberInfo
                       {
                           Value = v,
                           Display = GetDisplay(v),
                       })
                       .ToArray();
        }


        private bool ValueIsBindable(object enumValue)
        {
            var attr = _enumType
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(BindableAttribute), false)
                .FirstOrDefault() as BindableAttribute;

            return attr?.Bindable ?? true;
        }

        private string GetDisplay(object enumValue)
        {
            DescriptionAttribute attr = _enumType
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return attr?.Description ?? enumValue.ToString();
        }
    }
}
