using System;

namespace NitroxModel.Platforms.Gaming.Exceptions
{
    public class PlatformException : Exception
    {
        public GamePlatform Platform { get; }

        public PlatformException(GamePlatform platform, string message) : base($"Steam: {message}")
        {
            Platform = platform;
        }

        public PlatformException(GamePlatform platform, string message, Exception innerException) : base($"Steam: {message}", innerException)
        {
            Platform = platform;
        }
    }
}
