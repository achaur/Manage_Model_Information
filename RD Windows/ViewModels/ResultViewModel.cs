using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

namespace ManageInfo_Windows
{
    /// <summary>
    /// View model "Result" windows
    /// </summary>
    public class ResultViewModel : BaseViewModel
    {
        #region PROPERTIES

        private string perfomanceTime;

        public string PerfomanceTime
        {
            get { return perfomanceTime; }
            set { 
                perfomanceTime = value; 
                OnPropertyChanged(nameof(PerfomanceTime));
            }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set {
                fileName = value; 
                OnPropertyChanged(nameof(FileName));
            }
        }

        private string operationPerformed;

        public string OperationPerformed
        {
            get { return operationPerformed; }
            set {
                operationPerformed = value; 
                OnPropertyChanged(nameof(OperationPerformed));
            }
        }


        private string _commandName;
        public string CommandName
        {
            get { return _commandName; }
            set
            {
                _commandName = value;
                OnPropertyChanged(nameof(CommandName));
            }
        }

        private bool ExportPerfomed = false;

        private string _reportText;
        public string ReportText
        {
            get { return _reportText; }
            set
            {
                _reportText = value;
                OnPropertyChanged(nameof(ReportText));
            }
        }

        private bool allowExport;

        public bool AllowExport
        {
            get {
                if (ExportPerfomed == true)
                    allowExport = false;
                else
                    allowExport = true;
                return allowExport; 
            }
            set { 
                allowExport = value;
                OnPropertyChanged(nameof(AllowExport));
            }
        }

        private Visibility showExportBtn;

        public Visibility ShowExportBtn
        {
            get {

                if (CommandName == "Join/Unjoin elements")
                    showExportBtn = Visibility.Visible;
                else
                    showExportBtn = Visibility.Hidden;
                return showExportBtn;
            }
            set {
                showExportBtn = value;
                OnPropertyChanged(nameof(ShowExportBtn));
            }
        }
        #endregion
        
        public ResultViewModel(string commandName, string reportText, string performanceTime, string fileName, string operationPerformed)
        {
            CommandName = commandName;
            ReportText = reportText;
            PerfomanceTime = performanceTime;
            OperationPerformed= operationPerformed;
            FileName = fileName;

            CloseCommand = new CommandWindow(CloseAction);
        }

        #region METHODS

        public override void SetInitialData() { }

        #endregion

        #region COMMANDS

        private protected override void RunAction(Window window) { }

        private protected override void CloseAction(Window window)
        {
            if (window != null)
            {
                Closed = true;
                window.Close();
            }
        }

        private CommandWindow exportReport;

        public ICommand ExportReport
        {
            get
            {
                if (exportReport == null)
                    exportReport = new CommandWindow(PerformExportReport);

                return exportReport;
            }
        }

        private string CheckFileNameCorrectness(string s)
        {
            string[] illegalSymbols = { "#", "%", "&", "{", "}", "\\", "<", ">", "*", "?", "/", "$", "\'", "\"", ":", "@", "+", "`", "|", "=", "."};
            string res = s;

            foreach (string illegalSymbol in illegalSymbols)
            {
                if (res.Contains(illegalSymbol))
                {
                    res = res.Replace(illegalSymbol, "_");
                }
            }

            return res;
        }

        private string CreateFileName()
        {
            DateTime dateTime= DateTime.Now;

            string timeString = dateTime.ToShortTimeString();
            string datestring = dateTime.ToShortDateString();


            string res = CheckFileNameCorrectness(datestring + "_" + timeString + "_" + fileName + "_" + operationPerformed);

            return res;
        }

        private void PerformExportReport(object commandParameter)
        {
            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = CreateFileName(),
                CheckFileExists = false,
                Title = "Please select a folder...",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the text to the file
                    writer.Write(PerfomanceTime + '\n' + ReportText);
                }

                ExportPerfomed = true;
                AllowExport = false;
            }
        }
        #endregion
    }
}
