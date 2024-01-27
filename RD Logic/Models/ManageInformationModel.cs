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

namespace ManageInfo_Logic
{
    [Transaction(TransactionMode.Manual)]
    public class ManageInformationModel : BaseModel
    {
        #region PROPERTIES

        private List<int> selectedElement;

        public List<int> SelectedElement
        {
            get { return selectedElement; }
            set { selectedElement = value; }
        }

        private int transparencyNumber;

        public int TransparencyNumber
        {
            get { return transparencyNumber; }
            set
            {
                transparencyNumber = value;
                OnPropertyChanged(nameof(TransparencyNumber));
            }
        }

        private string familyName;

        public string FamilyName
        {
            get { return familyName; }
            set
            {
                familyName = value;
                OnPropertyChanged(nameof(FamilyName));
            }
        }

        private bool halftone;

        public bool Halftone
        {
            get { return halftone; }
            set
            {
                halftone = value;
                OnPropertyChanged(nameof(Halftone));
            }
        }

        private string familyShortName;

        public string FamilyShortName
        {
            get { return familyShortName; }
            set
            {
                familyShortName = value;
            }
        }

        #endregion

        #region METHODS
        private protected override void TryExecute()
        {
            using (Transaction trans = new Transaction(Doc, TransactionName))
            {
                trans.Start();

                

                trans.Commit();
            }
        }

        #endregion
    }
}
