using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCounterEx.Models;
using System.Collections.ObjectModel;

namespace SourceCounterEx.ViewModels
{
    public class ReportViewModel : ViewModel
    {
        public const int PERCENT_LENGTH = 400;
        private MainViewModel _mainvm;

        public ReportViewModel(MainViewModel mainvm)
        {
            this._mainvm = mainvm;

            //this._ChangeItems.Add(new ChangeItem("PKG6_003", 0.3*400,300));
            //this._ChangeItems.Add(new ChangeItem("PKG6_004", 0.2*400, 200));
            //this._ChangeItems.Add(new ChangeItem("PKG6_005", 0.4*400, 400));
            //this._ChangeItems.Add(new ChangeItem("PKG6_006", 0,0));

            //this._ComponentName = "<工事中>";

        }

        #region ChangeItems
        private ObservableCollection<ChangeItem> _ChangeItems = new ObservableCollection<ChangeItem>();
        public ObservableCollection<ChangeItem> ChangeItems
        {
            get { return this._ChangeItems; }
        }

        #endregion

        #region ComponentName
        string _ComponentName= null;
        public string ComponentName
        {
            get { return this._ComponentName; }
            set
            {
                if (Equals(value, _ComponentName))
                {
                    return;
                }

                _ComponentName = value;
                OnPropertyChanged(nameof(ComponentName));
            }
        }

        bool _hasReport = false;
        public bool HasReport
        {
            get { return this._hasReport; }
            set
            {
                if (Equals(value, _hasReport))
                {
                    return;
                }

                _hasReport = value;
                OnPropertyChanged(nameof(HasReport));
            }
        }

        #endregion

        public void SetReportData(CwxComponent cwxComponent ,CwxBlock cwxBlock,int sumStep,int factStemp)
        {
            this._ChangeItems.Clear();
            this.ComponentName = null;
            this.HasReport = false;

            if (cwxComponent == null) return;
            if (cwxComponent.Changes == null) return;
            if (cwxBlock == null) return;

            int sum = 0;
            int factsum = 0;
            ChangeItem chgitem = null;
            int icnt = 0;

            foreach (Change item in cwxComponent.Changes)
            {
                
                sum = cwxBlock.Blocks.Where(b => b.ChangeName.Equals(item.No)).Sum(blk => blk.Code);
                factsum = cwxBlock.Blocks.Where(b => b.ChangeName.Equals(item.No)).Sum(blk => blk.FactStep);

                if (icnt == 0)
                {
                    chgitem = new ChangeItem(item.No, (double)sum / sumStep * PERCENT_LENGTH, sum, factsum, true);
                }
                else
                {
                    chgitem = new ChangeItem(item.No, (double)sum / sumStep * PERCENT_LENGTH, sum, factsum);
                }
                    

                chgitem.LineItemData = GetLineItemData(cwxBlock.Blocks.Where(b => b.ChangeName.Equals(item.No)).Select(b=>b));
                
                this._ChangeItems.Add(chgitem);

                

                icnt++;

            }

            this.HasReport = true;
            this.ComponentName = cwxBlock.ComponentName;
            

        }

        private ObservableCollection<LineItem> GetLineItemData(IEnumerable<Block> blocks)
        {

            ObservableCollection<LineItem> LineItemData= new ObservableCollection<LineItem>();
            int startrow = 0;
            string[] limit = { "\r\n" };
            int count = 0;

            foreach (string filenam in blocks.Select(blk => blk.FileName).Distinct())
            {
                LineItemData.Add(new LineItem(0, filenam, 1));

                count = 0;
                foreach (Block item in blocks.Where(b=>b.FileName.Equals(filenam)).Select(b=> b))
                {
                    startrow = item.StartRow;
                    if(count>0)
                    {
                        LineItemData.Add(new LineItem(0, string.Empty, -1));
                    }
                    
                    foreach (string sline in item.Content.Split(limit, StringSplitOptions.None))
                    {
                        LineItemData.Add(new LineItem(startrow, sline, 0));
                        startrow++;
                    }

                    count++;

                }
            }
            return LineItemData;

        }

    }

}
