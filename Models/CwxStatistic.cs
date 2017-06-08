using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// ステップ集合クラス
    /// </summary>
    public class CwxStatistic
    {

        private ObservableCollection<Statistics> _Statistics;
        public ObservableCollection<Statistics> Statistics
        {
            get { return _Statistics; }
            set
            {
                if (Equals(value, _Statistics)) return;
                _Statistics = value;

            }
        }

        public string ComponentName { get; set; }
        
        public int FileCount { get; set; }

        public int Sum_FactStep { get; set; }

        public int Sum_SpaceStep { get; set; }

        public int Sum_CommentStep { get; set; }

        public int Sum_SumStep { get; set; }

        public void Sum()
        {
            FileCount = this.Statistics.Count();
            Sum_FactStep = this.Statistics.Sum<Statistics>(m => m.FactStep);
            Sum_SpaceStep = this.Statistics.Sum<Statistics>(m => m.SpaceStep);
            Sum_CommentStep = this.Statistics.Sum<Statistics>(m => m.CommentStep);
            Sum_SumStep = this.Statistics.Sum<Statistics>(m => m.SumStep);

        }

    }
}
