using Autodesk.Revit.DB.Analysis;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ManageInfo_Windows
{
    public class RowData
    {
        public List<string> _rowData;
        public int _columnNumber = 11;
        public RowData()
        {
            _rowData = new List<string>();

            for (int i = 0; i < _columnNumber; i++)
            {
                _rowData.Add("");
            }
        }

        public int GetColumnNumber()
        {
            return _rowData?.Count ?? 0;
        }

        public void AddColumn()
        {
            _rowData.Add("");
            _columnNumber++;
        }

        public string? _column0 { get; set; }
        public string? _column1 { get; set; }
        public string? _column2 { get; set; }
        public string? _column3 { get; set; }
        public string? _column4 { get; set; }
        public string? _column5 { get; set; }

        //define calculation rules for calculation fields
        public string? _calculation1 { get; set; }
        public string? _calculation2 { get; set; }
        public string? _calculation3 { get; set; }
        public string? _calculation4 { get; set; }
        public string? _calculation5 { get; set; }
    }
}
