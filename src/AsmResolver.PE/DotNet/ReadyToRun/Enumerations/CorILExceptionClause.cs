using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    /// <summary>
    /// If COR_ILMETHOD_SECT_HEADER::Kind() = CorILMethod_Sect_EHTable then the attribute
    /// is a list of exception handling clauses.  There are two formats, fat or small
    /// </summary>
    [Flags]
    public enum CorILExceptionClause
    {
        None,                          // This is a typed handler
        OffSetLen = 0x0000,            // Deprecated
        Deprecated = 0x0000,           // Deprecated
        Filter = 0x0001,               // If this bit is on, then this EH entry is for a filter
        Finally = 0x0002,              // This clause is a finally clause
        Fault = 0x0004,                // Fault clause (finally that is called on exception only)
        Duplicated = 0x0008,           // duplicated clause. This clause was duplicated to a funclet which was pulled out of line
        Sametry = 0x0010,              // This clause covers same try block as the previous one
        R2RSystemException = 0x0020,   // R2R only: This clause catches System.Exception

        KindMask = Filter | Finally | Fault,
    }
}