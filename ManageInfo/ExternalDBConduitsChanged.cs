using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.Text.RegularExpressions;

namespace ManageInfo
{
    class ExternalDBConduitsChanged : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(CoonduitChanged);
            return ExternalDBApplicationResult.Succeeded;
        }
        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
                //add event that tracks when parameters of element of type conduit was changed
                application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(CoonduitChanged);
            }
            catch (Exception)
            {
                return ExternalDBApplicationResult.Failed;
            }
            return ExternalDBApplicationResult.Succeeded;
        }

        public void CoonduitChanged(object sender, DocumentChangedEventArgs args)
        {
            ElementFilter conduitsFilter = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
            ElementId changedElementId = args.GetModifiedElementIds(conduitsFilter).First();
            string name = args.GetTransactionNames().First();

            //check type of transaction
            if (args.Operation != UndoOperation.TransactionUndone &&
                args.Operation != UndoOperation.TransactionRolledBack &&
                args.Operation != UndoOperation.TransactionGroupRolledBack)
            {
                if (name == "Modify element attributes")
                {
                    //retrieve value of key shared parameter of the conduit

                    //retrieve all elements of electricity network modified conduit belongs to

                    //set values to other shared parameters of conduit according to key ID
                    TaskDialog dialog = new TaskDialog("Conduit Changed")
                    {
                        MainIcon = TaskDialogIcon.TaskDialogIconWarning,
                        MainInstruction = "Conduit has Changed!",
                        MainContent = "Conduit has Changed!"
                    };
                    dialog.Show();
                }
            }
            //
        }
    }
}