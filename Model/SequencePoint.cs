using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCoverageAnalyzer.Model
{
    public class SequencePoint
    {
        public int Line { get; set; }
        public int VisitCount { get; set; }
    }

}
