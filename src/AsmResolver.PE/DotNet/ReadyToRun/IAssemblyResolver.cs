namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public interface IAssemblyResolver
    {
        IAssemblyMetadata FindAssembly(MetadataReader metadataReader, AssemblyReferenceHandle assemblyReferenceHandle, string parentFile);
        IAssemblyMetadata FindAssembly(string simpleName, string parentFile);
    }

    public class SignatureFormattingOptions
    {
        public bool Naked { get; set; }
        public bool SignatureBinary { get; set;  }
        public bool InlineSignatureBinary { get; set; }
    }
}
