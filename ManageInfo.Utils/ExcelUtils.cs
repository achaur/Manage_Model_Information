using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace ManageInfo_Utils
{
    public class ExcelUtils
    {
        public static List<List<string>> ExcelToRowData(string filePath)
        {
            List<List<string>> parsedExcelFile = new List<List<string>>();

            //parse file

            return parsedExcelFile;
        }
        public static void RowDataToExcel(string filePath, List<List<string>> rowData)
        {
            try
            {
                //parse file
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    // Add a new worksheet to the Excel file
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    object[,] dataToWriteToExcel = new object[rowData.Count, 11];

                    // Sample data to write to the worksheet
                    List<object[]> data = new List<object[]>();

                    foreach (List<string> row in rowData)
                    {
                        data.Add(row.ToArray()); 
                    }

                    List<object[]> d = new List<object[]>();
                    d.Add(new object[1] { "f" });

                    // Define the range where the data will be written
                    string writeRange = "A1";

                    // Write data to the worksheet
                    worksheet.Cells[writeRange].LoadFromArrays(d);

                    // Save the changes to the Excel file
                    package.Save();
                }
            }
            catch(Exception ex)
            { 
                string s = ex.Message;
            }
        }
    }
}
