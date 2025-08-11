using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Table.Download
{
    public class TableDownloader : MonoBehaviour
    {
        public string FileName = "fileName";
        public static void Download(Texture2D logoForTable, Table.UI.Views.TableView tableView, string fileName)
        {
            if (fileName == string.Empty)
                fileName = tableView.Table.TableName;

            ExcelExporter.ExcelFileWorkbook excelFileWorkbook = new ExcelExporter.ExcelFileWorkbook();
            ExcelExporter.SheetData currentSheet = new ExcelExporter.SheetData()
            {
                headers = new List<string>(),
                rows = new List<ExcelExporter.RowData>(),
            };
            currentSheet.name = tableView.Table.TableName;
            currentSheet.description = tableView.Table.TableDescription;
            currentSheet.SetLogo(logoForTable);
            excelFileWorkbook.sheets = new List<ExcelExporter.SheetData>
            {
                currentSheet
            };

            for (int i = 0; i < tableView.Table.RecordsCount(); i++)
            {
                List<string> rowCells = new List<string>();
                for (int j = 0; j < tableView.Table.GetRecord(i).TableFields.Length; j++)
                {
                    if (i == 0)
                        rowCells.Add(tableView.Table.GetRecord(i).TableFields[j].fieldID);
                    else
                        rowCells.Add(tableView.Table.GetRecord(i).TableFields[j].Value);
                }
                if (i == 0)
                    currentSheet.headers = rowCells;
                else
                    currentSheet.rows.Add(new ExcelExporter.RowData() { row = rowCells.ToArray() });

            }

            ExcelExporter.ExportWorkbook(excelFileWorkbook, fileName);
        }
    }
}