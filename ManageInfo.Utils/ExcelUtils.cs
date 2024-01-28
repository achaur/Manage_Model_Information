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
        public static ObservableCollection<ObservableCollection<int>> ExcelToRowData(string filePath)
        {
            ObservableCollection<ObservableCollection<int>> data = 
                new ObservableCollection<ObservableCollection<int>>();

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
                        ObservableCollection<int> rowFiltered = new ObservableCollection<int>();
                        for (int i = 0; i < fields.Count() && i < 6; i++)
                        {
                            int number;
                            bool success = int.TryParse(fields[i], out number);
                            if (success)
                                rowFiltered.Add(number);
                        }
                        data.Add(rowFiltered);
                    }
                }
            }

            return data;
        }
        public static void RowDataToExcel(string filePath, 
            ObservableCollection<ObservableCollection<int>> rowData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (ObservableCollection<int> row in rowData)
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
