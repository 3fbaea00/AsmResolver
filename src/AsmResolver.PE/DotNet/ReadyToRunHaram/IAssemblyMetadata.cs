using System;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    /// <summary>
    /// This interface represents MSIL information for a single component assembly.
    /// </summary>
    public interface IAssemblyMetadata
    {
        void GetSectionData(int relativeVirtualAddress, Action<BlobReader> action);
        MetadataReader MetadataReader { get;  }
    }
}
