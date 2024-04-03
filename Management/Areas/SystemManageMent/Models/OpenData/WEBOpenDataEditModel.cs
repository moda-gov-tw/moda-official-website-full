using DBModel;
using System.Collections.Generic;
using Utility;
using static Utility.Files;

namespace Management.Areas.SystemManageMent.Models.OpenData
{
    public class WEBOpenDataEditModel
    {
        public WEBOpenDataMain wEBOpenDataMain { get; set; }
        public List<WEBOpenDataDetail> wEBOpenDataDetails { get; set; }
        public List<WEBOpenDataSchema> wEBOpenDataSchemas { get; set; }
        public List<ExpandoObject> SchemasObject { get; set; }
        public List<CommonFileModel> commonFileModels { get; set; } = new List<CommonFileModel>();
        public RelSysUserGroup relSysUserGroup { get; set; }
        public WEBNewsExtend wEBNewsExtend { get; set; }

        public string sysUserSysDepartmentID { get; set; }

    }

    public class ExpandoObject
    {
        public int OpenDataMainSN { get; set; }
        public int OpenDataDetailSN { get; set; }
        public string Code { get; set; }
        public string TableName { get; set; }
        public string Column { get; set; }
        public int? SortOrder { get; set; }
    }
}
