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
    public class AddRowDataViewModel : BaseViewModel
    {
        #region PROPERTIES

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

        public AddRowDataViewModel()
        {
            RunCommand = new CommandWindow(RunAction);
            CloseCommand = new CommandWindow(CloseAction);
            //ImportExcelCommand = new CommandWindow(ImportExcelAction);
            //ExportExcelCommand = new CommandGeneric(ExportExcelAction);
            //AddRowCommand = new CommandGeneric(AddRowDataAction);
            //RemoveRowCommand = new CommandGeneric(DeleteRowDataAction);
            //CopyRowCommand = new CommandGeneric(CopyRowDataAction);
        }

        #region METHODS
        public override void SetInitialData()
        {
            ManageData md = new ManageData();
        }

        #endregion

        #region VALIDATION

        private bool CheckInput()
        {
           
            return false;
        }

        #endregion

        #region COMMANDS

        private protected override void RunAction(Window window)
        {
            //Model.SelectedElement = SelectedElement;

            //Model.Run();
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

        #endregion
    }
}