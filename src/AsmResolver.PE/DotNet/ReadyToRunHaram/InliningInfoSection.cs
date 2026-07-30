namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public class InliningInfoSection
    {
        private readonly ReadyToRunReader _r2r;
        private readonly int _startOffset;
        private readonly int _endOffset;

        public InliningInfoSection(ReadyToRunReader reader, int offset, int endOffset)
        {
            _r2r = reader;
            _startOffset = offset;
            _endOffset = endOffset;
        }

        static int RidToMethodDef(int rid) => MetadataTokens.GetToken(MetadataTokens.MethodDefinitionHandle(rid));
    }
}
