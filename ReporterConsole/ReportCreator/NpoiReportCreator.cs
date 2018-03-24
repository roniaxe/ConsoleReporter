using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using ReporterConsole.Utils;

namespace ReporterConsole.ReportCreator
{
    class NpoiReportCreator : IReportCreator<DataTable>
    {
        private readonly ILogger<NpoiReportCreator> _logger;
        private readonly IConfigurationRoot _configuration;

        public NpoiReportCreator(ILogger<NpoiReportCreator> logger, IConfigurationRoot configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> CreateReportAsync()
        {
            var newFile = _configuration.GetSection("ReportsLocation").Value
                          + $@"BatchesDailySummary_{Program.ReporterArgs.Environment}_{Program.ReporterArgs.FromDate:M}.xlsx";

            var data = await QueryManager.GetQueriesResultList();

            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                foreach (var dataTable in data)
                {
                    ISheet sheet1 = workbook.CreateSheet(dataTable.TableName);
                    //sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    var rowIndex = 0;
                    IRow headerRow = sheet1.CreateRow(rowIndex);
                    headerRow.Height = 30 * 80;

                    var headerStyle = workbook.CreateCellStyle();
                    headerStyle.FillForegroundColor = HSSFColor.Blue.Index2;
                    headerStyle.FillPattern = FillPattern.SolidForeground;

                    List<ICell> headerCells = new List<ICell>();
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var headerCell = headerRow.CreateCell(i);
                        headerCell.SetCellValue(dataTable.Columns[i].ColumnName);
                        headerCells.Add(headerCell);
                        sheet1.AutoSizeColumn(i);
                    }
                    rowIndex++;
                    headerRow.RowStyle = headerStyle;                   
                    
                    for (int i = rowIndex; i < dataTable.Rows.Count; rowIndex++)
                    {
                        IRow dataRow = sheet1.CreateRow(rowIndex);
                        for (int j = 0; j < headerCells.Count; j++)
                        {
                            dataRow.CreateCell(j).SetCellValue(dataTable.Rows[rowIndex][j].ToString());
                            sheet1.AutoSizeColumn(j);
                        }

                        //rowIndex++;
                    }
                }
                workbook.Write(fs);
            }

            return newFile;
        }
    }
}
