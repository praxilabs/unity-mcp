using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Table.Download;
namespace Table.Download.Test
{
    public class ExcelExporterTest : MonoBehaviour
    {
        public Texture2D LogoTexture;  // Use a Unity Texture2D for the logo
        
        public void ExportTest()
        {
            string logoBase64 = ExcelExporter.ConvertLogoToBase64(LogoTexture);

            var workbook = new ExcelExporter.ExcelFileWorkbook()
            {
                sheets = new List<ExcelExporter.SheetData>()
            {
                new ExcelExporter.SheetData()
                {
                    name = "EnglishSheet",
                    logoBase64 = logoBase64,
                    description = "<b>This is an <i>optional</i> description with <color=#ff0000>colored</color>, <u>underlined</u>, <sup>superscript</sup> text, and <i><b>nested tags</b></i>.</b>",
                    headers = new List<string>() { "Formula", "Description" },
                    rows = new List<ExcelExporter.RowData>()
                    {
                        new ExcelExporter.RowData() { row = new string[] { "H<sub>2</sub>O", "Water molecule with subscript" } },
                        new ExcelExporter.RowData() { row = new string[] { "E = mc<sup>2</sup>", "Energy equation with superscript" } },
                        new ExcelExporter.RowData() { row = new string[] { "<b>Bold and <i>Italic</i></b>", "Example of nested tags" } }
                    }
                },
                new ExcelExporter.SheetData()
                {
                    name = "ArabicSheet",
                    logoBase64 = logoBase64,
                    description = "<b>هذا وصف <i>اختياري</i> مع نص <color=#ff0000>ملون</color> و <u>تحته خط</u> و <sup>نص علوي</sup>.</b>",
                    headers = new List<string>() { "معادلة", "وصف" },
                    rows = new List<ExcelExporter.RowData>()
                    {
                        new ExcelExporter.RowData() { row = new string[] { "H<sub>2</sub>O", "جزيء الماء" } },
                        new ExcelExporter.RowData() { row = new string[] { "E = mc<sup>2</sup>", "معادلة الطاقة" } }
                    }
                }
            }
            };
            foreach (ExcelExporter.SheetData sheetData in workbook.sheets)
                sheetData.SetLogo(LogoTexture);
           ExcelExporter.ExportWorkbook(workbook);
        }


        

        [DllImport("__Internal")]
        private static extern void ConvertTableToExcel(string jsonData, string fileName);
    }
}