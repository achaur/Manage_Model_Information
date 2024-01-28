using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Windows.ViewModels
{
    public class ManageData
    {
        //initialize data
        public static ObservableCollection<ObservableCollection<string>> _RowsData;

        public ManageData(int NumberOfColumns, int numberOfRows)
        {
            _RowsData = new ObservableCollection<ObservableCollection<string>>();
            for (int i = 0; i < numberOfRows; i++)
            {
                var row = new ObservableCollection<string>();
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    row.Add("new"); // Initialize with some default values
                }
                _RowsData.Add(row);
            }
        }

        public static void InitializeGrid(int NumberOfColumns, int numberOfRows)
        {
            _RowsData = new ObservableCollection<ObservableCollection<string>>();
            for (int i = 0; i < numberOfRows; i++)
            {
                var row = new ObservableCollection<string>();
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    row.Add("new"); // Initialize with some default values
                }
                _RowsData.Add(row);
            }
        }

        public static List<List<string>> ObservableCollectionToList()
        {
            List<List<string>> items = new List<List<string>>();

            foreach (var row in _RowsData)
            {
                List<string> rowConverted = new List<string>();
                foreach (var column in row)
                {
                    rowConverted.Add(column);
                }
                items.Add(rowConverted);
            }
            return items;
        }

        public static ObservableCollection<ObservableCollection<string>> GetRowsData()
        {
            return _RowsData;
        }
        public static ObservableCollection<string> GetRowDataAtIndex(int index)
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

        public static void AddColumn()
        {
            foreach (ObservableCollection<string> rowData in _RowsData)
            {
                rowData.Add("new");
            }
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
                        _column1 = data[1],
                        _column2 = data[2],
                        _column3 = data[3],
                        _column4 = data[4],
                        _column5 = data[5]
                    };

                    dataConverted.Add(rowData);
                }
                return dataConverted;
            }
            return null;
        }

        public static void AddRowData(ObservableCollection<string> rowData)
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
