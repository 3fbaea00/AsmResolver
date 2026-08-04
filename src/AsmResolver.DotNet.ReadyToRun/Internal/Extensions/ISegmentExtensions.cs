using AsmResolver.IO;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Extensions
{
    internal static class ISegmentExtensions
    {
        public static byte[] GetData(this ISegment segment, ref ulong offset, uint rvaToCheck, uint lengthToCheck)
        {
            if (segment is DataSegment dataSegment)
            {
                return dataSegment.Data;
            }
            else if (segment is VirtualSegment virtualSegment)
            {
                segment = virtualSegment.PhysicalContents;

                var leftBytes = segment.Rva + segment.GetPhysicalSize() - rvaToCheck;
                if (leftBytes > lengthToCheck)
                    return segment.GetData(ref offset, rvaToCheck, lengthToCheck); 
            }
            else if (segment is DataSourceSegment dataSourceSegment)
            {
                var displacedDataSoure = dataSourceSegment.GetDisplacedDataSource();
                if (displacedDataSoure is null)
                {
                    var dataSource = dataSourceSegment.GetDataSource();
                    if (dataSource is ByteArrayDataSource byteArrayDataSource)
                    {
                        var leftBytes = segment.Rva + segment.GetPhysicalSize() - rvaToCheck;
                        if (leftBytes > lengthToCheck)
                        {
                            offset = dataSourceSegment.Offset;
                            return byteArrayDataSource.GetByteArrayNoCopy();
                        }
                    }
                }
            }

            return segment.WriteIntoArray();
        }

        public static byte[] GetDataNoChecks(this ISegment segment, ref ulong offset)
        {
            if (segment is DataSegment dataSegment)
            {
                return dataSegment.Data;
            }
            else if (segment is VirtualSegment virtualSegment)
            {
                return virtualSegment.PhysicalContents.GetDataNoChecks(ref offset);
            }
            else if (segment is DataSourceSegment dataSourceSegment)
            {
                if (dataSourceSegment.GetDisplacedDataSource() is null)
                {
                    if (dataSourceSegment.GetDataSource() is ByteArrayDataSource byteArrayDataSource)
                    {
                        offset = dataSourceSegment.Offset;
                        return byteArrayDataSource.GetByteArrayNoCopy();
                    }
                }
            }

            return segment.WriteIntoArray();
        }
    }
}
