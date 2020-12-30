using System.Collections.Generic;

namespace NitroxModel.Platforms.OS.Unix
{
    public sealed class UnixFileSystem : FileSystem
    {
        public override IEnumerable<string> GetDefaultPrograms(string file)
        {
            yield return "xdg-open";
        }
    }
}
