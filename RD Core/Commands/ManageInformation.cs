using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using ManageInfo_Core;
using ManageInfo_Logic;
using ManageInfo_Windows;

namespace ManageInfo_Core
{
    [Transaction(TransactionMode.Manual)]
    public class ManageInformation : BaseCommand
    {
        public ManageInformation()
        {
            _transactionName = "Family Base Point";

            _model = new ManageInformationModel();
            _viewModel = new ManageInformationViewModel();
            _view = new ManageInformationForm();
        }

        public static string GetPath() => typeof(ManageInformation).Namespace + "." + nameof(ManageInformation);
    }
}
