using Autodesk.Revit.UI;

//testComment

namespace ManageInfo
{
    /// <summary>
    /// Startup class for all Revit plugin (described in .addin file).
    /// OnStartup() after initializing on startup returns Result.Succeeded.
    /// </summary>
    public class Main : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Initialize whole plugin's user interface
            SetupInterface ui = new SetupInterface();

            ui.Initialize(application);

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
