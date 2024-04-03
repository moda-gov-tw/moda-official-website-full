using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class OpenDataType
    {
        public static List<OpenDataTypeModel> GetForm()
        {
            var list = new List<OpenDataTypeModel>()
            {
                new OpenDataTypeModel(){title ="XML",value = "1"},
                new OpenDataTypeModel(){ title ="JSON",value="2"},
                new OpenDataTypeModel(){title="CSV",value="3"},
            };
            return list;
        }

        public static List<OpenDataTypeModel>GetJoin()
        {
            var list = new List<OpenDataTypeModel>()
            {
                new OpenDataTypeModel(){title ="INNER JOIN",value = "0"},
                new OpenDataTypeModel(){ title ="LEFT JOIN",value="1"},
                new OpenDataTypeModel(){title="RIGHT JOIN",value="2"},
                new OpenDataTypeModel(){title="CROSS JOIN",value="3"},
                new OpenDataTypeModel(){title="FULL OUTER JOIN",value="4"},
            };
            return list;
        }

        public static List<OpenDataTypeModel> GetUpdateTime()
        {
            var list = new List<OpenDataTypeModel>()
            {
                new OpenDataTypeModel(){title ="永不",value = "永不"},
                new OpenDataTypeModel(){ title ="每日",value="每日"},
                new OpenDataTypeModel(){title="每週",value="每週"},
                new OpenDataTypeModel(){title="每月",value="每月"},
                new OpenDataTypeModel(){title="每季",value="每季"},
                new OpenDataTypeModel(){title="半年",value="半年"},
                new OpenDataTypeModel(){title="每年",value="每年"},
                new OpenDataTypeModel(){title="不定期",value="不定期"},
                new OpenDataTypeModel(){title="每秒",value="每秒"},
            };
            return list;
        }


    }
    public class OpenDataTypeModel
    {
        public string title { get; set; }
        public string value { get; set; }
    }
}
