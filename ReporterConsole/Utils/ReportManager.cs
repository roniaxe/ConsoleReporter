using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReporterConsole.Utils
{
    public static class ReportManager
    {
        public static string ExportDataSet(List<DataTable> dataTables)
        {
            var destination = $@"C:\DailyBatchReports\BatchesDailySummary_PROD_{DateTime.Today:M}.xlsx";
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };

                foreach (DataTable table in dataTables) {

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Any())
                    {
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns) {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(column.ColumnName)
                        };
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {                       
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            var type = GetTypeOf(dsrow[col]);
                            Cell cell = new Cell
                            {
                                DataType = type == ValueType.String ? CellValues.String :
                                type == ValueType.Integer ? CellValues.Number : 
                                type == ValueType.Date ? CellValues.Date : CellValues.InlineString,
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

        private static ValueType? GetTypeOf(object value)
        {
            ValueType? typeOfValue = null;
            if (value is string)
            {
                typeOfValue = ValueType.String;
            }

            if (value is int)
            {
                typeOfValue = ValueType.Integer;
            }

            if (value is bool)
            {
                typeOfValue = ValueType.Boolean;
            }

            if (value is DateTime)
            {
                typeOfValue = ValueType.Date;
            }

            return typeOfValue;
        }
    }

    enum ValueType
    {
        String,
        Integer,
        Boolean,
        Date
    }
}
