using System.Data;
using System.Windows;

namespace ManageInfo_Core
{
    public class RunResult
    {
        #region PROPERTIES
        /*
        private string _transactionName;
        public string TransactionName
        {
            get { return _transactionName; }
            set { _transactionName = value; }
        }
        */

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string operationPerformed;

        public string OperationPerformed
        {
            get { return operationPerformed; }
            set { operationPerformed = value; }
        }

        private bool _started;
        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        private bool _failed;
        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }

        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }


        private string perfomanceTime;

        public string PerfomanceTime
        {
            get { return perfomanceTime; }
            set { perfomanceTime = value;}
        }

        private DataSet _report;
        public DataSet Report
        {
            get { return _report; }
            set { _report = value; }
        }



        #endregion
    }
}