using AsmResolver.PE.File;
using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Amd64
{
    public struct InterruptibleRange
    {
        public uint Index { get; set; }
        public uint StartOffset { get; set; }
        public uint StopOffset { get; set; }
        public InterruptibleRange(uint index, uint start, uint stop)
        {
            Index = index;
            StartOffset = start;
            StopOffset = stop;
        }
    }

    public class GcTransition : BaseGcTransition
    {
        public int SlotId { get; set; }
        public bool IsLive { get; set; }
        public int ChunkId { get; set; }
        public string SlotState { get; set; }

        public GcTransition() { }

        public GcTransition(int codeOffset, int slotId, bool isLive, int chunkId, GcSlotTable slotTable, MachineType machine)
        {
            CodeOffset = codeOffset;
            SlotId = slotId;
            IsLive = isLive;
            ChunkId = chunkId;
            SlotState = GetSlotState(slotTable, machine);
        }

        public string GetSlotState(GcSlotTable slotTable, MachineType machine)
        {
            GcSlotTable.GcSlot slot = slotTable.GcSlots[SlotId];
            string slotStr = "";
            if (slot.StackSlot == null)
            {
                Type regType;
                switch (machine)
                {
                    case MachineType.Arm64:
                        regType = typeof(Arm64.Registers);
                        break;

                    case MachineType.Amd64:
                        regType = typeof(Amd64.Registers);
                        break;

                    case MachineType.LoongArch64:
                        regType = typeof(LoongArch64.Registers);
                        break;

                    case MachineType.RiscV64:
                        regType = typeof(RiscV64.Registers);
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
