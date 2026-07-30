using System.IO;

namespace AsmResolver.PE.DotNet.ReadyToRun.MachO;

internal static class MachMagicExtensions
{
    public static uint ConvertValue(this MachMagic magic, uint value)
    {
        return magic switch
        {
            MachMagic.MachHeader64CurrentEndian or MachMagic.MachHeaderCurrentEndian
                => value,
            MachMagic.MachHeader64OppositeEndian or MachMagic.MachHeaderOppositeEndian
                => BinaryPrimitives.ReverseEndianness(value),
            _ => throw new InvalidDataException($"Invalid magic value 0x{magic:X}")
        };
    }

    public static ulong ConvertValue(this MachMagic magic, ulong value)
    {
        return magic switch
        {
            MachMagic.MachHeader64CurrentEndian or MachMagic.MachHeaderCurrentEndian
                => value,
            MachMagic.MachHeader64OppositeEndian or MachMagic.MachHeaderOppositeEndian
                => BinaryPrimitives.ReverseEndianness(value),
            _ => throw new InvalidDataException($"Invalid magic value 0x{magic:X}")
        };
    }
}
