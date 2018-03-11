using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using ReporterConsole.Exceptions;

namespace ReporterConsole.Utils
{
    class ExcelReportCreator : IReportCreator<DataTable>
    {
        private readonly ILogger<ExcelReportCreator> _logger;
        private readonly string _reportLocation;
        private List<DataTable> _dataSource;

        public ExcelReportCreator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExcelReportCreator>();
            _reportLocation = AppConfig.Configuration.GetSection("ReportsLocation").Value 
                              + $@"BatchesDailySummary_{Program.ReporterArgs.Environment}_{Program.ReporterArgs.FromDate:M}.xlsx";            
        }

        public async Task<string> CreateReportAsync()
        {
            _logger.LogInformation("Creating Report...");
            if (_reportLocation == null)
            {
                _logger.LogError("Can't Find Attachment Location, Please Config It In config.json File.");
                throw new MissingAttachmentLocationException("Can't Find Attachment Location, Please Config It In config.json File.");
            }
            _logger.LogInformation("Querying The Database Async...");
            _dataSource = await QueryManager.GetQueriesResultList();
            using (var workbook = SpreadsheetDocument.Create(_reportLocation, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };
                uint sheetId = 1;
                foreach (var table in _dataSource)
                {
                    _logger.LogInformation($@"Creating {table.TableName} Sheet...");
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
            return _reportLocation;
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
