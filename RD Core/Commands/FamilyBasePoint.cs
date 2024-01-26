using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using ManageInfo_Core;
using ManageInfo_Logic;
using ManageInfo_Windows;

namespace ManageInfo_Core
{
    [Transaction(TransactionMode.Manual)]
    public class FamilyBasePoint : BaseCommand
    {
        public FamilyBasePoint()
        {
            _transactionName = "Family Base Point";

            _model = new FamilyBasePointModel();
            _viewModel = new FamilyBasePointViewModel();
            _view = new FamilyBasePointForm();
        }

        public static string GetPath() => typeof(FamilyBasePoint).Namespace + "." + nameof(FamilyBasePoint);
    }
}
