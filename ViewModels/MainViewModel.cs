using SourceCounterEx.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using SourceCounterEx.Models;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using SourceCounterEx.View;

namespace SourceCounterEx.ViewModels
{
    public class MainViewModel : ViewModel
    {

        private readonly IDialogCoordinator _dialogCoordinator;
        private String sLastedForlder;
        private MainWindow _mainview;
        public MainViewModel(IDialogCoordinator dialogCoordinator, MainWindow mainview)
        {
            _dialogCoordinator = dialogCoordinator;
            this.Title = string.Format("SourceCounterEx@Metro {0}", "1.0");
            this.TickerViewModel = new TickerViewModel();
            this._mainview = mainview;

        }


        public string Title { get; private set; }

        #region VerNo
        string _VerNo = "ANN";
        public string VerNo
        {
            get { return this._VerNo; }
            set
            {
                if (Equals(value, _VerNo))
                {
                    return;
                }

                _VerNo = value;
                OnPropertyChanged(nameof(VerNo));
            }
        }
        #endregion

        #region VerNoPrefixSuffix
        string _VerNoPrefixSuffix = "[ANN1_,_ANN";
        public string VerNoPrefixSuffix
        {
            get { return this._VerNoPrefixSuffix; }
            set
            {
                if (Equals(value, _VerNoPrefixSuffix))
                {
                    return;
                }

                _VerNoPrefixSuffix = value;
                OnPropertyChanged(nameof(VerNoPrefixSuffix));
            }
        }
        #endregion

        #region IsAll
        bool _IsAll = true;
        public bool IsAll
        {
            get { return this._IsAll; }
            set
            {
                if (Equals(value, IsAll))
                {
                    return;
                }

                _IsAll = value;
                OnPropertyChanged(nameof(IsAll));
                if (_IsAll)
                {
                    this._mainview.IsReportVisible = false;
                }        
            }
        }
        #endregion

        #region IsBugChg
        bool _IsBugChg = true;
        public bool IsBugChg
        {
            get { return this._IsBugChg; }
            set
            {
                if (Equals(value, IsBugChg))
                {
                    return;
                }

                _IsBugChg = value;
                OnPropertyChanged(nameof(IsBugChg));
            }
        }
        #endregion

        #region IsNewChg
        bool _IsNewChg = true;
        public bool IsNewChg
        {
            get { return this._IsNewChg; }
            set
            {
                if (Equals(value, IsNewChg))
                {
                    return;
                }

                _IsNewChg = value;
                OnPropertyChanged(nameof(IsNewChg));
            }
        }
        #endregion

        #region IsProgress
        bool _IsProgress = false;
        public bool IsProgress
        {
            get { return this._IsProgress; }
            set
            {
                if (Equals(value, _IsProgress))
                {
                    return;
                }

                _IsProgress = value;
                OnPropertyChanged(nameof(IsProgress));
            }
        }
        #endregion

        #region IsRevScan
        bool _IsRevScan = true;
        public bool IsRevScan
        {
            get { return this._IsRevScan; }
            set
            {
                if (Equals(value, _IsRevScan))
                {
                    return;
                }

                _IsRevScan = value;
                OnPropertyChanged(nameof(IsRevScan));
            }
        }
        #endregion

        #region RevSetFileName
        string _RevSetFileName = "";
        public string RevSetFileName
        {
            get { return this._RevSetFileName; }
            set
            {
                if (Equals(value, _RevSetFileName))
                {
                    return;
                }

                _RevSetFileName = value;
                OnPropertyChanged(nameof(RevSetFileName));
            }
        }
        #endregion


        #region NaviViewModel

        NaviViewModel _NaviViewModel = null;
        public NaviViewModel NaviViewModel
        {
            get
            {
                if (_NaviViewModel == null)
                    _NaviViewModel = new NaviViewModel(this);

                return _NaviViewModel;
            }
        }

        #endregion


        #region ContentViewModel

        ContentViewModel _ContentViewModel = null;
        public ContentViewModel ContentViewModel
        {
            get
            {
                if (_ContentViewModel == null)
                {
                    _ContentViewModel = new ContentViewModel(this);
                    //統計対象ファイルカテゴリー
                    _ContentViewModel.OpenJson("DefaultFileCategories.json");
                }

                return _ContentViewModel;
            }
        }

        #endregion

        #region ReportViewModel

        ReportViewModel _ReportViewModel = null;
        public ReportViewModel ReportViewModel
        {
            get
            {
                if (_ReportViewModel == null)
                {
                    _ReportViewModel = new ReportViewModel(this);
                    
                }

                return _ReportViewModel;
            }
        }

        #endregion



        public TickerViewModel TickerViewModel { get; private set; }


        #region Exit
        private RelayCommand exitCommand;
        /// <summary>
        /// Exit from the application
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (this.exitCommand == null)
                {
                    this.exitCommand = new RelayCommand(System.Windows.Application.Current.Shutdown);
                }

                return this.exitCommand;
            }
        }

        #endregion

        #region Open
        RelayCommand _openCommand = null;
        
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand<string>((p) => OnOpen(p), (p) => CanOpen(p));
                   
                }

                return _openCommand;
            }
        }

        private bool CanOpen(string p)
        {
            if (p.Equals("f") )
            {
                if (this._IsAll)
                {
                    return false;
                }else
                {
                    return true;
                }

            }
            else
            {

                return true;
            }


        }
        #endregion

        #region Drop
        RelayCommand _dropCommand = null;

        public ICommand DropCommand
        {
            get
            {
                if (_dropCommand == null)
                {
                    _dropCommand = new RelayCommand<System.Windows.DragEventArgs>((p) => OnDrop(p));
                }

                return _dropCommand;
            }
        }
        #endregion

        private void OnDrop(System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {

                string[] drags = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

                foreach(string item in drags)
                {
                    if (System.IO.Directory.Exists(item))
                    {
                        OnOpenDir(item);

                    }
                    else if (System.IO.File.Exists(item))
                    {
                        if (!this._IsAll)
                        {
                            OnOpenFile(item);
                        }
                        

                    }
                }
                e.Handled = true;
            }


        }
        

        /// <summary>
        /// Open Dir
        /// </summary>
        /// <param name="sdir"></param>
        private void OnOpenDir(string sdir)
        {

            string sResult = string.Empty;
            string sRevFolder = string.Empty;
            bool bChkRev =false;

            //履歴ファイルを読み込む
            string cwxComp;
            string sPrefix;
            string sSuffix;
            string sRevFilePath = string.Empty;

            //履歴ファイルを読み込む
            cwxComp = sdir.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault<string>();
            sPrefix = this.VerNoPrefixSuffix.Split(',').FirstOrDefault<string>();
            sSuffix = this.VerNoPrefixSuffix.Split(',').LastOrDefault<string>();

            if (!IsAll)
            {
                if (this._IsRevScan)
                {

                    bChkRev = CheckProjForlder(sdir, ref sResult, ref sRevFolder);

                }
                else
                {
                    if (string.IsNullOrWhiteSpace(this._RevSetFileName))
                    {
                        this.RevSetFileName = GetDefaultRevFileName(cwxComp);
                    }

                    bChkRev = CheckProjForlder_RevSet(sdir, this._RevSetFileName, ref sResult, ref sRevFolder);
                }


                if (!bChkRev)
                {

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _dialogCoordinator.ShowMessageAsync(this, "プロジェクト構成エラー", sResult)));
                    return;
                }



                if (this._IsRevScan)
                {
                    sRevFilePath = System.IO.Path.Combine(sRevFolder, cwxComp + sSuffix + ".rev");
                }
                else
                {
                    sRevFilePath = sRevFolder;
                }

            }
            
            this.IsProgress = true;
            
            Task.Factory.StartNew(() => {

                //履歴ファイルを読み込み
                this._NaviViewModel.ProcessPathRevFile(cwxComp, sRevFilePath, sPrefix);
                
                //ステップをカウント
                this._ContentViewModel.Scan(sdir, cwxComp, IsAll);
                
                
            }).ContinueWith(_ =>
                {

                    this._NaviViewModel.ProcessPathRevFileFinish();
                    this._ContentViewModel.ScanFinish();
                    this.IsProgress = false;

                }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(_=> {

                    this._NaviViewModel.SelectLast();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            

        }

        /// <summary>
        /// Open File
        /// </summary>
        /// <param name="sFileName"></param>
        private void OnOpenFile(string sFileName)
        {

            string sResult = string.Empty;
            string sRevFolder = string.Empty;

            
            if (this._IsRevScan)
            {

                if (!CheckExcelFile(sFileName, ref sResult, ref sRevFolder))
                {

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _dialogCoordinator.ShowMessageAsync(this, "プロジェクト構成エラー", sResult)));
                    return;
                }

            }          


            this.IsProgress = true;

            Task.Factory.StartNew(() => {

                
                //VBAコードを抽出する

                FileInfo FileInfo = new FileInfo(sFileName);
                string sSaveTempPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Temp", DateTime.Now.ToString("yyyyMMddhhmmss"));
                string cwxComp = FileInfo.Name.Substring(0, 9);

                CopyFileExcel(FileInfo.Directory.FullName, FileInfo.Name, sSaveTempPath, sRevFolder);


                //履歴ファイルを読み込む
                string sPrefix = this.VerNoPrefixSuffix.Split(',').FirstOrDefault<string>();
                string sSuffix = this.VerNoPrefixSuffix.Split(',').LastOrDefault<string>();
                string sRevFilePath = System.IO.Path.Combine(sRevFolder, cwxComp + sSuffix + ".rev");

                if (!this._IsRevScan)
                {
                    if (string.IsNullOrWhiteSpace(this._RevSetFileName))
                    {
                        this.RevSetFileName = GetDefaultRevFileName(cwxComp);
                    }

                    
                    if(! CheckProjForlder_RevSet(System.IO.Path.Combine(sSaveTempPath, cwxComp), this._RevSetFileName, ref sResult, ref sRevFilePath))
                    {
                        //System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _dialogCoordinator.ShowMessageAsync(this, "プロジェクト構成エラー", sResult)));
                        return;
                    }

                }
                
                //履歴ファイルを読み込み
                this._NaviViewModel.ProcessPathRevFile(cwxComp, sRevFilePath, sPrefix);


                //ステップをカウント
                this._ContentViewModel.Scan(System.IO.Path.Combine(sSaveTempPath, cwxComp), cwxComp, IsAll);

            }).ContinueWith(_ =>
            {

                this._NaviViewModel.ProcessPathRevFileFinish();
                this._ContentViewModel.ScanFinish();
                this.IsProgress = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        /// <summary>
        /// プロジェクトを開く
        /// </summary>
        /// <param name="p"></param>
        private void OnOpen(string p)
        {

            string sResult = string.Empty;
            string sRevFolder = string.Empty;

            
            if (p.Equals("d", StringComparison.OrdinalIgnoreCase))
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog
                {
                    Description = "プロジェクトフォルダを選択してください",
                    SelectedPath = System.IO.Directory.Exists(this.sLastedForlder) ? this.sLastedForlder : Environment.SpecialFolder.MyComputer.ToString()
                };

                System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.sLastedForlder = folderBrowserDialog1.SelectedPath;

                    OnOpenDir(this.sLastedForlder);

                }

               
            }
            else if(p.Equals("f", StringComparison.OrdinalIgnoreCase))
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = false;
                dlg.Title = "帳票(Excel)を選択してください";
                dlg.RestoreDirectory = true;
                dlg.Filter = "帳票ファイル|*.xls|帳票(2013)|*.xlsx";
               
                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    OnOpenFile(dlg.FileName);


                }
                

            }
            
        }

        /// <summary>
        /// VB.net、Java　プロジェクトのフォルダ構成が正しいかチェック
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="sResult"></param>
        /// <param name="sRevFolder"></param>
        /// <returns></returns>
        private Boolean CheckProjForlder(string sPath, ref string sResult, ref string sRevFolder)
        {
            sResult = "";
            if (!System.IO.Directory.Exists(sPath))
            {
                sResult = String.Format("{0}は存在しません！", sPath);
                return false;
            }
                

            string rootPath = sPath.Replace("*", string.Empty);
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            IEnumerable<FileInfo> foundFiles;
            IEnumerable<DirectoryInfo> foundDirs;

            foundDirs = dir.GetDirectories("Rev", SearchOption.TopDirectoryOnly);
            if (foundDirs.LongCount() == 0)
            {
                sResult = String.Format("{0}配下にrevフォルダは存在しません！", sPath);
                return false;
            }

            foreach(DirectoryInfo DirectoryInfo in foundDirs)
            {
                foundFiles = DirectoryInfo.GetFiles("*.rev", SearchOption.AllDirectories);
                if (foundFiles.LongCount() == 0)
                {
                    sResult = String.Format("{0}配下に履歴ファイルは存在しません！", DirectoryInfo.FullName);
                    return false;
                }
            }
            sRevFolder = foundDirs.FirstOrDefault<DirectoryInfo>().FullName;
            return true;
        }

        /// <summary>
        /// VB.net、Java　プロジェクトのフォルダ構成が正しいかチェック(履歴ファイル名を指定する場合)
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="sResult"></param>
        /// <param name="sRevFolder"></param>
        /// <returns></returns>
        private Boolean CheckProjForlder_RevSet(string sPath,string sFileNm, ref string sResult, ref string sRevFolder)
        {
            sResult = "";
            if (!System.IO.Directory.Exists(sPath))
            {
                sResult = String.Format("{0}は存在しません！", sPath);
                return false;
            }


            string rootPath = sPath.Replace("*", string.Empty);
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            IEnumerable<FileInfo> foundFiles;
            
           
            foundFiles = dir.GetFiles("*" + sFileNm + "*", SearchOption.AllDirectories);
            if (foundFiles.LongCount() == 0)
            {
                sResult = String.Format("{0}配下に履歴ファイルは存在しません！", sFileNm);
                return false;
            }
            sRevFolder = foundFiles.FirstOrDefault<FileInfo>().FullName;
            return true;
        }

        /// <summary>
        /// デフォルト名を取得
        /// </summary>
        /// <param name="sComponentName"></param>
        /// <returns></returns>
        private string GetDefaultRevFileName(string sComponentName)
        {
            Dictionary<string, string> CompFile = new Dictionary<string, string>();
            string sRet = "";
            string sType = sComponentName.Substring(0, 2);

            CompFile.Add("bj", "*Obj.java");
            CompFile.Add("bs", "*.java");
            CompFile.Add("ue", "Main.bas");
            CompFile.Add("ud", "Main.bas");
            CompFile.Add("up", "M_Main.vb");

            sRet = CompFile.Where(item => item.Key.Equals(sType)).Select(item => item.Value).First();
            sRet = sRet.Replace("*", sComponentName);

            return sRet;
        }
        /// <summary>
        /// 帳票のフォルダ構成が正しいかチェック
        /// </summary>
        /// <param name="sFile"></param>
        /// <param name="sResult"></param>
        /// <param name="sRevFolder"></param>
        /// <returns></returns>
        private Boolean CheckExcelFile(String sFile, ref String sResult, ref string sRevFolder)
        {
            sResult = "";
            String sFileShortName;
            if (!System.IO.File.Exists(sFile))
            {
                sResult = String.Format("{0}は存在しません！", sFile);
                return false;
            }

            FileInfo FileInfo = new FileInfo(sFile);
            sFileShortName = FileInfo.Name.Substring(0, 9);
            string rootPath = Path.Combine(FileInfo.DirectoryName + "_rev", sFileShortName);

            if (!System.IO.Directory.Exists(rootPath))
            {
                sResult = String.Format("{0}配下にrevフォルダは存在しません！", rootPath);
                return false;
            }

            DirectoryInfo dir = new DirectoryInfo(rootPath);
            IEnumerable<FileInfo> foundFiles;
            foundFiles = dir.GetFiles(sFileShortName + "*.rev", SearchOption.AllDirectories);
            if (foundFiles.LongCount() == 0)
            {
                sResult = String.Format("{0}配下に履歴ファイルは存在しません！", dir.FullName);
                return false;
            }
            sRevFolder = rootPath;
            return true;
        }

        

        /// <summary>
        /// VBAコードを抽出する
        /// </summary>
        /// <param name="sSourcePath"></param>
        /// <param name="sExcel"></param>
        /// <param name="sSavePath"></param>
        /// <param name="sRevFolder"></param>
        private void CopyFileExcel(string sSourcePath,string sExcel, string sSavePath, string sRevFolder)
        {

            if (!System.IO.Directory.Exists(sSourcePath))
            {
                return;
            }



            string rootPath = sSourcePath.Replace("*", string.Empty);
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            StringBuilder ngfile = new StringBuilder();
            IEnumerable<FileInfo> foundFiles;
            IEnumerable<FileInfo> foundRevFiles;

            foundFiles = dir.GetFiles(sExcel,SearchOption.TopDirectoryOnly);

            foreach (FileInfo fileinfo in foundFiles)
            {

               
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9))))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9)));
                }


                CopyFileFromExcelSub(fileinfo.FullName,
                                     System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9)),
                                     ngfile);

                if (System.IO.Directory.Exists(sRevFolder))
                {
                    DirectoryInfo revdir = new DirectoryInfo(sRevFolder);

                    //履歴ファイル
                    foundRevFiles = revdir.GetFiles("*.rev", SearchOption.AllDirectories);

                    foreach (FileInfo revfileinfo in foundRevFiles)
                    {

                        if (!System.IO.Directory.Exists(System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9), "Rev")))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9), "Rev"));
                        }

                        System.IO.File.Copy(revfileinfo.FullName,
                                            System.IO.Path.Combine(sSavePath, fileinfo.Name.Substring(0, 9), "Rev", revfileinfo.Name),
                                            true);


                    }
                }
                
                
            }
            

            if (ngfile.Length > 0)
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ngfile.txt"),
                                            ngfile.ToString(),
                                            Encoding.GetEncoding("Shift_JIS"));


            }
            return;
        }


        private void CopyFileFromExcelSub(string sExcelFile, string sSavePath, StringBuilder ngfile)
        {

           
            this.RemoveVBAPassword(sExcelFile);

            object workbook = null;
            object code = null;
            object oExcel = null;
            bool hasException = false;

            oExcel = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("Excel.Application", ""));

            NewLateBinding.LateSet(oExcel, null, "Visible", new object[]
            {
                false
            }, null, null);

            NewLateBinding.LateSetComplex(NewLateBinding.LateGet(oExcel, null, "Application", new object[0], null, null, null), null, "ScreenUpdating", new object[]
            {
                false
            }, null, null, false, true);

            NewLateBinding.LateSetComplex(NewLateBinding.LateGet(oExcel, null, "Application", new object[0], null, null, null), null, "DisplayAlerts", new object[]
            {
                false
            }, null, null, false, true);
            NewLateBinding.LateSet(oExcel, null, "WindowState", new object[]
            {
                2
            }, null, null);


            checked
            {
                try
                {

                    hasException = false;

                    workbook = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(oExcel, null, "Workbooks", new object[0], null, null, null), null, "Open", new object[]
                    {
                        sExcelFile,
                        Missing.Value,
                        true
                    }, null, null, null));

                    IEnumerator enumerator = ((IEnumerable)NewLateBinding.LateGet(NewLateBinding.LateGet(workbook, null, "VBProject", new object[0], null, null, null), null, "VBComponents", new object[0], null, null, null)).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object comp = RuntimeHelpers.GetObjectValue(enumerator.Current);
                        code = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(comp, null, "CodeModule", new object[0], null, null, null));
                        bool flag = Operators.ConditionalCompareObjectGreater(NewLateBinding.LateGet(code, null, "CountOfLines", new object[0], null, null, null), 0, false);
                        if (flag)
                        {
                            //content.Append(Operators.ConcatenateObject(NewLateBinding.LateGet(comp, null, "Name", new object[0], null, null, null), "\r\n"));
                            object arg_232_0 = NewLateBinding.LateGet(workbook, null, "VBProject", new object[0], null, null, null);
                            Type arg_232_1 = null;
                            string arg_232_2 = "VBComponents";
                            object[] array = new object[1];
                            object[] arg_219_0 = array;
                            int arg_219_1 = 0;
                            object instance = comp;
                            arg_219_0[arg_219_1] = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(instance, null, "Name", new object[0], null, null, null));
                            object[] array2 = array;
                            object[] arg_232_3 = array2;
                            string[] arg_232_4 = null;
                            Type[] arg_232_5 = null;
                            bool[] array3 = new bool[]
                            {
                                true
                            };
                            object arg_2CF_0 = NewLateBinding.LateGet(arg_232_0, arg_232_1, arg_232_2, arg_232_3, arg_232_4, arg_232_5, array3);
                            if (array3[0])
                            {
                                NewLateBinding.LateSetComplex(instance, null, "Name", new object[]
                                {
                                    RuntimeHelpers.GetObjectValue(array2[0])
                                }, null, null, true, false);
                            }

                            string wkname = (string)NewLateBinding.LateGet(workbook, null, "Name", new object[0], null, null, null);
                            string compname = (string)NewLateBinding.LateGet(comp, null, "Name", new object[0], null, null, null);

                            NewLateBinding.LateCall(arg_2CF_0, null, "Export", new object[]
                            {
                                //sSavePath + "\\" + wkname.Substring(0,wkname.Length-4) + "_" + compname + ".vb"
                                sSavePath + "\\" +  compname + ".vb"
                            }, null, null, null, true);

                        }
                    }
                }
                catch (Exception)
                {
                    hasException = true;
                    ngfile.Append(sExcelFile + "\r\n");
                }
                finally
                {
                }
                NewLateBinding.LateCall(NewLateBinding.LateGet(oExcel, null, "Application", new object[0], null, null, null), null, "Quit", new object[0], null, null, null, true);
                Marshal.FinalReleaseComObject(RuntimeHelpers.GetObjectValue(oExcel));
                GC.Collect(0);
                oExcel = null;

                //if (!hasException)
                //    System.IO.File.Delete(sExcelFile);
                //File.WriteAllText("c:\\vbacode--.txt", codecontent.ToString(), Encoding.Default);
                //Interaction.MsgBox(content.ToString(), MsgBoxStyle.OkOnly, null);
            }


        }

        private int RemoveVBAPassword(string filename)
        {
            int LiCMG = 0;
            int LiDPB = 0;
            string LsTemp = "";
            byte[] buffer = File.ReadAllBytes(filename);
            for (int i = 0; i < buffer.Length - 5; i++)
            {
                LsTemp = Encoding.ASCII.GetString(buffer, i, 5);
                if ("CMG=\"".Equals(LsTemp))
                {
                    LiCMG = i;
                }
                int index_34 = 0;
                if (LsTemp.StartsWith("GC="))
                {
                    for (int j = i + 3; j < buffer.Length - 1; j++)
                    {
                        if ("\"".Equals(Encoding.ASCII.GetString(buffer, j, 1)))
                        {
                            index_34++;
                            if (index_34 == 2)
                            {
                                LiDPB = j;
                                break;
                            }
                        }
                    }
                }
                if (LiDPB != 0)
                {
                    break;
                }
            }
            if (LiCMG == 0)
            {
                return 1;
            }
            byte[] test_buffer = (new MemoryStream(buffer, LiCMG, LiDPB - LiCMG)).ToArray();
            string test_text = Encoding.ASCII.GetString(test_buffer);
            for (int i = LiCMG; i <= LiDPB; i++)
            {
                byte ascii = buffer[i];
                byte ascii_next = buffer[i + 1];
                if (isUseChar(ascii) && isUseChar(ascii_next))
                {
                    buffer[i] = 32;
                }
            }
            File.WriteAllBytes(filename, buffer);
            return 0;
        }

        private bool isUseChar(byte ascii)
        {
            if ((ascii >= 48 && ascii <= 57) || (ascii >= 65 && ascii <= 70))
            {
                return true;
            }
            if (ascii == 34)
            {
                return true;
            }
            if (ascii == 13 || ascii == 10)
            {
                return true;
            }
            if (ascii == 61)
            {
                return true;
            }
            if (ascii == 71 || ascii == 77 || ascii == 80)
            {
                return true;
            }
            return false;
        }
        
        public void Dispose()
        {
            return;
        }

        #region Change Language
        private RelayCommand changeLanguageCommand;
        /// <summary>
        /// Change Language
        /// </summary>
        public ICommand ChangeLanguageCommand
        {
            get
            {
                if (this.changeLanguageCommand == null)
                {
                    this.changeLanguageCommand = new RelayCommand<string>((p) => ChangeLanguage(p));
                }

                return this.changeLanguageCommand;
            }
        }

        private void ChangeLanguage(string p)
        {
            
            ResourceDictionary rd = System.Windows.Application.Current.Resources.MergedDictionaries.LastOrDefault<ResourceDictionary>();
            rd.Source=new System.Uri(string.Format("Resource\\{0}.xaml",p), System.UriKind.Relative);


        }
        #endregion

        #region Save 
        private RelayCommand saveCommand;
        /// <summary>
        /// Change Language
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (this.saveCommand == null)
                {
                    this.saveCommand = new RelayCommand<string>((p) => Save(p),(p)=> CanSave(p));
                }

                return this.saveCommand;
            }
        }

        private void Save(string p)
        {

           
            if (p.Equals("copycurrent"))
            {
               
                string compName = this.ContentViewModel.SelectedItem.ComponentName;
                CwxStatistic result =this.ContentViewModel.ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(compName));
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => System.Windows.Clipboard.SetDataObject(result.ToString())));
                return;

            }

            if (p.Equals("copyselected"))
            {
                string compName = this.ContentViewModel.SelectedItem.ComponentName;
                CwxStatistic result = this.ContentViewModel.ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(compName));

                if(result.SelectedItem is Statistics)
                {
                    Statistics selecteditem = (Statistics)result.SelectedItem;
                    StringBuilder strBuf = new StringBuilder();

                    strBuf.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"\r\n", "FileName", "Type", "FactStep", "SpaceStep", "CommentStep", "Sum");
                    strBuf.AppendLine(selecteditem.ToString());
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => System.Windows.Clipboard.SetDataObject(strBuf.ToString())));
                    return;

                } 
            }

            if (p.Equals("csv"))
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                
                dlg.Title = "保守CSV文件";
                dlg.RestoreDirectory = true;
                dlg.Filter = "CSV文件|*.csv|任意文件|*.*";

                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    
                    StringBuilder strBuf = new StringBuilder();
                    this.ContentViewModel.ComponentStatistics.ToList().ForEach(item => {
                        strBuf.AppendLine(string.Format("<{0}>",item.ComponentName));
                        strBuf.Append(item.ToString());
                        strBuf.AppendLine();

                    });

                    System.IO.File.WriteAllText(dlg.FileName, strBuf.ToString());
                }

            }
        }

        private bool CanSave(string p)
        {
            if (p.Equals("copyselected"))
            {
                if (this.ContentViewModel.ComponentStatistics.Count > 0)
                {
                    string compName = this.ContentViewModel.SelectedItem.ComponentName;
                    CwxStatistic result = this.ContentViewModel.ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(compName));
                    return result.SelectedItem != null;

                }else
                {
                    return false;
                }

            }
            else
            {
                
                return this.ContentViewModel.ComponentStatistics.Count > 0;
            }


        }
        #endregion



    }
}
