using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
namespace Table.Download
{
    public static class ExcelExporter 
    {

        public static void ExportWorkbook(ExcelFileWorkbook workbook, string fileName = "MyExcelFile")
        {
            string jsonData = JsonUtility.ToJson(workbook);
            ConvertTableToExcel(jsonData, fileName);
        }


        public static string ConvertLogoToBase64(Texture2D texture)
        {
            byte[] imageData = texture.EncodeToPNG();
            return System.Convert.ToBase64String(imageData);
        }

        [System.Serializable]
        public class ExcelFileWorkbook
        {
            public List<SheetData> sheets;
        }

        [System.Serializable]
        public class SheetData
        {
            public string name;
            public string logoBase64;
            public string description;
            public List<string> headers;
            public List<RowData> rows;

            public void SetLogo(Texture2D texture)
            {
                logoBase64 = ConvertLogoToBase64(texture);
            }
        }

        [System.Serializable]
        public class RowData
        {
            public string[] row;
        }

        [DllImport("__Internal")]
        private static extern void ConvertTableToExcel(string jsonData, string fileName);
    }
}