using SourceCounterEx.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using SourceCounterEx.Command;
using System.Windows.Input;

namespace SourceCounterEx.ViewModels
{
    public class ContentViewModel : ViewModel
    {

        #region 構成関数
        private MainViewModel _mainvm;
        public ContentViewModel(MainViewModel mainvm)
        {
            //メインViewModelを引き渡す
            this._mainvm = mainvm;
            
        }
        #endregion



        #region CwxBlock(改修内容のブロックの集合)
        private ObservableCollection<CwxBlock> _CwxBlock = new ObservableCollection<CwxBlock>();
        public ObservableCollection<CwxBlock> CwxBlock
        {
            get { return this._CwxBlock; }
        }
        #endregion



        #region ComponentStatistics(コンポーネット単位の統計情報の集合)
        private ObservableCollection<CwxStatistic> _ComponentStatistics = new ObservableCollection<CwxStatistic>();
        private ObservableCollection<CwxStatistic> ComponentStatistics_Binding = new ObservableCollection<CwxStatistic>();
        public ObservableCollection<CwxStatistic> ComponentStatistics
        {
            get { return this.ComponentStatistics_Binding; }
        }
        #endregion

        #region Categories(統計ファイル対象の種類)
        private ObservableCollection<FileCategory> cats = new ObservableCollection<FileCategory>();
        public ObservableCollection<FileCategory> Categories
        {
            get { return this.cats; }
        }
        public void OpenJson(string filePath)
        {
            FileInfo f = new FileInfo(filePath);
            if (File.Exists(f.FullName))
            {
                this.cats = JsonConvert.DeserializeObject<ObservableCollection<FileCategory>>(File.ReadAllText(f.FullName));

            }
        }
        #endregion



        #region ファイルを走査する
        /// <summary>
        /// カウント完了時、ViewModelに反映する
        /// </summary>
        public void ScanFinish()
        {

            foreach (CwxStatistic item in this._ComponentStatistics)
            {
                this.ComponentStatistics_Binding.Add(item);
              
            }

            this._ComponentStatistics.Clear();
        }


        /// <summary>
        /// パスを指定してステップをカウントする
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="componet"></param>
        /// <param name="IsScanAll"></param>
        public void Scan(string rootPath,string componet,bool IsScanAll)
        {
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            IEnumerable<FileInfo> foundFiles;

            foundFiles = dir.GetFiles("*", SearchOption.AllDirectories).Where(x =>(x.Attributes & FileAttributes.Hidden) == 0);

            //コンポーネット統計集合に追加
            this._ComponentStatistics.Add(new CwxStatistic() { ComponentName = componet, Statistics = new ObservableCollection<Statistics>() });

            //コンポーネットブロック集合に追加
            this._CwxBlock.Add(new CwxBlock() { ComponentName = componet, Blocks = new ObservableCollection<Block>() });

            //該当するコンポーネットの改修履歴を取得
            IEnumerable<Change> chgs = this._mainvm.NaviViewModel.CwxComponets_In.Where(item => item.ComponentName.Equals(componet)).SelectMany(item => item.Changes);

            //ファイル種別毎に統計
            for (int i = 0; i < this.Categories.Count(); i++)
            {

                if (IsScanAll)
                {
                    this.ProcessPath(this.Categories[i],
                                    foundFiles,
                                    this._ComponentStatistics.LastOrDefault().Statistics,
                                    null,
                                    null);

                    //合計
                    this._ComponentStatistics.LastOrDefault().Sum();
                }
                else
                {


                    if (this.ProcessPath(this.Categories[i],
                                        foundFiles,
                                        this._ComponentStatistics.LastOrDefault().Statistics,
                                        chgs,
                                        this._CwxBlock.LastOrDefault().Blocks) > 0)
                    {

                        //OutPut
                        //foreach (Change chg in chgs)
                        //{
                        //    Console.WriteLine(this._CwxBlock.LastOrDefault().Blocks.Where(block => block.ChangeName.Equals(chg.No)).Sum(block => block.FactStep));
                        //}

                        //ブロック集合からファイル名を抽出する
                        IEnumerable<string> files = this._CwxBlock.LastOrDefault().Blocks.Select(block => block.FileName).Distinct();

                        foreach (string file in files)
                        {
                            int factcount = 0;
                            int emptycount = 0;
                            int commentcount = 0;

                            //ファイル別にサマリ
                            emptycount = this._CwxBlock.LastOrDefault().Blocks.Where(block => block.FileName.Equals(file)).Sum(block => block.Empty);
                            commentcount = this._CwxBlock.LastOrDefault().Blocks.Where(block => block.FileName.Equals(file)).Sum(block => block.Comments);
                            factcount = this._CwxBlock.LastOrDefault().Blocks.Where(block => block.FileName.Equals(file)).Sum(block => block.FactStep);

                            this._ComponentStatistics.LastOrDefault().Statistics.Add(new Statistics
                            {
                                FileName = file,
                                Type = this.Categories[i].Category,
                                SpaceStep = emptycount,
                                CommentStep = commentcount,
                                FactStep = factcount,
                                SumStep = commentcount + emptycount + factcount,

                            });
                        }
                    }
                    
                    //合計
                    this._ComponentStatistics.LastOrDefault().Sum();

                }
                
            }
            
        }

        /// <summary>
        /// カテゴリー別にファイルをカウントする
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="fileInfo"></param>
        /// <param name="filereport"></param>
        /// <param name="chgs"></param>
        /// <param name="blocks"></param>
        /// <returns></returns>
        private int ProcessPath(FileCategory cat, IEnumerable<FileInfo> fileInfo, ObservableCollection<Statistics> filereport, IEnumerable<Change> chgs, ObservableCollection<Block> blocks)
        {
            cat.Code = 0;
            cat.Comments = 0;
            cat.Empty = 0;
            cat.ExcludedFiles = 0;
            cat.IncludedFiles = 0;
            cat.TotalFiles = 0;
            cat.TotalLines = 0;

            int iRet = 0;
            if (!cat.Include)
            {
                return iRet;
            }

            foreach (FileInfo f in cat.FileTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).SelectMany(type => fileInfo.Where(f => f.Extension.ToLower(CultureInfo.CurrentCulture) == type.TrimStart().ToLower(CultureInfo.CurrentCulture))))
            {
                cat.TotalFiles++;

                if (blocks == null)
                {
                    //ブロックで分けない
                    this.CountLines(f, cat, filereport);
                }
                else
                {
                    //ブロックで分けるカウント
                    this.CountLinesEx(f, cat, chgs, blocks);

                    //ブロック内のステップをカウント
                    this.CountLinesInBlock(cat, blocks);
                    
                }
                iRet++;

            }

            cat.Code = cat.TotalLines - cat.Empty - cat.Comments;
            return iRet;
        }


        private void CountLines(FileSystemInfo i, FileCategory cat, ObservableCollection<Statistics> filereport)
        {
            FileInfo thisFile = new FileInfo(i.FullName);
          
            if (!string.IsNullOrEmpty(cat.NameExclusions))
            {
                if (cat.NameExclusions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(s => thisFile.FullName.ToLower(CultureInfo.CurrentCulture).Contains(s.ToLower(CultureInfo.CurrentCulture))))
                {
                    cat.ExcludedFiles++;
                    return;
                }
            }

            bool incomment = false;
            bool handlemulti = !string.IsNullOrWhiteSpace(cat.MultilineCommentStart);
            int filelinecount = 0;
            int emptycount = 0;
            int commentcount = 0;

            foreach (string line in File.ReadLines(i.FullName))
            {
                filelinecount++;
                string line1 = line;
                if (string.IsNullOrWhiteSpace(line))
                {
                    cat.Empty++;
                    emptycount++;
                }
                else
                {
                    if (handlemulti)
                    {
                        if (incomment)
                        {
                            cat.Comments++;
                            commentcount++;
                            if (line1.TrimEnd(' ').EndsWith(cat.MultilineCommentEnd, StringComparison.OrdinalIgnoreCase))
                            {
                                incomment = false;
                            }
                        }
                        else
                        {
                            if (line1.TrimStart(' ').StartsWith(cat.MultilineCommentStart, StringComparison.OrdinalIgnoreCase))
                            {
                                incomment = true;
                                cat.Comments++;
                                commentcount++;
                                if (line1.TrimEnd(' ').EndsWith(cat.MultilineCommentEnd, StringComparison.OrdinalIgnoreCase))
                                {
                                    incomment = false;
                                }
                            }
                            else
                            {
                                foreach (string s in cat.SingleLineComment.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(s => line1.TrimStart(' ').TrimStart('\t').StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                                {
                                    cat.Comments++;
                                    commentcount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string s in cat.SingleLineComment.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(s => line1.TrimStart(' ').TrimStart('\t').StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                        {
                            cat.Comments++;
                            commentcount++;
                        }
                    }
                }

                cat.TotalLines++;
            }

            filereport.Add(new Statistics { FileName = thisFile.Name, Type = cat.Category, FactStep= filelinecount - emptycount - commentcount, SpaceStep= emptycount, CommentStep= commentcount, SumStep= filelinecount });

           cat.IncludedFiles++;
        }
        
        private void CountLinesEx(FileSystemInfo i, FileCategory cat, IEnumerable<Change> chgs, ObservableCollection<Block> blocks)
        {
            FileInfo thisFile = new FileInfo(i.FullName);

            if (!string.IsNullOrEmpty(cat.NameExclusions))
            {
                if (cat.NameExclusions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(s => thisFile.FullName.ToLower(CultureInfo.CurrentCulture).Contains(s.ToLower(CultureInfo.CurrentCulture))))
                {
                    cat.ExcludedFiles++;
                    return;
                }
            }

            int filelinecount = 0;
            Stack<string> stkMark = new Stack<string>();
            StringBuilder strbufContent = new StringBuilder();
            string sTempKey = string.Empty;
            Change hitChg = null;

            foreach (string line in File.ReadLines(i.FullName, Encoding.GetEncoding("Shift_JIS")))
            {
                filelinecount++;
                string line1 = line;


               
                //変更履歴のStart
                if (line1.Contains(cat.ChgStart) &&
                    chgs.Any(chg => line1.Contains(chg.No)))
                {

                    
                    InsertBlock(stkMark, chgs, blocks, filelinecount, i.Name, strbufContent.ToString());

                    strbufContent.AppendLine(line1);
                    hitChg = chgs.Where(chg => line1.Contains(chg.No)).FirstOrDefault();
                    stkMark.Push(string.Format("{0},{1}", hitChg.No,filelinecount));
                    strbufContent.Clear();
                    
                }
                //変更履歴のEnd
                if (line1.Contains(cat.ChgEnd) &&
                    chgs.Any(chg => line1.Contains(chg.No)))
                {

                    strbufContent.AppendLine(line1);
                    InsertBlock(stkMark, chgs, blocks, filelinecount, i.Name, strbufContent.ToString());


                    hitChg = chgs.Where(chg => line1.Contains(chg.No)).FirstOrDefault();
                    if (stkMark.Count > 0)
                    {
                        stkMark.Pop();
                    }
                    
                    strbufContent.Clear();

                }

                if (stkMark.Count > 0)
                {
                    strbufContent.AppendLine(line1);
                }
                
            }
            
        }

        private void InsertBlock(Stack<string> stkMark, IEnumerable<Change> chgs, ObservableCollection<Block> blocks,int endrow,string fileName,string sContent)
        {
            Change hitChg = null;
            if (stkMark.Count > 0)
            {
                string chgNo = stkMark.Peek().Split(new[] { ',' })[0];
                int startRow = int.Parse(stkMark.Peek().Split(new[] { ',' })[1]);

                hitChg = chgs.Where(chg => chg.No.Equals(chgNo)).FirstOrDefault();
                if (hitChg != null)
                {
                    blocks.Add(new Block
                    {
                        ChangeName = hitChg.No,
                        ComponetName = hitChg.ComponentName,
                        FileName = fileName,
                        Content = sContent,
                        EndRow = endrow,
                        StartRow = startRow
                    });

                }
                else
                {
                    blocks.Add(new Block
                    {
                        ChangeName = chgNo,
                        ComponetName = "no exist",
                        FileName = fileName,
                        Content = sContent,
                        EndRow = endrow,
                        StartRow = endrow
                    });
                }


            }

        }

        private void CountLinesInBlock(FileCategory cat,ObservableCollection<Block> blocks)
        {
           
            bool incomment = false;
            bool handlemulti = !string.IsNullOrWhiteSpace(cat.MultilineCommentStart);
            int count = 0;
            int emptycount = 0;
            int commentcount = 0;
            string[] limit = {"\r\n" };
            string[] sContent = null ;

            foreach (Block block in blocks)
            {
                count = 0;
                emptycount = 0;
                commentcount = 0;
                sContent = block.Content.Substring(0, block.Content.Length-1).Split(limit, StringSplitOptions.None);
                                
                foreach (string line in sContent)
                {
                    count++;
                    string line1 = line;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        emptycount++;
                    }
                    else
                    {
                        if (handlemulti)
                        {
                            if (incomment)
                            {
                                commentcount++;
                                if (line1.TrimEnd(' ').EndsWith(cat.MultilineCommentEnd, StringComparison.OrdinalIgnoreCase))
                                {
                                    incomment = false;
                                }
                            }
                            else
                            {
                                if (line1.TrimStart(' ').StartsWith(cat.MultilineCommentStart, StringComparison.OrdinalIgnoreCase))
                                {
                                    incomment = true;
                                    commentcount++;
                                    if (line1.TrimEnd(' ').EndsWith(cat.MultilineCommentEnd, StringComparison.OrdinalIgnoreCase))
                                    {
                                        incomment = false;
                                    }
                                }
                                else
                                {
                                    foreach (string s in cat.SingleLineComment.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(s => line1.TrimStart(' ').TrimStart('\t').StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        commentcount++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (string s in cat.SingleLineComment.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(s => line1.TrimStart(' ').TrimStart('\t').StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                            {
                                commentcount++;
                            }
                        }
                    }


                }

                block.Empty = emptycount;
                block.Comments = commentcount;
                block.Code = count;
                block.FactStep = count - emptycount - commentcount;


            }
        }
        #endregion


        #region SelectedItem
        CwxStatistic _SelectedItem = null;
        public CwxStatistic SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                if (Equals(value, _SelectedItem))
                {
                    return;
                }

                _SelectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));

                //
                if(this._mainvm.NaviViewModel.CwxComponets!=null &&
                   CwxBlock!=null &&
                   ComponentStatistics != null &&
                   _SelectedItem!=null)
                {
                    this._mainvm.ReportViewModel.SetReportData(this._mainvm.NaviViewModel.CwxComponets.FirstOrDefault(item => item.ComponentName.Equals(_SelectedItem.ComponentName)),
                                                          CwxBlock.FirstOrDefault(item => item.ComponentName.Equals(_SelectedItem.ComponentName)),
                                                          ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(_SelectedItem.ComponentName)).Sum_SumStep,
                                                          ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(_SelectedItem.ComponentName)).Sum_FactStep);
                }
                else
                {
                    if (_SelectedItem == null)
                    {
                        this._mainvm.ReportViewModel.SetReportData(null, null, 1,1);
                    }

                }
                
            }
        }

        public void SetSelectedItem(string componetName)
        {
            SelectedItem= ComponentStatistics.FirstOrDefault(item => item.ComponentName.Equals(componetName));
        }
        #endregion

      

        }
}
