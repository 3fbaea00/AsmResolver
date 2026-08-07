using AsmResolver.PE.DotNet;
using System.Runtime.InteropServices.JavaScript;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    internal struct READYTORUN_HEADER
    {
        public ManagedNativeHeaderSignature Signature; // READYTORUN_SIGNATURE
        public ushort MajorVersion; // READYTORUN_VERSION_XXX
        public ushort MinorVersion;

        public READYTORUN_CORE_HEADER CoreHeader;
    }
}
