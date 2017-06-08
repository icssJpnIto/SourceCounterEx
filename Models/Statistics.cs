using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// ステップデータクラス
    /// </summary>
    public class Statistics
    {

        public string FileName { get; set; }

        public string Type { get; set; }

        public int FactStep { get; set; }

        public int SpaceStep { get; set; }

        public int CommentStep { get; set; }

        public int SumStep { get; set; }

    }
}
