using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;
using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Amd64
{
    public class GcTransition : BaseGcTransition
    {
        public int SlotId { get; set; }
        public bool IsLive { get; set; }
        public int ChunkId { get; set; }
        public string SlotState { get; set; }

        public GcTransition() { }

        public GcTransition(int codeOffset, int slotId, bool isLive, int chunkId, GcSlotTable slotTable, SupportedMachineType machine)
        {
            CodeOffset = codeOffset;
            SlotId = slotId;
            IsLive = isLive;
            ChunkId = chunkId;
            SlotState = GetSlotState(slotTable, machine);
        }

        public string GetSlotState(GcSlotTable slotTable, SupportedMachineType machine)
        {
            GcSlotTable.GcSlot slot = slotTable.GcSlots[SlotId];
            string slotStr = "";
            if (slot.StackSlot == null)
            {
                Type regType;
                switch (machine)
                {
                    case SupportedMachineType.Arm64:
                        regType = typeof(Arm64.Register);
                        break;

                    case SupportedMachineType.Amd64:
                        regType = typeof(Amd64.Register);
                        break;

                    case SupportedMachineType.LoongArch64:
                        regType = typeof(LoongArch64.Register);
                        break;

                    case SupportedMachineType.RiscV64:
                        regType = typeof(RiscV64.Register);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                slotStr = Enum.GetName(regType, slot.RegisterNumber);
            }
            else
            {
                slotStr = $"sp{slot.StackSlot.SpOffset:+#;-#;+0}";
            }
            string isLiveStr = "live";
            if (!IsLive)
                isLiveStr = "dead";
            return $"{slotStr} is {isLiveStr}";
        }
    }
}
