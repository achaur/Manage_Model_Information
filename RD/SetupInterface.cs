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
            string tabName = "ManageInfo Studio";
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
                Label = "Find Base Point",
                Panel = panel,
                ToolTip = "Creates a visual indication of the Family Base Point for selected Families.",
                CommandNamespacePath = FamilyBasePoint.GetPath(),
                IconImageName = "family_base_points.png",
                AvailabilityClassName = "ManageInfo_Core.DocumentIsNotFamily"
            };

            RevitPushButton.Create(buttonData01);

            return panel;
        }
    }
}
