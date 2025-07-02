using System.Collections.Generic;
using System.Reflection.Metadata;

namespace CodeCoverageAnalyzer.Model
{
    

    public class CoverageFile
    {
        public string FullPath { get; set; } = string.Empty;
        public List<SequencePoint> SequencePoints { get; set; } = new();
    }

}
