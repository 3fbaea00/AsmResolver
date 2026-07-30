namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    public enum GcInfoVersion : byte;

    public static class GcInfoVersionExtensions
    {
        public static bool HasReturnKind(this GcInfoVersion self) => (byte)self is >= 2 and <= 3;

        public static bool MayHaveReversePInvokeFrame(this GcInfoVersion self) => (byte)self >= 2;

        public static bool IsCodeOffsetsNormalized(this GcInfoVersion self) => (byte)self >= 3;

        public static uint GetHeaderFlagsBits(this GcInfoVersion self) => (byte)self == 1 ? 9U : 10U;
    }
}
