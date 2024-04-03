using System.Collections.Generic;

namespace Management.Models.Common
{
    public class SelectTextArrayModel : definitionModel
    {
        public string title { get; set; } = "";
        public selectTextType selectTextType { get; set; }
        public string txtName { get; set; } = "";
        public string valName { get; set; } = "";
        public List<SelectTxt> SelectTxts { get; set; } = new List<SelectTxt>();
    }
    public class SelectTxt
    {
        public string val { get; set; }
        public string txt {get;set;}
    }

    public enum selectTextType { 
        onlyText,
        TwoControl,
    
    }
}
