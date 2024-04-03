using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Utility.Model.SystemManageMent;
using Utility.Models.Authorization;

namespace Utility
{
    public class Output
    {

        /// <summary>
        /// 公版報表
        /// </summary>
        /// <param name="excelModel"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MemoryStream ExampleReport(ExcelModel excelModel, string fileName)
        {
            try
            {
                int startSheet = 1;
                int startRow = 0;
                startSheet = startSheet > 0 ? startSheet - 1 : 0;
                startRow = startRow > 0 ? startRow - 1 : 0;
                IWorkbook Workbook = NPOIExtensions.OpenExcel(fileName);
                ISheet mySheet = Workbook.GetSheetAt(startSheet);
                if (excelModel != null && excelModel.ExcelDetails != null)
                {
                    var endRow = startRow + (excelModel.Info == null ? 0 : excelModel.Info.Count()) + 1 + excelModel.ExcelDetails.Count();
                    var n = startRow + (excelModel.Info == null ? 0 : excelModel.Info.Count()) + excelModel.ExcelDetails.Count();
                    mySheet.ShiftRows(
                       startRow,
                       endRow,
                       n,
                       true,
                       false
                   );
                    ICell targetCell = null;
                    #region EXCEL 標題 STYLE
                    ICellStyle style0 = Workbook.CreateCellStyle();//建立excel style
                    style0.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
                    #endregion
                    #region EXCEL Detail標題 STYLE
                    ICellStyle style = Workbook.CreateCellStyle();//建立excel style
                    style.BorderTop = BorderStyle.Thin;
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderBottom = BorderStyle.Thin;
                    style.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
                    #endregion
                    #region EXCEL Detail STYLE
                    ICellStyle style2 = Workbook.CreateCellStyle();//建立excel style
                    style2.BorderTop = BorderStyle.Thin;
                    style2.BorderRight = BorderStyle.Thin;
                    style2.BorderLeft = BorderStyle.Thin;
                    style2.BorderBottom = BorderStyle.Thin;
                    #endregion
                    int i = 0;
                    IRow targetRows = mySheet.GetRow(i);
                    #region 標題
                    if (targetRows == null)
                    {
                        targetRows = mySheet.CreateRow(i);
                        targetCell = targetRows.CreateCell(0);
                        targetCell.SetCellValue(excelModel.Title);
                        mySheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelModel.DetailTitle.Count() - 1)); //合併儲存格
                        targetCell.CellStyle = style0;
                        //
                        i++;
                    }
                    #endregion
                    #region 詳細說明
                    if (excelModel.Info != null && excelModel.Info.Any())
                    {
                        foreach (var info in excelModel.Info)
                        {
                            targetRows = mySheet.CreateRow(i);
                            targetCell = targetRows.CreateCell(0);
                            targetCell.SetCellValue(info);
                            i++;
                        }
                    }
                    #endregion
                    #region 設定報表Detail標題

                    targetRows = mySheet.CreateRow(i);
                    for (int t = 0; t < excelModel.DetailTitle.Count(); t++)
                    {
                        targetCell = targetRows.CreateCell(t);
                        targetCell.SetCellValue(excelModel.DetailTitle[t]);
                        targetCell.CellStyle = style;
                    }
                    i++;

                    #endregion
                    #region 報表資料
                    for (int d = 0; d < excelModel.ExcelDetails.Count(); d++)
                    {
                        targetRows = mySheet.CreateRow(i);
                        for (int t = 0; t < excelModel.DetailTitle.Count(); t++)
                        {
                            targetCell = targetRows.CreateCell(t);
                            targetCell.CellStyle = style2;
                            switch (t)
                            {
                                case 0: targetCell.SetCellValue(excelModel.ExcelDetails[d].a); break;
                                case 1: targetCell.SetCellValue(excelModel.ExcelDetails[d].b); break;
                                case 2: targetCell.SetCellValue(excelModel.ExcelDetails[d].c); break;
                                case 3: targetCell.SetCellValue(excelModel.ExcelDetails[d].d); break;
                                case 4: targetCell.SetCellValue(excelModel.ExcelDetails[d].e); break;
                                case 5: targetCell.SetCellValue(excelModel.ExcelDetails[d].f); break;
                                case 6: targetCell.SetCellValue(excelModel.ExcelDetails[d].g); break;
                                case 7: targetCell.SetCellValue(excelModel.ExcelDetails[d].h); break;
                                case 8: targetCell.SetCellValue(excelModel.ExcelDetails[d].i); break;
                                case 9: targetCell.SetCellValue(excelModel.ExcelDetails[d].j); break;
                                    //  case 10: targetCell.SetCellValue(excelModel.ExcelDetails[d].k); break;
                            }
                        }
                        i++;
                    }
                    #endregion
                }
                MemoryStream ms = new MemoryStream();
                Workbook.Write(ms,true);
                ms.Flush();
                return ms;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 公版報表多個sheet
        /// </summary>
        /// <param name="excelModel"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MemoryStream ExampleReport(List<ExcelModel> excelModel, string fileName)
        {
            IWorkbook Workbook = NPOIExtensions.OpenExcel(fileName);
            int startSheet = 0;
            #region EXCEL 標題 STYLE
            ICellStyle style0 = Workbook.CreateCellStyle();//建立excel style
            style0.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
            #endregion
            #region EXCEL Detail標題 STYLE
            ICellStyle style = Workbook.CreateCellStyle();//建立excel style
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
            #endregion
            #region EXCEL Detail STYLE
            ICellStyle style2 = Workbook.CreateCellStyle();//建立excel style
            style2.BorderTop = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Thin;
            #endregion
            foreach (var sheetData in excelModel.Where(x => x.ExcelDetails != null))
            {
                #region 基礎設定
                int startRow = 0;
                startRow = startRow > 0 ? startRow - 1 : 0;
                if (startSheet > 0) {
                    Workbook.CreateSheet(sheetData.SheetTitle);
                } else {
                    Workbook.SetSheetName(startSheet, sheetData.SheetTitle);
                }
                ISheet mySheet = Workbook.GetSheetAt(startSheet);
                var endRow = startRow + (sheetData.Info == null ? 0 : sheetData.Info.Count()) + 1 + sheetData.ExcelDetails.Count();
                var n = startRow + (sheetData.Info == null ? 0 : sheetData.Info.Count()) + sheetData.ExcelDetails.Count();
                mySheet.ShiftRows(
                   startRow,
                   endRow,
                   n,
                   true,
                   false
               );
                ICell targetCell = null;
                int i = 0;
                IRow targetRows = mySheet.GetRow(i); 
                #endregion
                #region 標題
                if (targetRows == null)
                {
                    targetRows = mySheet.CreateRow(i);
                    targetCell = targetRows.CreateCell(0);
                    targetCell.SetCellValue(sheetData.Title);
                    mySheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, sheetData.DetailTitle.Count() - 1)); //合併儲存格
                    targetCell.CellStyle = style0;
                    //
                    i++;
                }
                #endregion
                #region 詳細說明
                if (sheetData.Info != null && sheetData.Info.Any())
                {
                    foreach (var info in sheetData.Info)
                    {
                        targetRows = mySheet.CreateRow(i);
                        targetCell = targetRows.CreateCell(0);
                        targetCell.SetCellValue(info);
                        mySheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, sheetData.DetailTitle.Count() - 1)); //合併儲存格
                        i++;
                    }
                }
                #endregion
                #region 設定報表Detail標題

                targetRows = mySheet.CreateRow(i);
                for (int t = 0; t < sheetData.DetailTitle.Count(); t++)
                {
                    targetCell = targetRows.CreateCell(t);
                    targetCell.SetCellValue(sheetData.DetailTitle[t]);
                    targetCell.CellStyle = style;
                }
                i++;

                #endregion
                #region 報表資料
                for (int d = 0; d < sheetData.ExcelDetails.Count(); d++)
                {
                    targetRows = mySheet.CreateRow(i);
                    for (int t = 0; t < sheetData.DetailTitle.Count(); t++)
                    {
                        targetCell = targetRows.CreateCell(t);
                        targetCell.CellStyle = style2;
                        switch (t)
                        {
                            case 0: targetCell.SetCellValue(sheetData.ExcelDetails[d].a); break;
                            case 1: targetCell.SetCellValue(sheetData.ExcelDetails[d].b); break;
                            case 2: targetCell.SetCellValue(sheetData.ExcelDetails[d].c); break;
                            case 3: targetCell.SetCellValue(sheetData.ExcelDetails[d].d); break;
                            case 4: targetCell.SetCellValue(sheetData.ExcelDetails[d].e); break;
                            case 5: targetCell.SetCellValue(sheetData.ExcelDetails[d].f); break;
                            case 6: targetCell.SetCellValue(sheetData.ExcelDetails[d].g); break;
                            case 7: targetCell.SetCellValue(sheetData.ExcelDetails[d].h); break;
                            case 8: targetCell.SetCellValue(sheetData.ExcelDetails[d].i); break;
                            case 9: targetCell.SetCellValue(sheetData.ExcelDetails[d].j); break;
                            case 10: targetCell.SetCellValue(sheetData.ExcelDetails[d].k); break;
                            case 11: targetCell.SetCellValue(sheetData.ExcelDetails[d].l); break;
                            case 12: targetCell.SetCellValue(sheetData.ExcelDetails[d].m); break;
                        }
                    }
                    i++;
                }
                #endregion
                //
                startSheet++;
            }
            MemoryStream ms = new MemoryStream();
            Workbook.Write(ms, true);
            ms.Flush();
            return ms;
        }

        /// <summary>
        /// 檔案下載數統計
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="excelModel"></param>
        /// <param name="Lang"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        public static MemoryStream SheetReportByFile(string fileName, ExcelModel excelModel, List<string> Lang, List<string[]> website)
        {
            try
            {
                int startSheet = 0;
                int startRow = 0;
                int x = 0;
                IWorkbook Workbook = NPOIExtensions.OpenExcel(fileName);

                foreach (var site in website)
                {
                    foreach (var s in Lang)
                    {
                        startSheet = x;
                        startRow = x;

                        var name = s.ToUpper() == "ZH-TW" ? "中文" : "英文";

                        if (x > 0)
                        {
                            Workbook.CreateSheet(site[1] + "(" + name + ")");
                        }
                        else
                        {
                            Workbook.SetSheetName(startSheet, site[1] + "(" + name + ")");
                        }

                        ISheet mySheet = Workbook.GetSheetAt(startSheet);
                        var endRow = startRow + (excelModel.Info == null ? 0 : excelModel.Info.Count()) + 1;
                        var n = startRow + (excelModel.Info == null ? 0 : excelModel.Info.Count());
                        mySheet.ShiftRows(
                            startRow,
                            endRow,
                            n,
                            true,
                            false
                            );
                        ICell targetCell = null;
                        #region EXCEL 標題 STYLE
                        ICellStyle style0 = Workbook.CreateCellStyle();//建立excel style
                        style0.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
                        #endregion

                        #region EXCEL Detail標題 STYLE
                        ICellStyle style = Workbook.CreateCellStyle();//建立excel style
                        style.BorderTop = BorderStyle.Thin;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Thin;
                        style.Alignment = HorizontalAlignment.CenterSelection; //文字水平置中 
                        #endregion

                        #region EXCEL Detail STYLE
                        ICellStyle style2 = Workbook.CreateCellStyle();//建立excel style
                        style2.BorderTop = BorderStyle.Thin;
                        style2.BorderRight = BorderStyle.Thin;
                        style2.BorderLeft = BorderStyle.Thin;
                        style2.BorderBottom = BorderStyle.Thin;
                        #endregion

                        int i = 0;
                        IRow targetRows = mySheet.GetRow(i);
                        targetRows = mySheet.CreateRow(i);

                        #region 詳細說明
                        if (excelModel.Info != null && excelModel.Info.Any())
                        {
                            foreach (var info in excelModel.Info)
                            {
                                targetRows = mySheet.CreateRow(i);
                                targetCell = targetRows.CreateCell(0);
                                //mySheet.AutoSizeColumn((short)i);
                                //mySheet.SetColumnWidth(i, mySheet.GetColumnWidth(i) * 4 / 10);
                                targetCell.SetCellValue(info);
                                i++;
                            }
                        }
                        #endregion

                        #region 標題
                        if (targetRows == null)
                        {
                            targetRows = mySheet.CreateRow(i);
                            targetCell = targetRows.CreateCell(0);
                            targetCell.SetCellValue(excelModel.Title);
                            //mySheet.AutoSizeColumn((short)i);
                            //mySheet.SetColumnWidth(i, mySheet.GetColumnWidth(i) * 17 / 10);
                            //mySheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelModel.DetailTitle.Count() - 1)); //合併儲存格
                            targetCell.CellStyle = style0;
                            i++;
                        }
                        #endregion

                        #region 設定報表Detail標題
                        targetRows = mySheet.CreateRow(i);
                        for (int t = 0; t < excelModel.DetailTitle.Count(); t++)
                        {
                            targetCell = targetRows.CreateCell(t);
                            //mySheet.AutoSizeColumn((short)t);
                            if (excelModel.DetailTitle[t] == "檔案路徑")
                            {
                                mySheet.SetColumnWidth(t, (int)(100 + 0.72) * 256);
                            }
                            //mySheet.SetColumnWidth(t, mySheet.GetColumnWidth(t) * 4 / 10);
                            targetCell.SetCellValue(excelModel.DetailTitle[t]);
                            targetCell.CellStyle = style;
                        }
                        i++;
                        #endregion

                        #region 報表資料
                        var DD = excelModel.ExcelDetails.Where(x => x.e == site[0] && x.d.ToUpper() == s).OrderByDescending(x => x.c).ToList();
                        for (int d = 0; d < DD.Count(); d++)
                        {
                            targetRows = mySheet.CreateRow(i);
                            for (int t = 0; t < excelModel.DetailTitle.Count(); t++)
                            {
                                targetCell = targetRows.CreateCell(t);
                                if (excelModel.DetailTitle[t] == "檔案路徑")
                                {
                                    mySheet.SetColumnWidth(t, (int)(100 + 0.72) * 256);
                                }
                                //mySheet.AutoSizeColumn((short)t);
                                //mySheet.SetColumnWidth(t, mySheet.GetColumnWidth(t) * 4 / 10);
                                targetCell.CellStyle = style2;

                                switch (t)
                                {
                                    case 0: targetCell.SetCellValue(d + 1); break;
                                    case 1: targetCell.SetCellValue(DD[d].a); break;
                                    case 2: targetCell.SetCellValue(DD[d].b); break;
                                    case 3: targetCell.SetCellValue(DD[d].c); break;
                                    case 4: targetCell.SetCellValue(DD[d].d); break;
                                    case 5: targetCell.SetCellValue(DD[d].e); break;
                                    case 6: targetCell.SetCellValue(DD[d].f); break;
                                    case 7: targetCell.SetCellValue(DD[d].g); break;
                                    case 8: targetCell.SetCellValue(DD[d].h); break;
                                    case 9: targetCell.SetCellValue(DD[d].i); break;
                                    case 10: targetCell.SetCellValue(DD[d].j); break;
                                }
                            }
                            i++;
                        }
                        #endregion
                        x++;
                    }
                }

                MemoryStream ms = new MemoryStream();
                Workbook.Write(ms, true);
                ms.Flush();
                return ms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
    /// <summary>
    /// 元件
    /// </summary>
    public static class NPOIExtensions
    {
        public static IWorkbook OpenExcel(string FileName)
        {
            try
            {
                IWorkbook MyWorkBook;

                Stream MyExcelStream = OpenClasspathResource(FileName);

                if (FileName.ToLower().IndexOf(".xlsx") > 0)
                {
                    MyExcelStream.Position = 0;
                    MyWorkBook = new XSSFWorkbook(MyExcelStream);
                    MyExcelStream.Dispose();
                }
                else
                {
                    MyWorkBook = new HSSFWorkbook(MyExcelStream);
                    MyExcelStream.Dispose();
                }

                return MyWorkBook;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IWorkbook OpenExcel(Stream sm, string type = "xls")
        {
            try
            {
                IWorkbook MyWorkBook;

                if (type == "xlsx")
                {
                    MyWorkBook = new XSSFWorkbook(sm);
                }
                else
                {
                    MyWorkBook = new HSSFWorkbook(sm);
                }

                return MyWorkBook;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Stream OpenClasspathResource(String fileName)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class ExcelModel
    {
        /// <summary>
        /// excel 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 詳細說明
        /// </summary>
        public List<string> Info { get; set; }
        /// <summary>
        /// 報表標題
        /// </summary>
        public List<string> DetailTitle { get; set; }
        /// <summary>
        /// 報表內文
        /// </summary>
        public List<ExcelDetailModel> ExcelDetails { get; set; }
        /// <summary>
        /// Sheet標題
        /// </summary>
        public string SheetTitle { get; set; }
    }
    /// <summary>
    /// 報表內文模型
    /// </summary>
    public class ExcelDetailModel
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
        public string e { get; set; }
        public string f { get; set; }
        public string g { get; set; }
        public string h { get; set; }
        public string i { get; set; }
        public string j { get; set; }
        public string k { get; set; }
        public string l { get; set; }
        public string m { get; set; }
    }
}
