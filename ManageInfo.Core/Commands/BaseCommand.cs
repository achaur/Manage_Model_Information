﻿using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ManageInfo_Core;
using ManageInfo_Windows;

namespace ManageInfo_Core
{
    [Transaction(TransactionMode.Manual)]
    public abstract class BaseCommand : IExternalCommand
    {
        private protected string _transactionName;

        private protected BaseModel _model;
        private protected BaseViewModel _viewModel;
        private protected BaseView _view;

        private protected RunResult _result;

        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _result = new RunResult() { Started = true };

            Run(commandData);

            if (!_result.Started)
                return Result.Cancelled;
            if (_result.Failed)
                return Result.Failed;
            else
                return Result.Succeeded;
        }

        private protected virtual void Run(ExternalCommandData commandData)
        {
            // Model
            _model.SetCommandData(commandData);
            _model.TransactionName = _transactionName;
            _model.Result = _result;
            _model.ShowResult = new Action<RunResult>(ShowResult);
            ExternalEvent externalEvent = ExternalEvent.Create(_model);
            _model.ExternalEvent = externalEvent;

            // ViewModel
            _viewModel.BaseModel = _model;
            _viewModel.SetInitialData();

            // View
            _view.DataContext = _viewModel;
            _view.ShowDialog();
        }

        private protected void ShowResult(RunResult runResult)
        {
            _result = runResult;

            if (!_result.Started)
                return;
            if (!string.IsNullOrEmpty(_result.Result))
            {
                ShowResultDialog();
                return;
            }
            if (_result.Report != null)
                ShowResultReport();
        }

        private void ShowResultDialog()
        {
            // ViewModel
            ResultViewModel formVM = new ResultViewModel(_transactionName, _result.Result, _result.PerfomanceTime, _result.FileName, _result.OperationPerformed);

            // View
            ResultForm form = new ResultForm() { DataContext = formVM };
            form.ShowDialog();
        }

        private void ShowResultReport()
        {
            // ViewModel
            ReportViewModel formReportVM = new ReportViewModel(_result.Report);

            // View
            ReportForm formReport = new ReportForm() { DataContext = formReportVM };
            formReport.ShowDialog();
        }
    }
}