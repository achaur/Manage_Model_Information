using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ManageInfo_Logic;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Collections.ObjectModel;
using ManageInfo_Utils;
using System.Windows.Forms;
using System.Linq;
using Autodesk.Revit.DB.Mechanical;
using System.Globalization;
using Autodesk.Revit.DB;

namespace ManageInfo_Windows
{
    public class ManageInformationViewModel : BaseViewModel
    {
        #region PROPERTIES

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

        private ManageInformationModel _model;
        public ManageInformationModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private ObservableCollection<ObservableCollection<string>> items;
        public ObservableCollection<ObservableCollection<string>> Items
        {
            get 
            { 

                return items; 
            }
            set 
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        private bool inputCorrect;
        public bool InputCorrect
        {
            get { return inputCorrect; }
            set
            {
                inputCorrect = value;
                if (value == false)
                {
                    ErrorMessage = "One of the values is not within a range (0;100)";
                }
                else
                { 
                    ErrorMessage = string.Empty;
                }
                OnPropertyChanged(nameof(InputCorrect));
            }
        }

        private int numberOfColumns;
        public int NumberOfColumns
        {
            get { return numberOfColumns; }
            set
            {
                numberOfColumns = value;
                OnPropertyChanged(nameof(NumberOfColumns));
            }
        }

        private int numberOfRows;
        public int NumberOfRows
        {
            get { return numberOfRows; }
            set
            {
                numberOfRows = value;
                if (numberOfRows == 0)
                    ErrorMessage = "Table is empty";
                else
                    ErrorMessage = string.Empty;
                OnPropertyChanged(nameof(NumberOfRows));
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        #endregion

        #region METHODS
        public ManageInformationViewModel()
        {
            RunCommand = new CommandWindow(RunAction);
            CloseCommand = new CommandWindow(CloseAction);
            ImportExcelCommand = new CommandWindow(ImportExcelAction);
            ExportExcelCommand = new CommandGeneric(ExportExcelAction);
            AddRowCommand = new CommandGeneric(AddRowDataAction);
            RemoveRowCommand = new CommandGeneric(DeleteRowDataAction);
            CopyRowCommand = new CommandGeneric(CopyRowDataAction);
        }

        public override void SetInitialData()
        {
            NumberOfColumns = 2;
            NumberOfRows = 0;
            inputCorrect = true;
            InitializeMatrix();
            ErrorMessage = "";
            Model = (ManageInformationModel)BaseModel;
        }
        #endregion

        #region COMMANDS
        public ICommand ImportExcelCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand AddRowCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }
        public ICommand CopyRowCommand { get; set; }

        private protected override void RunAction(Window window)
        {
            Model.RowData = Items.Select(x => x.ToList()).ToList();
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
                Filter = "xlsx Source Files | *.xlsx",
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? success = openFileDialog.ShowDialog();

            if (success == true)
            {
                string pathName = openFileDialog.FileName;
                //ObservableCollection<ObservableCollection<string>> readData = ExcelUtils.ExcelToRowData(pathName);
                ObservableCollection<ObservableCollection<string>> readData = ExcelUtils.ExcelToList(pathName);
                foreach (ObservableCollection<string> row in readData)
                {
                    //initialize empty row
                    ObservableCollection<string> rowWithCalculatedValues = new ObservableCollection<string>();
                    //fill row with imported values
                    for (int i = 0; i < row.Count; i++)
                    {
                        rowWithCalculatedValues.Add(row[i]);
                    }
                    Items.Add(rowWithCalculatedValues);
                    NumberOfRows++;
                }
                InputCorrect = MatrixIsCorrect();
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
                ExcelUtils.RowDataToExcel(filePath, Items);
            }
        }

        private void AddRowDataAction()
        {
            CreateDefaultRow();
            InputCorrect = MatrixIsCorrect();
            NumberOfRows++;
        }

        private void DeleteRowDataAction()
        {
            if (SelectedIndex >= 0 && SelectedIndex < NumberOfRows)
            { 
                if (null != Items && Items?.Count != 0)
                { 
                    Items.RemoveAt(SelectedIndex);
                    NumberOfRows--;

                    if (numberOfRows > 0)
                        SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region MATRIX MANAGER
        public bool MatrixIsCorrect()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                ObservableCollection<string> row = Items[i];
                for (int j = 1; j < NumberOfColumns && j < row?.Count; j++)
                {
                    string cell = row[j];
                    if (!(null != cell && cell != string.Empty))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void InitializeMatrix()
        {
            Items = new ObservableCollection<ObservableCollection<string>>();
            //CreateDefaultRow();
        }
        private void CopyRowDataAction()
        {
            if (null != Items && Items?.Count != 0) 
            {
                if (SelectedIndex >= 0 && SelectedIndex < NumberOfRows)
                { 
                    ObservableCollection<string> rowData = Items[SelectedIndex];
                    Items.Add(rowData);
                    NumberOfRows++;
                    InputCorrect = MatrixIsCorrect();
                }
            }
        }
        public void CreateDefaultRow()
        {
            ObservableCollection<string> defaultRow = new ObservableCollection<string>();

            defaultRow.Add("Number");
            defaultRow.Add("Name");

            Items.Add(defaultRow);
        }

        #endregion
    }
}