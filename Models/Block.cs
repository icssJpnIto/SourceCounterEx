using System;
using System.Linq;
using System.Text;

namespace SourceCounterEx.Models
{
    /// <summary>
    /// 変更内容BlockのModelクラス
    /// </summary>
    public class Block 
    {
        public string Content { get; set; }

        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public int Code { get; set; }

        public int Comments { get; set; }

        public int Empty { get; set; }

        public int FactStep { get; set; }

        public string FileName { get; set; }

        public string ChangeName { get; set; }

        public string ComponetName { get; set; }


        public string keyHeader {
            get
            {
                return string.Format("{0},{1},{2}",ComponetName,FileName,ChangeName);
            }

        }

        public static string MakekeyHeader(string sComponetName,string sFileName,string sChangeName)
        {
            return string.Format("{0},{1},{2}", sComponetName, sFileName, sChangeName);
        }

    }
}
