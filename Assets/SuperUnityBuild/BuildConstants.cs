using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        Retail,
    }

    public enum Platform
    {
        None,
        PC,
        Linux,
        Android,
        WebGL,
    }

    public enum ScriptingBackend
    {
        None,
        Mono,
        IL2CPP,
    }

    public enum Architecture
    {
        None,
        Windows_x86,
        Linux_x64,
        Android,
        WebGL,
    }

    public enum Distribution
    {
        None,
    }

    public static class BuildConstants
    {
        public static readonly DateTime buildDate = new DateTime(638765910035169677);
        public const string version = "1.1.0.1";
        public const ReleaseType releaseType = ReleaseType.Retail;
        public const Platform platform = Platform.WebGL;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Architecture architecture = Architecture.WebGL;
        public const Distribution distribution = Distribution.None;
    }
}

