namespace AsmResolver.PE.DotNet.ReadyToRun
{
    internal struct ReadyToRunHeaderConstants
    {
        public const uint Signature = 0x00525452; // 'RTR'

        public const ushort CurrentMajorVersion = 26;
        public const ushort CurrentMinorVersion = 0;
    }

    internal struct ReadyToRunHeader
    {
        private uint Signature;      // ReadyToRunHeaderConstants.Signature
        private ushort MajorVersion;
        private ushort MinorVersion;

        private uint Flags;

        private ushort NumberOfSections;
        private byte EntrySize;
        private byte EntryType;
    };
}
