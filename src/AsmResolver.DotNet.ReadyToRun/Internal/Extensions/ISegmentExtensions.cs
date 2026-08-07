using AsmResolver.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Extensions
{
    internal unsafe static class ISegmentExtensions
    {
        // generilize
        public static ref byte GetData(this ISegment segment, out nuint length)
        {
            if (segment is DataSegment dataSegment)
            {
                var offset = dataSegment.Offset;
                var bytes = dataSegment.Data;
                length = (nuint)((uint)bytes.Length - offset);
                return ref bytes[offset];
            }
            else if (segment is VirtualSegment virtualSegment)
            {
                ref byte data = ref virtualSegment.PhysicalContents.GetData(out var length_);
                length = length_;
                return ref data;
            }
            else if (segment is DataSourceSegment dataSourceSegment)
            {
                if (dataSourceSegment.GetDisplacedDataSource() is null)
                {
                    if (dataSourceSegment.GetDataSource() is ByteArrayDataSource byteArrayDataSource)
                    {
                        var offset = dataSourceSegment.Offset;
                        var bytes = byteArrayDataSource.GetByteArrayNoCopy();
                        length = (nuint)((uint)bytes.Length - offset); // real size is DataSourceSegment._originalSize
                        return ref bytes[offset];
                    }
                }
            }

            {
                var offset = segment.Offset;
                var bytes = segment.WriteIntoArray();
                length = (nuint)((uint)bytes.Length - offset);
                return ref bytes[offset];
            }
        }
    }
}
