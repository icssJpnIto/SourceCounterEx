using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// 変更履歴集合クラス
    /// </summary>
    public class CwxComponent 
    {
        
        private ObservableCollection<Change> _changes;

        public string ComponentName { get; set; }
   
        public ObservableCollection<Change> Changes
        {
            get { return _changes; }
            set
            {
                if (Equals(value, _changes)) return;
                _changes = value;
                
            }
        }


    }
}
