using System.Windows.Media.Imaging;
using System.IO;

namespace ManageInfo_Resources
{
    public class ResourceSound
    {
        // Gets the sound from the users assembly
        public static Stream GetSound(string name)
        {
            // Create the resource reader
            Stream stream = ResourceAssembly.GetAssembly().GetManifestResourceStream(ResourceAssembly.GetNamespace() + "Sounds." + name);

            return stream;
        }
    }
}
