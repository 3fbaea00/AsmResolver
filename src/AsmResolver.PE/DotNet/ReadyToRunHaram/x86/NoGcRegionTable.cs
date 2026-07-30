using System.Collections.Generic;

namespace AsmResolver.PE.DotNet.ReadyToRun.x86
{
    public class NoGcRegionTable
    {
        public class NoGcRegion
        {
            public uint Offset { get; set; }
            public uint Size { get; set; }

            public NoGcRegion(uint offset, uint size)
            {
                Offset = offset;
                Size = size;
            }
        }

        public List<NoGcRegion> Regions { get; set; }

        public NoGcRegionTable() { }

        public NoGcRegionTable(NativeReader imageReader, InfoHdrSmall header, ref int offset)
        {
            Regions = new List<NoGcRegion>((int)header.NoGCRegionCnt);

            uint count = header.NoGCRegionCnt;
            while (count-- > 0)
            {
                uint regionOffset = imageReader.DecodeUnsignedGc(ref offset);
                uint regionSize = imageReader.DecodeUnsignedGc(ref offset);
                Regions.Add(new NoGcRegion(regionOffset, regionSize));
            }
        }
    }
}
