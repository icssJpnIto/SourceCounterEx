using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// 変更ブロック集合クラス
    /// </summary>
    public class CwxBlock
    {

        private ObservableCollection<Block> _Blocks;
        

        public string ComponentName { get; set; }

        public ObservableCollection<Block> Blocks
        {
            get { return _Blocks; }
            set
            {
                if (Equals(value, _Blocks)) return;
                _Blocks = value;

            }
        }
        
    }
}
