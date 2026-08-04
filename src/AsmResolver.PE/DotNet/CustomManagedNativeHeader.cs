using AsmResolver.IO;
using AsmResolver.PE.File;

namespace AsmResolver.PE.DotNet
{
    /// <summary>
    /// Represents a managed native header of a .NET Portable Executable that is in an unsupported or unknown file format.
    /// </summary>
    public class CustomManagedNativeHeader : IManagedNativeHeader
    {
        /// <summary>
        /// Creates a new custom managed native header.
        /// </summary>
        /// <param name="signature">The signature to use.</param>
        /// <param name="contents">The contents of the header, excluding the signature.</param>
        public CustomManagedNativeHeader(PEFile file, ManagedNativeHeaderSignature signature, ISegment contents)
        {
            File = file;
            Signature = signature;
            Contents = contents;
        }

        public PEFile File
        {
            get;
        }

        /// <inheritdoc />
        public ManagedNativeHeaderSignature Signature
        {
            get;
        }

        /// <inheritdoc />
        public ISegment Contents
        {
            get;
        }

        public bool CanUpdateOffsets => Contents.CanUpdateOffsets;

        public ulong Offset => Contents.Offset;

        public uint Rva => Contents.Rva;

        public uint GetPhysicalSize() => Contents.GetPhysicalSize();

        public uint GetVirtualSize() => Contents.GetVirtualSize();

        public void UpdateOffsets(in RelocationParameters parameters) => Contents.UpdateOffsets(in parameters);

        public void Write(BinaryStreamWriter writer) => Contents.Write(writer);
    }
}
