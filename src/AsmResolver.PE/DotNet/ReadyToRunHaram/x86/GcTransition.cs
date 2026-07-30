using System.Collections.Generic;

namespace AsmResolver.PE.DotNet.ReadyToRun.x86
{
    public enum Action
    {
        POP = 0x00,
        PUSH = 0x01,
        KILL = 0x02,
        LIVE = 0x03,
        DEAD = 0x04
    }

    public class CalleeSavedRegister : BaseGcTransition
    {
        public CalleeSavedRegistersI386 Register { get; set; }

        public CalleeSavedRegister() { }

        public CalleeSavedRegister(int codeOffset, CalleeSavedRegistersI386 reg)
            : base(codeOffset)
        {
            Register = reg;
        }
    }

    public class IPtrMask : BaseGcTransition
    {
        public uint IMask { get; set; }

        public IPtrMask() { }

        public IPtrMask(int codeOffset, uint imask)
            : base(codeOffset)
        {
            IMask = imask;
        }
    }

    public class GcTransitionRegister : BaseGcTransition
    {
        public RegistersI386 Register { get; set; }
        public Action IsLive { get; set; }
        public int PushCountOrPopSize { get; set; }
        public bool IsThis { get; set; }
        public bool Iptr { get; set; }

        public GcTransitionRegister() { }

        public GcTransitionRegister(int codeOffset, RegistersI386 reg, Action isLive, bool isThis = false, bool iptr = false, int pushCountOrPopSize = -1)
            : base(codeOffset)
        {
            Register = reg;
            IsLive = isLive;
            PushCountOrPopSize = pushCountOrPopSize;
        }
    }

    public class GcTransitionPointer : BaseGcTransition
    {
        private bool _isEbpFrame;
        public uint ArgOffset { get; set; }
        public uint ArgCount { get; set; }
        public Action Act { get; set; }
        public bool IsPtr { get; set; }
        public bool IsThis { get; set; }
        public bool Iptr { get; set; }

        public GcTransitionPointer() { }

        public GcTransitionPointer(int codeOffset, uint argOffs, uint argCnt, Action act, bool isEbpFrame, bool isThis = false, bool iptr = false, bool isPtr = true)
            : base(codeOffset)
        {
            _isEbpFrame = isEbpFrame;
            CodeOffset = codeOffset;
            ArgOffset = argOffs;
            ArgCount = argCnt;
            Act = act;
            IsPtr = isPtr;
        }
    }

    public class GcTransitionCall : BaseGcTransition
    {
        public struct CallRegister
        {
            public RegistersI386 Register { get; set; }
            public bool IsByRef { get; set; }

            public CallRegister(RegistersI386 reg, bool isByRef)
            {
                Register = reg;
                IsByRef = isByRef;
            }
        }

        public struct PtrArg
        {
            public uint StackOffset { get; set; }
            public uint LowBit { get; set; }

            public PtrArg(uint stackOffset, uint lowBit)
            {
                StackOffset = stackOffset;
                LowBit = lowBit;
            }
        }

        public List<CallRegister> CallRegisters { get; set; }
        public List<PtrArg> PtrArgs { get; set; }
        public uint ArgMask { get; set; }
        public uint IArgs { get; set; }

        public GcTransitionCall() { }

        public GcTransitionCall(int codeOffset)
            : base(codeOffset)
        {
            CallRegisters = new List<CallRegister>();
            PtrArgs = new List<PtrArg>();
            ArgMask = 0;
            IArgs = 0;
        }

        public GcTransitionCall(int codeOffset, bool isEbpFrame, uint regMask, uint byRefRegMask)
            : base(codeOffset)
        {
            CallRegisters = new List<CallRegister>();
            PtrArgs = new List<PtrArg>();
            if ((regMask & 1) != 0)
            {
                RegistersI386 reg = RegistersI386.EDI;
                bool isByRef = (byRefRegMask & 1) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if ((regMask & 2) != 0)
            {
                RegistersI386 reg = RegistersI386.ESI;
                bool isByRef = (byRefRegMask & 2) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if ((regMask & 4) != 0)
            {
                RegistersI386 reg = RegistersI386.EBX;
                bool isByRef = (byRefRegMask & 4) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if (!isEbpFrame)
            {
                if ((regMask & 8) != 0)
                {
                    RegistersI386 reg = RegistersI386.EBP;
                    CallRegisters.Add(new CallRegister(reg, false));
                }
            }
            ArgMask = 0;
            IArgs = 0;
        }
    }
}
