namespace AnySoftBackend.Helpers;

public static class EnumDescriptionHelper
{
    public static string GetDescription(this Enum genericEnum)
    {
        var genericEnumType = genericEnum.GetType();
        if (genericEnumType.GetMember(genericEnum.ToString()) is not {Length: > 0} memberInfo)
            return genericEnum.ToString();
        if ((memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false) is { } attribs && attribs.Any()))
        {
            return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
        }
        return genericEnum.ToString();
    }
}