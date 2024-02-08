using ManageInfo_Logic;
using ManageInfo_Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ManageInfo_Windows
{
    public class DataManager : BaseViewModel
    {
        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        private ObservableCollection<ObservableCollection<DataCell>> matrix;

        public ObservableCollection<ObservableCollection<DataCell>> _Matrix
        {
            get { return matrix; }
            set { 
                matrix = value; 
                OnPropertyChanged(nameof(Matrix));
            }
        }

        private int columnsNumber;

        public int ColumnsNumber
        {
            get { return columnsNumber; }
            set { 
                columnsNumber = value; 
                OnPropertyChanged(nameof(ColumnsNumber));
            }
        }


        private int rowsNumber;

        public int RowsNumber
        {
            get { return rowsNumber; }
            set 
            {
                rowsNumber = value;
                OnPropertyChanged(nameof(RowsNumber));
            }
        }


        public DataManager()
        {
            RunCommand = new CommandWindow(RunAction);
            CloseCommand = new CommandWindow(CloseAction);
            ImportExcelCommand = new CommandWindow(ImportExcelAction);
            ExportExcelCommand = new CommandGeneric(ExportExcelAction);
            AddRowCommand = new CommandGeneric(AddRowDataAction);
            RemoveRowCommand = new CommandGeneric(DeleteRowDataAction);
            CopyRowCommand = new CommandGeneric(CopyRowDataAction);
        }

        public ObservableCollection<ObservableCollection<DataCell>> GetMatrix()
        {
            return _Matrix;
        }

        public ObservableCollection<DataCell> CreateDefaultRow()
        {
            ObservableCollection<DataCell> defaultRow = new ObservableCollection<DataCell>();

            for (int j = 0; j < ColumnsNumber; j++)
            {
                if (j > 5 && j < 11)
                {
                    DataCell dataCell = new DataCell(2);
                    defaultRow.Add(dataCell);
                }
                else
                {
                    DataCell dataCell = new DataCell(1);
                    defaultRow.Add(dataCell);
                }
            }
            return defaultRow;
        }

        public void AddRow()
        {
            ObservableCollection<DataCell> row = CreateDefaultRow();
            _Matrix.Add(row);
            RowsNumber++;
        }

        public void RemoveRowAtIndex(int index)
        {
            if (index >= 0 && index < RowsNumber)
            {
                _Matrix.RemoveAt(index);
                RowsNumber--;
            }
        }

        public bool MatrixIsCorrect()
        {
            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    DataCell dataCell = _Matrix[i][j];

                    if (dataCell.DataCellWithinRange() == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void CopyRowAtIndex(int index)
        {
            if (index >= 0 && index < RowsNumber)
            {
                ObservableCollection<DataCell> row = _Matrix[index];
                _Matrix.Add(row);
                RowsNumber++;
            }
        }

        public void UpdateMatrix()
        {
            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    if (j > 5 && j < 11)
                    {
                        _Matrix[i][j] = _Matrix[i][j - 5];
                    }
                }
            }
        }

        public ObservableCollection<ObservableCollection<int>> ConvertNatrixToIntLists()
        { 
            ObservableCollection<ObservableCollection<int>> ints = new ObservableCollection<ObservableCollection<int>>();

            for (int i = 0; i < RowsNumber; i++)
            {
                ObservableCollection<int> rowInt = new ObservableCollection<int>();
                for (int j = 0; j < ColumnsNumber; j++)
                { 
                    DataCell dataCell = _Matrix[i][j];
                    int value = dataCell.Value;
                    rowInt.Add(value);
                }
                ints.Add(rowInt);
            }

            return ints;
        }

        public void ReadExcelDataToMatrix(ObservableCollection<ObservableCollection<int>> dataToRead)
        {
            ObservableCollection<ObservableCollection<DataCell>> readData = new ObservableCollection<ObservableCollection<DataCell>>();

            foreach (ObservableCollection<int> row in dataToRead)
            {
                //initialize empty row
                ObservableCollection<DataCell> rowWithCalculatedValues = CreateDefaultRow();
                //fill row with imported values
                for (int i = 0; i < row.Count; i++)
                {
                    int value = row[i];
                    DataCell dataCell = new DataCell(value);
                    rowWithCalculatedValues[i] = dataCell;
                }
                //fill row with calculated values based on import
                for (int i = 1; i < row.Count && i + 5 < columnsNumber; i++)
                {
                    int value = row[i] * 2;
                    DataCell dataCell = new DataCell(value);
                    rowWithCalculatedValues[i + 5] = dataCell;
                }
                _Matrix.Add(rowWithCalculatedValues);
                RowsNumber++;
            }
        }

        private ManageInformationModel _model;
        public ManageInformationModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public override void SetInitialData()
        {
            _Matrix = new ObservableCollection<ObservableCollection<DataCell>>();
            ColumnsNumber = 11;
            RowsNumber = 0;
            AddRow();

            ErrorMessage = "";
            Model = (ManageInformationModel)BaseModel;
        }

        public ICommand ImportExcelCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand AddRowCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }
        public ICommand CopyRowCommand { get; set; }

        private protected override void RunAction(Window window)
        {
            //Model.RowData = Items.Select(x => x.ToList()).ToList();

            Model.Run();
            CloseAction(window);
        }

        private protected override void CloseAction(Window window)
        {
            if (null != window)
            {
                Closed = true;
                window.Close();
            }
        }

        private void ImportExcelAction(Window window)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Please select a folder...",
                Filter = "csv Source Files | *.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? success = openFileDialog.ShowDialog();

            if (success == true)
            {
                string pathName = openFileDialog.FileName;
                ObservableCollection<ObservableCollection<int>> readData = ExcelUtils.ExcelToRowData(pathName);

                ReadExcelDataToMatrix(readData);
                UpdateMatrix();
                //InputCorrect = MatrixIsCorrect();
            }
            else
            {
                //didn't pick anything
            }
        }

        private void ExportExcelAction()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Please select a folder...",
                Filter = "csv files | *.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = FileUtils.CreateFileName(),
                CheckFileExists = false,
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                filePath = filePath.Replace("\\", "/");

                // Write data to excel file
                ExcelUtils.RowDataToExcel(filePath, ConvertNatrixToIntLists());
            }
        }

        private void AddRowDataAction()
        {
            CreateDefaultRow();
            UpdateMatrix();
            //InputCorrect = MatrixIsCorrect();
        }

        private void DeleteRowDataAction()
        {
            RemoveRowAtIndex(SelectedIndex);
        }

        private void CopyRowDataAction()
        {
            CopyRowAtIndex(SelectedIndex);
        }
    }
}
