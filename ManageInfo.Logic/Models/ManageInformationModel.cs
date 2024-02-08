using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;
using ManageInfo_Core;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB.ExtensibleStorage;
using View = Autodesk.Revit.DB.View;

namespace ManageInfo_Logic
{
    [Transaction(TransactionMode.Manual)]
    public class ManageInformationModel : BaseModel
    {
        #region PROPERTIES

        public List<List<string>> RowData { get; set; }

        #endregion

        #region METHODS
        private protected override void TryExecute()
        {
            if (null == RowData || RowData?.Count == 0)
                return;

            //associate retrieved information with project base point
            using (Transaction trans = new Transaction(Doc, "Create Sheets"))
            {
                trans.Start();

                FilteredElementCollector coll = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Sheets);
                IList<Element> sheets = coll.ToElements();

                ElementCategoryFilter categoryFilter_block = new ElementCategoryFilter(BuiltInCategory.OST_TitleBlocks);
                FilteredElementCollector collector = new FilteredElementCollector(Doc);

                Type type = collector.WherePasses(categoryFilter_block).GetType();
                ElementId first_block = collector.FirstElementId();

                foreach (List<string> sheetData in RowData)
                {
                    string number = sheetData[0];
                    string name = sheetData[1];

                    if (SheetAlreadyExists(sheets, number) != true)
                    { 
                        ViewSheet sht = ViewSheet.Create(Doc, first_block);
                        sht.Name = name;
                        sht.SheetNumber = number;
                    }
                }
                trans.Commit();
            }
        }



        public bool SheetAlreadyExists(IList<Element> sheets, string num_d)
        {
            foreach (Element e in sheets)
            {
                ViewSheet sh = e as ViewSheet;

                if (sh.SheetNumber == num_d)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
