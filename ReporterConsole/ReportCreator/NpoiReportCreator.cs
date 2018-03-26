using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ReporterConsole.DTOs;
using ReporterConsole.Exceptions;
using ReporterConsole.Utils;

namespace ReporterConsole.ReportCreator
{
    class NpoiReportCreator : IReportCreator<DataTable>
    {
        private const string SpacingCamelCaseWordPattern = "(\\B[A-Z])";
        private readonly ILogger<NpoiReportCreator> _logger;
        private readonly AppSettings _configuration;
        public Task<List<DataTable>> Data { get; set; }

        public NpoiReportCreator(ILogger<NpoiReportCreator> logger, IOptions<AppSettings> configuration)
        {
            _logger = logger;
            _configuration = configuration.Value;
        }

        public async Task<string> CreateReportAsync()
        {
            Data = QueryManager.GetQueriesResultList();

            if (string.IsNullOrWhiteSpace(_configuration.ReportsLocation))
            {
                _logger.LogError("Can't Find Attachment Location, Please Config It In config.json File.");
                throw new MissingAttachmentLocationException("Can't Find Attachment Location, Please Config It In config.json File.");
            }

            var newFile = _configuration.ReportsLocation
                          + $@"BatchesDailySummary_{Program.ReporterArgs.Environment}_{Program.ReporterArgs.FromDate:M}.xlsx";

            //var data = await QueryManager.GetQueriesResultList();

            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                foreach (var dataTable in await Data)
                {
                    _logger.LogInformation($@"Creating Table - {dataTable.TableName}");
                    ISheet sheet1 = workbook.CreateSheet(dataTable.TableName);
                    IRow headerRow = sheet1.CreateRow(0);


                    // set bold font
                    var boldFont = CreateBoldFont(workbook);

                    // set header cell style
                    var headerStyle = CreateHeaderStyle(workbook);
                    headerStyle.SetFont(boldFont);

                    List<ICell> headerCells = new List<ICell>();
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var headerCell = headerRow.CreateCell(i);
                        headerCell.SetCellValue(Regex.Replace(dataTable.Columns[i].ColumnName, SpacingCamelCaseWordPattern, " $1"));
                        headerCell.CellStyle = headerStyle;
                        headerCells.Add(headerCell);
                    }

                    for (int i = 1; i < dataTable.Rows.Count; i++)
                    {
                        IRow dataRow = sheet1.CreateRow(i);
                        for (int j = 0; j < headerCells.Count; j++)
                        {
                            var dataCell = dataRow.CreateCell(j);
                            var value = dataTable.Rows[i][j];
                            if (value is int) dataCell.SetCellValue((int)dataTable.Rows[i][j]);
                            if (value is DateTime) dataCell.SetCellValue(((DateTime)dataTable.Rows[i][j]).ToString(CultureInfo.InvariantCulture));
                            if (value is bool) dataCell.SetCellValue((bool)dataTable.Rows[i][j]);
                            if (value is string) dataCell.SetCellValue((string)dataTable.Rows[i][j]);
                        }
                    }

                    for (int i = 0; i < headerCells.Count; i++)
                    {
                        sheet1.AutoSizeColumn(i);
                    }
                }
                workbook.Write(fs);
            }

            return newFile;
        }

        private static IFont CreateBoldFont(IWorkbook workbook)
        {
            var boldFont = workbook.CreateFont();
            boldFont.Boldweight = (short)FontBoldWeight.Bold;
            return boldFont;
        }

        private static ICellStyle CreateHeaderStyle(IWorkbook workbook)
        {
            var headerStyle = workbook.CreateCellStyle();
            headerStyle.BorderBottom = BorderStyle.MediumDashDot;
            headerStyle.FillForegroundColor = 42;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            return headerStyle;
        }
    }
}
