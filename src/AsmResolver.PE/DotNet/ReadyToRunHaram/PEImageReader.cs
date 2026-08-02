using AsmResolver.PE.File;
using System;
using System.Collections.Generic;
using System.IO;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    /// <summary>
    /// Wrapper around PEReader that implements IBinaryImageReader
    /// </summary>
    public class PEImageReader : IBinaryImageReader
    {
        private readonly PEReader _peReader;

        public MachineType Machine { get; }
        public OperatingSystem OperatingSystem { get; }
        public ulong ImageBase => _peReader.PEHeaders.PEHeader.ImageBase;

        public PEImageReader(PEReader peReader)
        {
            _peReader = peReader;

            // Extract machine and OS from PE header
            // The OS is encoded in the machine type
            uint rawMachine = (uint)_peReader.PEHeaders.CoffHeader.Machine;
            OperatingSystem = OperatingSystem.Unknown;

            foreach (OperatingSystem os in System.Enum.GetValues(typeof(OperatingSystem)))
            {
                MachineType candidateMachine = (MachineType)(rawMachine ^ (uint)os);
                if (System.Enum.IsDefined(typeof(MachineType), candidateMachine))
                {
                    Machine = candidateMachine;
                    OperatingSystem = os;
                    break;
                }
            }

            if (OperatingSystem == OperatingSystem.Unknown)
            {
                throw new BadImageFormatException($"Invalid PE Machine type: {rawMachine}");
            }
        }

        public byte[] GetEntireImage() => _peReader.GetEntireImage().GetContent();

        public int GetOffset(int rva) => _peReader.GetOffset(rva);

        public bool TryGetReadyToRunHeader(out int rva, out bool isComposite)
        {
            if ((_peReader.PEHeaders.CorHeader.Flags & DotNetDirectoryFlags.ILLibrary) == 0)
            {
                // Composite R2R - check for RTR_HEADER export
                if (_peReader.TryGetCompositeReadyToRunHeader(out rva))
                {
                    isComposite = true;
                    return true;
                }
            }
            else
            {
                var r2rHeaderDirectory = _peReader.PEHeaders.CorHeader.ManagedNativeHeaderDirectory;
                if (r2rHeaderDirectory.Size != 0)
                {
                    rva = r2rHeaderDirectory.RelativeVirtualAddress;
                    isComposite = false;
                    return true;
                }
            }

            rva = 0;
            isComposite = false;
            return false;
        }

        public IAssemblyMetadata GetStandaloneAssemblyMetadata()
            => _peReader.HasMetadata ? new StandaloneAssemblyMetadata(_peReader) : null;

        public IAssemblyMetadata GetManifestAssemblyMetadata(System.Reflection.Metadata.MetadataReader manifestReader)
            => new ManifestAssemblyMetadata(_peReader, manifestReader);

        public Dictionary<string, int> GetSections()
        {
            Dictionary<string, int> sectionMap = [];
            foreach (SectionHeader sectionHeader in _peReader.PEHeaders.SectionHeaders)
            {
                sectionMap.Add(sectionHeader.Name, (int) sectionHeader.SizeOfRawData);
            }

            return sectionMap;
        }
    }
}
