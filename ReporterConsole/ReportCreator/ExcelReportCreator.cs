using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReporterConsole.DTOs;
using ReporterConsole.Exceptions;
using ReporterConsole.Utils;

namespace ReporterConsole.ReportCreator
{
    class ExcelReportCreator : IReportCreator<DataTable>
    {
        private readonly ILogger<ExcelReportCreator> _logger;
        private readonly AppSettings _configurations;

        public Task<List<DataTable>> Data { get; set; }

		public ExcelReportCreator(ILoggerFactory loggerFactory, IOptions<AppSettings> configuration)
        {
            _logger = loggerFactory.CreateLogger<ExcelReportCreator>();
            _configurations = configuration.Value;            	
		}

        public async Task<string> CreateReportAsync()
        {
            Data = QueryManager.GetQueriesResultList();           

            if (string.IsNullOrWhiteSpace(_configurations.ReportsLocation))
            {
                _logger.LogError("Can't Find Attachment Location, Please Config It In config.json File.");
                throw new MissingAttachmentLocationException("Can't Find Attachment Location, Please Config It In config.json File.");
            }

            var reportLocation = _configurations.ReportsLocation
                                 + $@"BatchesDailySummary_{Program.ReporterArgs.Environment}_{Program.ReporterArgs.FromDate:M}.xlsx";

            using (var workbook = SpreadsheetDocument.Create(reportLocation, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };
                uint sheetId = 1;
                foreach (var table in await Data)
                {
                    _logger.LogInformation($@"Creating Table - {table.TableName}");
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);


                    if (sheets.Elements<Sheet>().Any())
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    var headerRow = new Row();

                    var columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(column.ColumnName)
                        };
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        var newRow = new Row();
                        foreach (var col in columns)
                        {
                            //var type = GetTypeOf(dsrow[col]);
                            var type = GetTypeOfCellValue(dsrow[col]);
                            var cell = new Cell
                            {
                                DataType = type,
                                CellValue = new CellValue(dsrow[col].ToString())
                            };
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
            _logger.LogInformation("Done Creating Report!");
            return reportLocation;
        }

        private static CellValues? GetTypeOfCellValue(object o)
        {
            CellValues? returnType = null;
            switch (o)
            {
                case int _:
                    returnType = CellValues.Number;
                    break;
                case string _:
                    returnType = CellValues.String;
                    break;
                case bool _:
                    returnType = CellValues.Boolean;
                    break;
                case DateTime _:
                    returnType = CellValues.Date;
                    break;
            }

            return returnType;
        }

        
    }
    internal enum ValueType
    {
        String,
        Integer,
        Boolean,
        Date
    }
}
