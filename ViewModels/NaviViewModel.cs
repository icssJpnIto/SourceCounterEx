using SourceCounterEx.Command;
using SourceCounterEx.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SourceCounterEx.ViewModels
{
    public class NaviViewModel : ViewModel
    {
        private MainViewModel _mainvm;

        public NaviViewModel(MainViewModel mainvm)
        {
            this._mainvm = mainvm;
            
        }

        #region HasComponets
        bool _HasComponets = false;
        public bool HasComponets
        {
            get { return this._HasComponets; }
            set
            {
                if (Equals(value, _HasComponets))
                {
                    return;
                }

                _HasComponets = value;
                OnPropertyChanged(nameof(HasComponets));
            }
        }
        #endregion

        #region CwxComponets
        private ObservableCollection<CwxComponent> CwxComponets_Inside = new ObservableCollection<CwxComponent>();
        public ObservableCollection<CwxComponent> CwxComponets_In
        {
            get { return this.CwxComponets_Inside; }
        }

        private ObservableCollection<CwxComponent> CwxComponets_Outside = new ObservableCollection<CwxComponent>();
        public ObservableCollection<CwxComponent> CwxComponets
        {
            get { return this.CwxComponets_Outside; }
        }

      
        #endregion

        #region SelectedItem
        Object _SelectedItem;
        public Object SelectedItem
        {
            get {
                return this._SelectedItem;
            }
            set
            {
                if (Equals(value, _SelectedItem))
                {
                    return;
                }

                _SelectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));

                //
                string componetName = string.Empty;

                if (this._SelectedItem is CwxComponent)
                {
                    componetName = (this._SelectedItem as CwxComponent).ComponentName;
                }
                if (this._SelectedItem is Change)
                {
                    componetName = (this._SelectedItem as Change).ComponentName;
                }
                this._mainvm.ContentViewModel.SetSelectedItem(componetName);
                
               

            }
        }
        #endregion

        #region Close One Componet
        RelayCommand _CloseComponet = null;

        public ICommand CloseComponet
        {
            get
            {
                if (_CloseComponet == null)
                {
                    _CloseComponet = new RelayCommand(() => OnCloseComponet(), () => { return SelectedItem!=null; } );
                }

                return _CloseComponet;
            }
        }
        #endregion

        private void OnCloseComponet()
        {
            string componetName =string.Empty;

            if(this._SelectedItem is CwxComponent)
            {
                componetName = (this._SelectedItem as CwxComponent).ComponentName;
            }
            if(this._SelectedItem is Change)
            {
                componetName = (this._SelectedItem as Change).ComponentName;
            }
          
            for(int i = this.CwxComponets.Count - 1; i >= 0; i--)
            {
                if (this.CwxComponets[i].ComponentName.Equals(componetName))
                {
                    this.CwxComponets.RemoveAt(i);
                    break;
                }
                
            }

            
            for (int i = this._mainvm.ContentViewModel.ComponentStatistics.Count - 1; i >= 0; i--)
            {
                if (this._mainvm.ContentViewModel.ComponentStatistics[i].ComponentName.Equals(componetName))
                {
                    this._mainvm.ContentViewModel.ComponentStatistics.RemoveAt(i);
                    break;
                }

            }

            for (int i = this._mainvm.ContentViewModel.CwxBlock.Count - 1; i >= 0; i--)
            {
                if (this._mainvm.ContentViewModel.CwxBlock[i].ComponentName.Equals(componetName))
                {
                    this._mainvm.ContentViewModel.CwxBlock.RemoveAt(i);
                    break;
                }

            }



        }

        /// <summary>
        /// 履歴ファイルを読み込む
        /// </summary>
        /// <param name="cwxComp"></param>
        /// <param name="sRevFile"></param>
        /// <param name="sRev"></param>
        public void ProcessPathRevFile(string cwxComp, string sRevFile, string sRevPrefix)
        {
            string[] allLines = null;
            this.CwxComponets_Inside.Add(new CwxComponent { ComponentName = cwxComp });

            if (!System.IO.File.Exists(sRevFile)) return;

            allLines = System.IO.File.ReadAllLines(sRevFile, Encoding.GetEncoding("Shift_JIS"));

            if (allLines != null)
            {

                this.CwxComponets_Inside.LastOrDefault<CwxComponent>().Changes = new ObservableCollection<Change>();

                for (int i = 0; i < allLines.Length; i++)
                {
                    if ((allLines[i].Trim().StartsWith(sRevPrefix, StringComparison.OrdinalIgnoreCase) ||
                        allLines[i].Trim().StartsWith("'"+sRevPrefix, StringComparison.OrdinalIgnoreCase) ||
                        allLines[i].Trim().StartsWith("//"+sRevPrefix, StringComparison.OrdinalIgnoreCase) ||
                        (allLines[i].Trim().StartsWith("//", StringComparison.OrdinalIgnoreCase) && 
                        allLines[i].Trim().Substring(2).Trim().StartsWith(sRevPrefix, StringComparison.OrdinalIgnoreCase)) ||
                        (allLines[i].Trim().StartsWith("'", StringComparison.OrdinalIgnoreCase) &&
                        allLines[i].Trim().Substring(1).Trim().StartsWith(sRevPrefix, StringComparison.OrdinalIgnoreCase))
                        ) &&
                        i + 4 < allLines.Length)
                    {

                        this.CwxComponets_Inside.LastOrDefault<CwxComponent>().Changes.Add(new Change()
                        {

                            No = allLines[i++].Replace("'","").Replace("//","").Trim(),
                            Row1 = allLines[i++].Replace('\t', ' '),
                            Row2 = allLines[i++].Replace('\t', ' '),
                            Row3 = allLines[i++].Replace('\t', ' '),
                            Row4 = allLines[i].Replace('\t', ' '),
                            ComponentName = cwxComp,

                        });

                    }
                }
            }


        }
        public void ProcessPathRevFileFinish()
        {

            foreach (CwxComponent item in this.CwxComponets_Inside)
            {
                this.CwxComponets_Outside.Add(item);
            }

            this.CwxComponets_Inside.Clear();

            if (this.CwxComponets_Outside.Count > 0)
            {
                this.HasComponets = true;
            }
        }


    }
}
