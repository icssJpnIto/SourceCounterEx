using SourceCounterEx.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// 改修履歴No毎の情報
    /// </summary>
    public class ChangeItem
    {
        public ChangeItem(string No,double Percent,int lines,int factlines,bool bFirst=false)
        {
            this.No = No;
            this.Percent = Percent;
            this.Lines = lines;
            this.IsFirst = bFirst;
            this.factLines = factlines;
        }

        public bool IsFirst { get; set; }
        public string No { get; set; }
        public double Percent { get; set; }
        public int Lines { get; set; }
        public int factLines { get; set; }
        public string LinesText {

            get {

                return string.Format("{0}({1})",this.factLines, this.Lines);
            }
        }

       
        private ObservableCollection<LineItem> _LineItemData;
        public ObservableCollection<LineItem> LineItemData
        {
            get { return _LineItemData; }
            set
            {
                if (Equals(value, _LineItemData)) return;
                _LineItemData = value;

            }
        }

    }

    public class LineItem
    {
        public LineItem(int row, string Content,int isFileName = 0)
        {
            this.Row = row;
            this.Content = Content;
            this.IsFileName = isFileName;
        }
        public int  Row { get; set; }
        public string Content { get; set; }
        public int IsFileName { get; set; }

        public string RowDisp
        {

            get
            {
                if (this.IsFileName==1)
                {
                    return this.Content;
                }
                else if(this.IsFileName == 0)
                {
                    return string.Format("{0}", this.Row);
                }
                else
                {
                    return string.Empty;
                }
                
            }
        }

        public bool HasContent
        {

            get
            {
                if (this.IsFileName == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }

        public bool IsBlockStart
        {

            get
            {
                if(this.Row==0 && string.IsNullOrWhiteSpace(this.Content))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


    }
    
}
