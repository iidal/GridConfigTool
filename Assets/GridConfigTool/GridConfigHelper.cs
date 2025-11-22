using System;

public static class GridConfigHelper
{

    public static bool GetCustomField(ref GridSO.ConfigurableField customField, GridSO gridConfig, string fieldName)
    {
        foreach (var field in gridConfig.customFields)
        {
            if (field.fieldName == fieldName)
            {
                customField = field;
                return true;
            }
        }
        return false;
    }
    public static int ParseInt(string value)
    {
        if (int.TryParse(value, out int intValue))
        {
            return intValue;
        }
        throw new System.InvalidCastException($"Cannot convert '{value}' to type integer");
    }
    public static float ParseFloat(string value)
    {
        if (float.TryParse(value, out float floatValue))
        {
            return floatValue;
        }
        throw new System.InvalidCastException($"Cannot convert '{value}' to type float");

    }
    public static bool ParseBool(string value)
    {
        if (bool.TryParse(value, out bool boolValue))
        {
            return boolValue;
        }
        throw new System.InvalidCastException($"Cannot convert '{value}' to type boolean");
    }
    public static string ParseString(string value)
    {
        return value;
    }
}
