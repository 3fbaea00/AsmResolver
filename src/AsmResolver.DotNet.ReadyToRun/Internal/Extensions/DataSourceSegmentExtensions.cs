using AsmResolver.IO;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Extensions
{
    internal static class DataSourceSegmentExtensions
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dataSource")]
        private static extern ref IDataSource DataSource(DataSourceSegment segment);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_displacedDataSource")]
        private static extern ref DisplacedDataSource? DisplacedDataSource(DataSourceSegment segment);

        public static ref IDataSource GetDataSource(this DataSourceSegment segment) => ref DataSource(segment);

        public static ref DisplacedDataSource? GetDisplacedDataSource(this DataSourceSegment segment) => ref DisplacedDataSource(segment);
    }
}