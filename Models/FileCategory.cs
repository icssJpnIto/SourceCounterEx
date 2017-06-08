using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    public class FileCategory
    {
        public bool Include { get; set; }

        public string Category { get; set; }

        public string FileTypes { get; set; }

        public string SingleLineComment { get; set; }

        public string MultilineCommentStart { get; set; }

        public string MultilineCommentEnd { get; set; }

        public string ChgStart { get; set; }

        public string ChgEnd { get; set; }

        public string NameExclusions { get; set; }

        public int TotalLines { get; set; }

        public int TotalFiles { get; set; }

        public int Code { get; set; }

        public int Comments { get; set; }

        public int Empty { get; set; }

        public int IncludedFiles { get; set; }

        public int ExcludedFiles { get; set; }
    }
}
