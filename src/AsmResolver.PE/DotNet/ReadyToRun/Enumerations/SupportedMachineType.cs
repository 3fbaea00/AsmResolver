using AsmResolver.PE.File;
using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{ 
    // This is mainly needed as an argument normalization for jumptable optimizations.
    /// <summary>
    /// Defines the machine types that are supported by this implementation of R2R.
    /// </summary>
    public enum SupportedMachineType : byte
    {
        I386,
        Amd64,
        Arm64,
        RiscV64,
        LoongArch64,
        // Arm32,
        // Wasm,
    }

    public static class SupportedMachineTypeExtensions
    {
        public static MachineType ToAllMachineType(this SupportedMachineType self)
            => self switch
            {
                SupportedMachineType.I386        => MachineType.I386,
                SupportedMachineType.Amd64       => MachineType.Amd64,
                SupportedMachineType.Arm64       => MachineType.Arm64,
                SupportedMachineType.RiscV64     => MachineType.LoongArch64,
                SupportedMachineType.LoongArch64 => MachineType.LoongArch64,
                _ => default
            };

        public static SupportedMachineType ToSupportedType(this MachineType self)
            => self switch
            {
                MachineType.I386        => SupportedMachineType.I386,
                MachineType.Amd64       => SupportedMachineType.Amd64,
                MachineType.Arm64       => SupportedMachineType.Arm64,
                MachineType.RiscV64     => SupportedMachineType.LoongArch64,
                MachineType.LoongArch64 => SupportedMachineType.LoongArch64,
                _ => throw new NotSupportedException()
            };
    }

}
