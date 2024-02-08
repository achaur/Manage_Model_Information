using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Windows
{
    public class DataCell
    {
        public int Value;
        public const int _defaultValue = 1;
        public const int _lowerLimit = 0;
        public const int _upperLimit = 100;
        
        public DataCell(int i) 
        { 
            Value = i;
        }
        public int GetCell()
        {
            return Value;
        }
        public void SetCell(int i)
        {
            Value = i;
        }
        public bool DataCellWithinRange()
        { 
            return Value > _lowerLimit && Value < _upperLimit;
        }
    }
}
