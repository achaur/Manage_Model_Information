using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Windows.ViewModels
{
    public class ManageData
    {
        public static ObservableCollection<RowData> _RowsData = new ObservableCollection<RowData>()
        {
            new RowData()
            {
                _column0 ="1",
                _column1="example", 
                _column2="example", 
                _column3="example", 
                _column4="example", 
                _column5="example"
            }
        };

        public static void UpdateCalculations()
        {
            foreach (RowData rowData in _RowsData)
            {
                rowData._calculation1 = rowData._column1;
                rowData._calculation2 = rowData._column2;
                rowData._calculation3 = rowData._column3;
                rowData._calculation4 = rowData._column4;
                rowData._calculation5 = rowData._column5;
            }
        }

        public static ObservableCollection<RowData> GetRowsData()
        {
            return _RowsData;
        }
        public static RowData GetRowDataAtIndex(int index)
        {
            if (null != _RowsData)
            {
                if (_RowsData.Count() > index && index >= 0)
                {
                    return _RowsData[index];
                }
            }
            return null;
        }

        public static List<List<string>> RowDataToStringLists()
        { 
            if (null != _RowsData)
            { 
                List<List<string>> dataConverted = new List<List<string>>();

                foreach(RowData rowData in _RowsData)
                {
                    List<string> rowDataConverted = new List<string> 
                    { 
                        rowData._column0,
                        rowData._column1,
                        rowData._column2,
                        rowData._column3,
                        rowData._column4,
                        rowData._column5,
                        rowData._calculation1,
                        rowData._calculation2,
                        rowData._calculation3,
                        rowData._calculation4,
                        rowData._calculation5
                    };
                    dataConverted.Add(rowDataConverted);
                }
                return dataConverted;
            }
            return null;
        }

        public static List<RowData> StringListsToRowData(List<List<string>> excelData)
        {
            if (null != excelData)
            {
                List<RowData> dataConverted = new List<RowData>();

                foreach (List<string> data in excelData)
                {
                    RowData rowData = new RowData()
                    { 
                        _column0 = data[0],
                        _column1= data[1],
                        _column2= data[2],
                        _column3= data[3],
                        _column4= data[4],
                        _column5= data[5]
                    };
                    dataConverted.Add(rowData);
                }
                return dataConverted;
            }
            return null;
        }

        public static void AddRowData(RowData rowData)
        {
            _RowsData.Add(rowData);
        }
        public static void DeleteRowData(int index)
        {
            if (null != _RowsData)
            {
                if (_RowsData.Count() > index && index >= 0)
                { 
                    _RowsData.RemoveAt(index);
                }
            }
        }
    }
}
