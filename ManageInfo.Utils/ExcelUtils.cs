using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace ManageInfo_Utils
{
    public class ExcelUtils
    {
        public static List<List<string>> ExcelToRowData(string filePath)
        {
            List<List<string>> data = new List<List<string>>();

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
                        List<string> row = new List<string>(fields);
                        data.Add(row);
                    }
                }
            }

            return data;
        }
        public static void RowDataToExcel(string filePath, List<List<string>> rowData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (List<string> row in rowData)
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
