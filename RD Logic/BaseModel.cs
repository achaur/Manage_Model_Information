using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ManageInfo_Core
{
    [Transaction(TransactionMode.Manual)]
    public abstract class BaseModel : INotifyPropertyChanged, IExternalEventHandler
    {
        #region PROPERTIES

        /// <summary>
        /// ExternalEvent needed for Revit to run transaction in API context.
        /// So we must call not the main method but raise the event.
        /// </summary>
        public ExternalEvent ExternalEvent { get; set; }

        public UIDocument Uidoc { get; set; }
        public Document Doc { get; set; }

        public Action<RunResult> ShowResult { get; set; }

        public RunResult Result { get; set; }

        public DateTime TimeStarted { get; set; }
        public DateTime TimeCompleted { get; set; }

        private string _transactionName;
        public string TransactionName
        {
            get { return _transactionName; }
            set
            {
                _transactionName = value;
                OnPropertyChanged(nameof(TransactionName));
            }
        }

        private bool _runFailed;
        public bool RunFailed
        {
            get { return _runFailed; }
            set
            {
                _runFailed = value;
                OnPropertyChanged(nameof(RunFailed));
            }
        }

        private string _runResult;
        public string RunResult
        {
            get { return _runResult; }
            set
            {
                _runResult = value;
                OnPropertyChanged(nameof(RunResult));
            }
        }

        #endregion

        public void Run()
        {
            ExternalEvent.Raise();
        }

        public void SetCommandData(ExternalCommandData commandData)
        {
            Uidoc = commandData.Application.ActiveUIDocument;
            Doc = Uidoc.Document;
        }

        #region IEXTERNALEVENTHANDLER

        public string GetName()
        {
            return TransactionName;
        }

        public virtual void Execute(UIApplication app)
        {
            Result.Started = true;

            try
            {
                //add time when execution started

                TryExecute();

                //add time when execution ended
            }
            catch (Exception e)
            {
                //add time when execution failed

                Result.Failed = true;
                Result.Result = ExceptionUtils.GetMessage(e);
            }
            finally
            {
                ShowResult(Result);
            }
        }

        private protected abstract void TryExecute();

        #endregion

        #region METHODS


        private protected virtual string GetRunResult() => "";
        private protected virtual DataSet GetRunReport(IEnumerable<ReportMessage> reportMessages) => null;

        #endregion

        #region INOTIFYPROPERTYCHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}