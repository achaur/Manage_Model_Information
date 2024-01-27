using Autodesk.Revit.UI;
using RD_UI;
using ManageInfo_Core;
using System.Collections.Generic;

namespace ManageInfo
{
    public class SetupInterface
    {
        public SetupInterface() { }

        private List<RibbonPanel> panelsOfPlugin;

        public List<RibbonPanel> PanelsOfPlugin
        {
            get { return panelsOfPlugin; }
            set { panelsOfPlugin = value; }
        }

        public void Initialize(UIControlledApplication application)
        {
            // Create Ribbon Tab
            string tabName = "Manage Info";
            application.CreateRibbonTab(tabName);

            PanelsOfPlugin = new List<RibbonPanel>();

            // Create Ribbon Panels
            RibbonPanel ribbonPanel01 = CreatePanel01(application, tabName, "Family");

            PanelsOfPlugin.Add(ribbonPanel01);
        }
        private RibbonPanel CreatePanel01(UIControlledApplication application, string tabName, string panelName)
        {
            RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);

            RevitPushButtonDataModel buttonData01 = new RevitPushButtonDataModel
            {
                Label = "Manage Information",
                Panel = panel,
                ToolTip = "Fill and Update Model Information.",
                CommandNamespacePath = ManageInformation.GetPath(),
                IconImageName = "family_base_points.png",
                AvailabilityClassName = "ManageInfo_Core.DocumentIsNotFamily"
            };

            RevitPushButton.Create(buttonData01);

            return panel;
        }
    }
}
