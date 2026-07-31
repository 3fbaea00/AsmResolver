using System;
using System.IO;

namespace AsmResolver.PE.DotNet.ReadyToRun.MachO;

/// <summary>
/// A managed object containing relevant information for AdHoc signing a Mach-O file.
/// The object is created from a memory mapped file, and a signature can be calculated from the memory mapped file.
/// However, since a memory mapped file cannot be extended, the signature is written to a file stream.
/// </summary>
internal unsafe partial class MachObjectFile
{
    public static bool IsMachOImage(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(System.IO.File.OpenRead(filePath)))
        {
            if (reader.BaseStream.Length < 256) // Header size
            {
                return false;
            }
            uint magic = reader.ReadUInt32();
            return Enum.IsDefined(typeof(MachMagic), magic);
        }
    }
}
