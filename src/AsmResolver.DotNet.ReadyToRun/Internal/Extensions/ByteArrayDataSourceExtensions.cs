using AsmResolver.IO;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Extensions
{
    internal static class ByteArrayDataSourceExtensions
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_data")]
        private static extern ref byte[] Data(ByteArrayDataSource dataSource);

        public static byte[] GetByteArrayNoCopy(this ByteArrayDataSource dataSource)
        {
            return Data(dataSource);
        }
    }
}