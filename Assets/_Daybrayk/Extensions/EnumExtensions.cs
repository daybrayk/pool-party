using System.ComponentModel;
namespace Daybrayk
{
    public static class EnumExtensions
    {
	    public static string GetDescription(this System.Enum value)
        {
            var type = value.GetType();

            var name = System.Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if(attr != null) return attr.Description;
                }
            }
            return string.Empty;
        }
    }
}