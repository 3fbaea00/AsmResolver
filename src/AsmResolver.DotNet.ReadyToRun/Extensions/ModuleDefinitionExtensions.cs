using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.PE.DotNet;
using System;

namespace AsmResolver.DotNet.ReadyToRun.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        public static bool EnsureReadyToRunInitialized(this ModuleDefinition module)
        {
            var dotnetDirectory = module.DotNetDirectory;
            if (dotnetDirectory is null)
                return false;

            var managedNativeHeader = dotnetDirectory.ManagedNativeHeader;
            if (managedNativeHeader is null)
                return false;

            if (managedNativeHeader is CustomManagedNativeHeader customManagedNativeHeader)
            {
                managedNativeHeader = ReadyToRunDirectory.FromCustomManagedNativeHeader(customManagedNativeHeader);
                dotnetDirectory.ManagedNativeHeader = managedNativeHeader;

                return true;
            }

            return managedNativeHeader is ReadyToRunDirectory;
        }

        public static ReadyToRunDirectory GetOrCreateReadyToRunDirectory(this ModuleDefinition module)
        {
            var dotnetDirectory = module.DotNetDirectory;
            if (dotnetDirectory is null)
                throw new NullReferenceException();

            var managedNativeHeader = dotnetDirectory.ManagedNativeHeader;
            if (managedNativeHeader is ReadyToRunDirectory readyToRunDirectory)
                return readyToRunDirectory;

            if (!module.EnsureReadyToRunInitialized())
            {
                dotnetDirectory.ManagedNativeHeader = new ReadyToRunDirectory(
                    ReadyToRunAttributes.UnrelatedR2RCode | 
                    ReadyToRunAttributes.Partial | 
                    ReadyToRunAttributes.MultiModuleVersionBubble | 
                    ReadyToRunAttributes.NonSharedPInvokeStubs
                );
            }

            return (ReadyToRunDirectory)dotnetDirectory.ManagedNativeHeader;
        }
    }
}
