namespace StoreService.Core.Extensions;

public static class CommonExtension
{
    public static string GetEnvPrefix(this string envName, string prefix)
    {
        if (envName == "Development" || envName == "Local")
        {
            return prefix + "D";
        }

        return prefix + "P";
    }
}