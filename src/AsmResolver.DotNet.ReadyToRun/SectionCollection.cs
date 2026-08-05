using AsmResolver.DotNet.Signatures;
using System.Collections.Generic;

namespace AsmResolver.DotNet.ReadyToRun
{
    public class SectionCollection<TSection>
        where TSection : IReadyToRunAssemblySection, new()
    {
        List<TSection> sections;

        public SectionCollection()
        {
            sections = new List<TSection>();
        }

        public IEnumerable<TSection> Sections => sections;

        public TSection? GetSectionForAssembly(AssemblyDescriptor assemblyDescriptor)
        {
            foreach (var section in sections)
            {
                var targetAssembly = section.TargetAssembly;
                if (SignatureComparer.Default.Equals(targetAssembly, assemblyDescriptor))
                    return section;
            }

            return default;
        }

        public TSection GetOrCreateSectionForAssembly(AssemblyDescriptor assemblyDescriptor)
        {
            var section = GetSectionForAssembly(assemblyDescriptor);
            if (section is null)
            {
                section = new TSection()
                {
                    TargetAssembly = assemblyDescriptor,
                };

                sections.Add(section);
            }

            return section;
        }
    }
}