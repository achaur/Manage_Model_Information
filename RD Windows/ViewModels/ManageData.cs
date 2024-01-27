using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                _column1="example", 
                _column2="example", 
                _column3="example", 
                _column4="example", 
                _column5="example" 
            }
        };
        public static ObservableCollection<RowData> GetRowsData()
        {
            return _RowsData;
        }
        public static RowData GetRowDataAtIndex(int index)
        {
            return _RowsData[index];
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
