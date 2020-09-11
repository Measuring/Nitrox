using System.IO;

namespace NitroxModel.Discovery
{
    public static class PlatformDetection
    {
        public static bool IsEpic(string subnauticaPath)
        {
            return Directory.Exists(Path.Combine(subnauticaPath, ".egstore"));
        }

        public static bool IsMicrosoftStore(string subnauticaPath)
        {
            return File.Exists(Path.Combine(subnauticaPath, "appxmanifest.xml"));
        }
    }
}
