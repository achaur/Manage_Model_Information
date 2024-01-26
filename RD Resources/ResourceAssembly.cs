using System.Reflection;

namespace ManageInfo_Resources
{
    public static class ResourceAssembly
    {
        public static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
        public static string GetNamespace()
        {
            return typeof(ResourceAssembly).Namespace + ".";
        }
    }
}