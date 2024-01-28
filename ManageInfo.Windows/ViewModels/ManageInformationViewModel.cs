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

        private ObservableCollection<RowData> items;
        public ObservableCollection<RowData> Items
        {
            get { return items; }
            set {
                items = value;
                ManageData.UpdateCalculations();
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

        private Visibility hideCalculations;

        public Visibility HideCalculations
        {
            get { return hideCalculations; }
            set {
                hideCalculations = value;
                OnPropertyChanged(nameof(HideCalculations));
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
            SetCalculationsVisibility = new CommandGeneric(SetCalculationsVisibilityAction);

            RemoveRowCommand = new CommandGeneric(DeleteRowDataAction);
            CopyRowCommand = new CommandGeneric(CopyRowDataAction);
        }

        #region METHODS
        public override void SetInitialData()
        {
            HideCalculations = Visibility.Hidden;
            Items = ManageData.GetRowsData();
            ManageData.UpdateCalculations();
            Model = (ManageInformationModel)BaseModel;
        }

        #endregion

        #region VALIDATION

        private bool CheckInput()
        {

            return false;
        }

        #endregion

        #region COMMANDS
        public ICommand SetCalculationsVisibility { get; set; }
        public ICommand ImportExcelCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand AddRowCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }
        public ICommand CopyRowCommand { get; set; }

        private protected override void RunAction(Window window)
        {
            Model.RowData = ManageData.RowDataToStringLists();

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
                List<List<string>> readData =  ExcelUtils.ExcelToRowData(pathName);
                List<RowData> rowsToAdd = ManageData.StringListsToRowData(readData);
                foreach (RowData row in rowsToAdd)
                { 
                    ManageData.AddRowData(row);
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
                List<List<string>> rowDataConverted = ManageData.RowDataToStringLists();
                ExcelUtils.RowDataToExcel(filePath, rowDataConverted);
            }
        }

        private void AddRowDataAction()
        {
            ManageData.AddRowData(new RowData());
            ManageData.UpdateCalculations();
        }

        private void SetCalculationsVisibilityAction()
        {
            if (HideCalculations == Visibility.Visible)
            {
                HideCalculations = Visibility.Hidden;
            }
            else
            { 
                HideCalculations = Visibility.Visible;
            }
        }

        private void DeleteRowDataAction()
        {
            ManageData.DeleteRowData(SelectedIndex);
        }

        private void CopyRowDataAction()
        {
            RowData rowData = ManageData.GetRowDataAtIndex(SelectedIndex);
            if (null != rowData)
            { 
                ManageData.AddRowData(rowData);
            }
        }

        #endregion
    }
}