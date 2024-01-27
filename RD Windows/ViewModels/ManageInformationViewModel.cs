using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ManageInfo_Logic;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using ManageInfo_Windows.ViewModels;
using System.Collections.ObjectModel;

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
            ManageData md = new ManageData();
            Items = ManageData.GetRowsData();
            //FamilyName = "";
            //FamilyShortName = "";
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
                Filter = "xlsx files (*.txt)|*.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = CreateFileName(),
                CheckFileExists = false,
                AddExtension = true
            };

            bool? success = saveFileDialog.ShowDialog();

            if (success == true)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write data to excel file
                }
            }
        }

        private void AddRowDataAction()
        {
            AddRowDataForm addRowDataWindow = new AddRowDataForm();

            /*
            ManageData.AddRowData(new RowData() 
            { 
                _column1 = "new",
                _column2 = "new",
                _column3 = "new",
                _column4 = "new",
                _column5 = "new"
            });
            */
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