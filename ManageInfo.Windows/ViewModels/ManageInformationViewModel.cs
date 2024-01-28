using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ManageInfo_Logic;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ManageInfo_Windows.ViewModels;
using System.Collections.ObjectModel;
using ManageInfo_Utils;
using System.Windows.Forms;
using System.Linq;

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
            get { return items; }
            set {
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
                OnPropertyChanged(nameof(NumberOfRows));
            }
        }

        #endregion

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

        #region METHODS
        public override void SetInitialData()
        {
            NumberOfColumns = 11;
            NumberOfRows = 1;
            inputCorrect = true;
            InitializeMatrix();

            Model = (ManageInformationModel)BaseModel;
        }
        public void InitializeMatrix()
        {
            Items = new ObservableCollection<ObservableCollection<string>>();

            for (int i = 0; i < NumberOfRows; i++)
            {
                var row = new ObservableCollection<string>();
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    row.Add("0"); // Initialize with some default values
                }
                Items.Add(row);
            }
        }


        #endregion

        #region VALIDATION

        private bool CheckInput()
        {

            return false;
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
                Filter = "csv Source Files | *.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? success = openFileDialog.ShowDialog();

            if (success == true)
            {
                string pathName = openFileDialog.FileName;
                ObservableCollection<ObservableCollection<string>> readData =  ExcelUtils.ExcelToRowData(pathName);
                foreach (ObservableCollection<string> row in readData)
                { 
                    Items.Add(row);
                    NumberOfRows++;
                }
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
            ObservableCollection<string> blankRow = 
                new ObservableCollection<string>();
            Items.Add(blankRow);
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
                }
            }
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
                }
            }
        }

        #endregion
    }
}