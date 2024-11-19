namespace StoreService.Core.Extensions;

public static class CommonExtension
{
    public static string GetEnvPrefix(this string envName, string prefix)
    {
        switch (envName)
        {
            case "Development":
            case "LOCAL":
                return "D";

            case "SIT":
                return "S";

            case "UAT":
                return "U";

            case "PROD":
                return "P";

            default:
                return "D";
        }
    }
}