using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Microsoft.VisualBasic.FileIO;
using OfficeOpenXml;
using static System.Net.WebRequestMethods;

namespace ManageInfo_Utils
{
    public class ExcelUtils
    {
        public static ObservableCollection<ObservableCollection<string>> ExcelToRowData(string filePath)
        {
            ObservableCollection<ObservableCollection<string>> data = 
                new ObservableCollection<ObservableCollection<string>>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    // Read current line from the CSV file
                    string[] fields = parser.ReadFields();

                    // Convert the array of strings to a list and add to the result
                    if (fields != null)
                    {
                        //read only first 6 columns of row
                        ObservableCollection<string> rowFiltered = new ObservableCollection<string>();
                        for (int i = 0; i < fields.Count() && i < 2; i++)
                        {
                            rowFiltered.Add(fields[i]);
                        }
                        data.Add(rowFiltered);
                    }
                }
            }

            return data;
        }

        public static ObservableCollection<ObservableCollection<string>> ExcelToList(string filePath)
        {
            ObservableCollection<ObservableCollection<string>> excelData = new ObservableCollection<ObservableCollection<string>>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Assume the data is in the first worksheet
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                var columnCells = worksheet.Cells
                    .Where(cell => !string.IsNullOrEmpty(cell.Text))?
                    .OrderBy(cell => cell.Start.Row);

                var firstNonBlankColumnNumber = columnCells?.FirstOrDefault()?.Start?.Column ?? 0;
                var firstNonBlankRowNumber = columnCells?.FirstOrDefault()?.Start?.Row ?? 0;

                // Iterate over rows and columns to read the data
                for (int row = firstNonBlankColumnNumber; row <= worksheet?.Dimension?.End?.Row; row++)
                {
                    ObservableCollection<string> rowData = new ObservableCollection<string>();

                    for (int col = firstNonBlankColumnNumber; col < firstNonBlankColumnNumber + 2; col++)
                    {
                        // Read the cell value as a string
                        string cellValue = worksheet.Cells[row, col].Text;

                        // Add the cell value to the current row data
                        rowData.Add(cellValue);
                    }
                    // Add the row data to the list of rows
                    if (ListContainsEmptyValues(rowData) != true)
                        excelData.Add(rowData);
                }
            }

            return excelData;
        }

        public static bool ListContainsEmptyValues(ObservableCollection<string> list)
        {
            if (null == list)
                return true;
            if (list.Count == 0)
                return true;
            if (list.Contains(null) == true)
                return true;

            int count = list.Count;
            int emptyCells = 0;

            for (int i = 0; i < count; i++)
            {
                if (list[i] == string.Empty)
                    emptyCells++;
            }
            if (emptyCells == count)
                return true;

            return false;
        }

        public static void RowDataToExcel(string filePath, 
            ObservableCollection<ObservableCollection<string>> rowData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (ObservableCollection<string> row in rowData)
                    {
                        // Join the elements of the row with commas and write to the file
                        writer.WriteLine(string.Join(",", row));
                    }
                }
            }
            catch(Exception ex)
            { 
                string s = ex.Message;
            }
        }
    }
}
