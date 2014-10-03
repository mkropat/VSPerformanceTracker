// Guids.cs
// MUST match guids.h

using System;

namespace VSPerformanceTracker
{
    static class GuidList
    {
        public const string guidVSPackage1PkgString = "fb877fc8-4c67-4ff4-a282-254700fd4d02";
        public const string guidVSPackage1CmdSetString = "7bf2efb7-daeb-4082-81fc-9eddcc42b7f4";

        public static readonly Guid guidVSPackage1CmdSet = new Guid(guidVSPackage1CmdSetString);
    };
}