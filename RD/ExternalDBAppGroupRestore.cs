using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace ManageInfo
{
    class ExternalDBAppGroupRestore : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            //add event that tracks when parameters of element of type conduit was changed
            return ExternalDBApplicationResult.Succeeded;
        }
        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
            }
            catch (Exception)
            {
                return ExternalDBApplicationResult.Failed;
            }
            return ExternalDBApplicationResult.Succeeded;
        }
    }
}