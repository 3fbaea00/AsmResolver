using System;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    /// <summary>
    /// Metadata access interface for standalone assemblies represented by MSIL PE files.
    /// </summary>
    public class StandaloneAssemblyMetadata : IAssemblyMetadata
    {
        /// <summary>
        /// Reader representing the MSIL assembly file.
        /// </summary>
        private readonly PEReader _peReader;

        /// <summary>
        /// Metadata reader for the MSIL assembly. We create one upfront to avoid going
        /// through the GetMetadataReader() helper and constructing a new instance every time.
        /// </summary>
        private readonly MetadataReader _metadataReader;

        public StandaloneAssemblyMetadata(PEReader peReader)
        {
            _peReader = peReader;
            _metadataReader = _peReader.GetMetadataReader();
        }

        public void GetSectionData(int relativeVirtualAddress, Action<BlobReader> action) => action(_peReader.GetSectionData(relativeVirtualAddress).GetReader());

        public MetadataReader MetadataReader => _metadataReader;
    }
}
