using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Table.Download
{
    public static class TableWindowDownloader
    {
        public static void Download(Texture2D logoForTableExcel, Table.UI.Views.TablesWindow tablesView, string fileName)
        {
            ExcelExporter.ExcelFileWorkbook excelFileWorkbook = new ExcelExporter.ExcelFileWorkbook();


            excelFileWorkbook.sheets = new List<ExcelExporter.SheetData>();
            foreach (var view in tablesView.TablesInView)
            {
                ExcelExporter.SheetData currentSheet = new ExcelExporter.SheetData()
                {
                    headers = new List<string>(),
                    rows = new List<ExcelExporter.RowData>()
                };
                currentSheet.name = view.Table.TableName;
                currentSheet.description = view.Table.TableDescription;
                currentSheet.SetLogo(logoForTableExcel);
                for (int i = 0; i < view.Table.RecordsCount(); i++)
                {
                    List<string> rowCells = new List<string>();
                    for (int j = 0; j < view.Table.GetRecord(i).TableFields.Length; j++)
                    {
                        if (i == 0)
                            rowCells.Add(view.Table.GetRecord(i).TableFields[j].fieldID);
                        else
                            rowCells.Add(view.Table.GetRecord(i).TableFields[j].Value);
                    }
                    if (i == 0)
                        currentSheet.headers = rowCells;
                    else
                        currentSheet.rows.Add(new ExcelExporter.RowData() { row = rowCells.ToArray() });

                }
                excelFileWorkbook.sheets.Add(currentSheet);
            }
            ExcelExporter.ExportWorkbook(excelFileWorkbook, fileName);

        }
    }
}