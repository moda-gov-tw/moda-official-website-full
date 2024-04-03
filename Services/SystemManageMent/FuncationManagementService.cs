using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Model.SystemManageMent;

namespace Services.SystemManageMent
{
    public class FuncationManagementService
    {
        public static List<SectionReportModel> getExcel1(SectionReportModel data)
        {
            using (var db = new Services.MODAContext())
            {
                try
                {
                    FormattableString sql = $@"
                    Select ParentTitle,ChildTitle,PIsEnable,CIsEnable,
                    case when (PIsEnable = '1' and CIsEnable = '1') then '1' else '0' end as IsEnable
                    from (
                    Select 
                    case when P.SysSectionSN is null then C.Title else P.Title end as ParentTitle,
                    case when P.SysSectionSN is null then '' else C.Title end as ChildTitle,
                    case when P.IsEnable is null then C.IsEnable else P.IsEnable end as PIsEnable,
                    C.IsEnable as CIsEnable,
                    C.Path
                    from [dbo].[SysSection] P
                    Right Outer Join [SysSection] C ON P.SysSectionSN = C.ParentSN
                    ) AA ";

                    var result = db.SectionReportModels.FromSqlInterpolated(sql);

                    if (data != null)
                    {
                        if (!string.IsNullOrWhiteSpace(data.ParentTitle))
                        {
                            result = result.Where(o => o.ParentTitle == data.ParentTitle);
                        }
                        if (!string.IsNullOrWhiteSpace(data.ChildTitle))
                        {
                            result = result.Where(o => o.ChildTitle == data.ChildTitle);
                        }
                        if (!string.IsNullOrWhiteSpace(data.IsEnable))
                        {
                            result = result.Where(o => o.IsEnable == data.IsEnable);
                        }
                    }

                    return result.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
