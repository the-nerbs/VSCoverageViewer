using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VSCoverageViewer
{
    class EnumMemberInfo
    {
        public object Value { get; set; }
        public string Display { get; set; }
    }

    class EnumSource : MarkupExtension
    {
        private Type _enumType;


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


        public EnumSource(Type enumType)
        {
            EnumType = enumType;
        }


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
