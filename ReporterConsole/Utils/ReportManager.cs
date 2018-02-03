using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReporterConsole.Utils
{
    public static class ReportManager
    {
        public static string ExportDataSet(List<DataTable> dataTables)
        {
            var destination = $@"C:\DailyBatchReports\BatchesDailySummary_PROD_{DateTime.Today:M}.xlsx";
            using (var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };
                uint sheetId = 1;
                foreach (var table in dataTables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);


                    if (sheets.Elements<Sheet>().Any())
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    var sheet = new Sheet {Id = relationshipId, SheetId = sheetId, Name = table.TableName};
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

            return destination;
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