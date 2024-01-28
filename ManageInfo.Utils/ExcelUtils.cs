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
                        ObservableCollection<string> row = new ObservableCollection<string>(fields);
                        data.Add(row);
                    }
                }
            }

            return data;
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
