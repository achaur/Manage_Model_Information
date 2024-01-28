using System.Reflection;

namespace ManageInfo_Core
{
    // The core assembly helper methods
    public static class CoreAssembly
    {
        public static string GetAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }
    }
}