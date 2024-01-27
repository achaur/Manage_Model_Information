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

        private List<int> selectedElement;
        public List<int> SelectedElement
        {
            get { return selectedElement; }
            set {
                selectedElement = value;
                OnPropertyChanged(nameof(SelectedElement));
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

        private bool hideCalculations;

        public bool HideCalculations
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
            HideCalculations = true;
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
            Model.SelectedElement = SelectedElement;

            Model.Run();
            CloseAction(window);
        }

        private protected override void CloseAction(Window window)
        {
            if (window != null)
            {
                Closed = true;
                window.Close();
            }
        }

        private string CreateFileName()
        {
            DateTime dateTime = DateTime.Now;
            string timeString = dateTime.ToShortTimeString();
            string datestring = dateTime.ToShortDateString();
            string res = datestring + "_" + timeString + "_";

            return res;
        }

        private void ImportExcelAction(Window window)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Please select a folder...",
                Filter = "xlsx Source Files | *.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? success = openFileDialog.ShowDialog();

            if (success == true)
            {
                string pathName = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;
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
                Filter = "xlsx files (*.xlsx)|*.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = CreateFileName(),
                CheckFileExists = false,
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                filePath = filePath.Replace("\\", "/");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    List<List<string>> rowDataConverted = ManageData.RowDataToStringLists();
                    ExcelUtils.RowDataToExcel(filePath, rowDataConverted);
                    // Write data to excel file
                }
            }
        }

        private void AddRowDataAction()
        {
            ManageData.AddRowData(new RowData()
            {
                _column0 = "Id",
                _column1 = "new",
                _column2 = "new",
                _column3 = "new",
                _column4 = "new",
                _column5 = "new"
            });
            ManageData.UpdateCalculations();
        }

        private void SetCalculationsVisibilityAction()
        {
            if (HideCalculations == true)
            {
                HideCalculations = false;
            }
            else
            { 
                HideCalculations = true;
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