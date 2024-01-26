using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using Microsoft.Win32;
using ManageInfo_Core;
using ManageInfo_Logic;

namespace ManageInfo_Windows
{
    /// <summary>
    /// View model for command "FamilyParameterSet"
    /// </summary>
    public class FamilyBasePointViewModel : BaseViewModel
    {
        #region PROPERTIES

        public ICommand ChooseFamilyCommand { get; set; }
        public ICommand SelectFamiliesCommand { get; set; }

        private FamilyBasePointModel _model;
        public FamilyBasePointModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string transparency;

        public string Transparency
        {
            get { return transparency; }
            set 
            { 
                transparency = value;                                                                                                                                                                                           
                OnPropertyChanged(nameof(Transparency));
            }
        }

        private string familyName;

        public string FamilyName
        {
            get { return familyName; }
            set { 
                familyName = value;
                InputCorrect = CheckInput();
                OnPropertyChanged(nameof(FamilyName));
            }
        }

        private string familyShortName;

        public string FamilyShortName
        {
            get { return familyShortName; }
            set
            {
                familyShortName = value;
            }
        }

        private int transparencyNumber;

        public int TransparencyNumber
        {
            get { return transparencyNumber; }
            set { 
                transparencyNumber = value;
                Transparency = transparencyNumber.ToString();
                OnPropertyChanged(nameof(TransparencyNumber));
            }
        }

        private string selectedFamiliesCount;

        public string SelectedFamiliesCount
        {
            get { return selectedFamiliesCount; }
            set { 
                selectedFamiliesCount = value;
                InputCorrect = CheckInput();
                OnPropertyChanged(nameof(SelectedFamiliesCount));
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private SelectFamiliesModel selectFamiliesModel;

        public SelectFamiliesModel SelectFamiliesModel
        {
            get { return selectFamiliesModel; }
            set { selectFamiliesModel = value; }
        }

        private List<int> selectedElement;

        public List<int> SelectedElement
        {
            get { return selectedElement; }
            set { selectedElement = value; }
        }

        private string _selectedElementError;
        public string SelectedElementError
        {
            get { return _selectedElementError; }
            set
            {
                _selectedElementError = value;
                OnPropertyChanged(nameof(SelectedElementError));
            }
        }

        private bool halftone;

        public bool Halftone
        {
            get { return halftone; }
            set { 
                halftone = value; 
                OnPropertyChanged(nameof(Halftone));
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

        public FamilyBasePointViewModel()
        {
            RunCommand = new CommandWindow(RunAction);
            CloseCommand = new CommandWindow(CloseAction);
            ChooseFamilyCommand = new CommandWindow(ChooseFamilyAction);
            SelectFamiliesCommand = new CommandGeneric(SelectFamiliesAction);
        }

        #region METHODS

        public string GetFamilyShortName()
        {
            if (null == FamilyName)
                return "";
            if (FamilyName?.Length == 0)
                return "";

            try
            {
                string fileName = Path.GetFileName(FamilyName);
                return fileName;
            }
            catch
            {
                return "";
            } 
        }

        public override void SetInitialData()
        {
            IsVisible = true;
            TransparencyNumber = 0;
            Transparency = "0";
            FamilyName = Properties.Settings.Default.LastUserSelectedPath;
            FamilyShortName = GetFamilyShortName();
            //FamilyName = "";
            //FamilyShortName = "";
            SelectedElement = new List<int>();
            SelectedFamiliesCount = "0";
            SelectedElementError = "No Selection";
            Model = (FamilyBasePointModel)BaseModel;
        }

        #endregion

        #region VALIDATION

        private bool CheckInput()
        {
            if (null != SelectedFamiliesCount)
            {
                if (SelectedFamiliesCount.Length != 0)
                {
                    if (SelectedFamiliesCount != "0")
                    {
                        if (FamilyName?.Length > 1)
                        {
                            SelectedElementError = "";
                            return true;
                        }
                    }
                }
            }
            SelectedElementError = "No Selection";

            return false;
        }
         
        #endregion

        #region COMMANDS

        private protected override void RunAction(Window window)
        {
            Model.TransparencyNumber = TransparencyNumber;
            Model.FamilyName = FamilyName;
            Model.Halftone = Halftone;
            Model.SelectedElement = SelectedElement;
            Model.FamilyShortName = FamilyShortName;

             Properties.Settings.Default.LastUserSelectedPath = FamilyName;
            Properties.Settings.Default.Save();
            //FamilyShortName = GetFamilyShortName();

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

        private void ChooseFamilyAction(Window window)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "rfa Source Files | *.rfa";
            bool? success = openFileDialog.ShowDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (success == true)
            {
                string pathName = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;
                FamilyName = pathName;
                FamilyShortName = fileName;
            }
            else
            { 
                //didn't pick anything
            }
        }

        private void SelectFamiliesAction()
        {
            IsVisible = false;
            SelectFamiliesModel = new SelectFamiliesModel(BaseModel);
            SelectFamiliesModel.Run();

            SelectedElement = SelectFamiliesModel.SelectedElement;
            SelectedFamiliesCount = (SelectedElement == null || SelectedElement?.Count == 0)
                ? "0"
                : SelectedElement.Count.ToString();
            SelectedElementError = SelectFamiliesModel.Error;

            CheckInput();

            IsVisible = true;
        }

        #endregion
    }
}