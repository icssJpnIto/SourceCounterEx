using SourceCounterEx.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

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

        #region SelectedItem
        Object _SelectedItem;
        public Object SelectedItem
        {
            get
            {
                return this._SelectedItem;
            }
            set
            {
                if (Equals(value, _SelectedItem))
                {
                    return;
                }

                _SelectedItem = value;
                
            }
        }
        #endregion

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

        public override string ToString()
        {
            StringBuilder strBuf = new StringBuilder();
            strBuf.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"\r\n", "FileName", "Type", "FactStep", "SpaceStep", "CommentStep", "SumStep");

            this._Statistics.ToList().ForEach(item => strBuf.AppendLine(item.ToString()));

            strBuf.AppendLine(" [summary]");

            strBuf.AppendFormat("\"{0}\",,\"{1}\",\"{2}\",\"{3}\",\"{4}\"\r\n", "FileCount", "FactStep", "SpaceStep", "CommentStep", "SumStep");
            strBuf.AppendFormat("\"{0}\",,\"{1}\",\"{2}\",\"{3}\",\"{4}\"\r\n", FileCount, Sum_FactStep, Sum_SpaceStep, Sum_CommentStep, Sum_SumStep);

            return strBuf.ToString();

        }
        

        bool _Chk_FileCount = true;
        public bool Chk_FileCount
        {
            get { return this._Chk_FileCount; }
            set
            {
                if (Equals(value, _Chk_FileCount))
                {
                    return;
                }

                _Chk_FileCount = value;
                //OnPropertyChanged(nameof(VerNo));
            }
        }

       
        bool _Chk_Sum_FactStep = true;
        public bool Chk_Sum_FactStep
        {
            get { return this._Chk_Sum_FactStep; }
            set
            {
                if (Equals(value, _Chk_Sum_FactStep))
                {
                    return;
                }

                _Chk_Sum_FactStep = value;
                //OnPropertyChanged(nameof(VerNo));
            }
        }

        bool _Chk_Sum_CommentStept = true;
        public bool Chk_Sum_CommentStep
        {
            get { return this._Chk_Sum_CommentStept; }
            set
            {
                if (Equals(value, _Chk_Sum_CommentStept))
                {
                    return;
                }

                _Chk_Sum_CommentStept = value;
                //OnPropertyChanged(nameof(VerNo));
            }
        }

        bool _Chk_Sum_SpaceStep = true;
        public bool Chk_Sum_SpaceStep
        {
            get { return this._Chk_Sum_SpaceStep; }
            set
            {
                if (Equals(value, _Chk_Sum_SpaceStep))
                {
                    return;
                }

                _Chk_Sum_SpaceStep = value;
                //OnPropertyChanged(nameof(VerNo));
            }
        }

        bool _Chk_Sum_SumStep = true;
        public bool Chk_Sum_SumStep
        {
            get { return this._Chk_Sum_SumStep; }
            set
            {
                if (Equals(value, _Chk_Sum_SumStep))
                {
                    return;
                }

                _Chk_Sum_SumStep = value;
                //OnPropertyChanged(nameof(VerNo));
            }
        }

       public string GetSumString()
        {
            StringBuilder strBuf = new StringBuilder();
            if (this.Chk_FileCount)
            {
                strBuf.Append(string.Format("{0}\t", this.FileCount));
            }

            if (this.Chk_Sum_FactStep)
            {
                strBuf.Append(string.Format("{0}\t", this.Sum_FactStep));
            }

            if (this.Chk_Sum_CommentStep)
            {
                strBuf.Append(string.Format("{0}\t", this.Sum_CommentStep));
            }

            if (this.Chk_Sum_SpaceStep)
            {
                strBuf.Append(string.Format("{0}\t", this.Sum_SpaceStep));
            }

            if (this.Chk_Sum_SumStep)
            {
                strBuf.Append(string.Format("{0}\t", this.Sum_SumStep));
            }
            return strBuf.ToString();
        }
  

        #region Copy
        RelayCommand _copyCommand = null;

        public ICommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(() => OnCopy());

                }

                return _copyCommand;
            }
        }

        private void OnCopy()
        {
            
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => Clipboard.SetDataObject(GetSumString())));                 
           
        }

        #endregion

    }
}
