using System.Collections.Generic;
using Utility;

namespace Management.Areas.SystemManageMent.Models.FilePath
{
    public class FilePathViewModel
    {
        public List<FilePathModel> filePaths { get; set; }
        public DefaultPager defaultPager { get; set; }
    }

    public class FilePathModel
    {
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
    }
}
