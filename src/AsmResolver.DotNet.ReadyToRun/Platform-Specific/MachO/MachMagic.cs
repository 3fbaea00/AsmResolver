using System.Buffers.Binary;
using System.IO;

namespace AsmResolver.DotNet.ReadyToRun.MachO
{
    // https://github.com/apple-oss-distributions/cctools/blob/main/tests/include/MachO/MachHeader.pm
    public enum MachMagic : uint
    {
        MachHeaderOppositeEndian   = 0xCEFAEDFE,
        MachHeaderCurrentEndian    = 0xFEEDFACE,
        MachHeader64OppositeEndian = 0xCFFAEDFE,
        MachHeader64CurrentEndian  = 0xFEEDFACF,
        FatMagicOppositeEndian     = 0xBEBAFECA,
        FatMagicCurrentEndian      = 0xCAFEBABE,
    }

    public static class MachMagicExtensions
    {
        public static uint ConvertValue(this MachMagic magic, uint value)
        {
            return magic switch
            {
                MachMagic.MachHeader64CurrentEndian or MachMagic.MachHeaderCurrentEndian => value,
                MachMagic.MachHeader64OppositeEndian or MachMagic.MachHeaderOppositeEndian => BinaryPrimitives.ReverseEndianness(value),
                _ => throw new InvalidDataException($"Invalid magic value 0x{magic:X}")
            };
        }

        public static ulong ConvertValue(this MachMagic magic, ulong value)
        {
            return magic switch
            {
                MachMagic.MachHeader64CurrentEndian or MachMagic.MachHeaderCurrentEndian => value,
                MachMagic.MachHeader64OppositeEndian or MachMagic.MachHeaderOppositeEndian => BinaryPrimitives.ReverseEndianness(value),
                _ => throw new InvalidDataException($"Invalid magic value 0x{magic:X}")
            };
        }
    }

}