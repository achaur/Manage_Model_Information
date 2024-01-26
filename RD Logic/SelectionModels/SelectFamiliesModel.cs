using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ManageInfo_Core
{
    public class SelectFamiliesModel : INotifyPropertyChanged
    {
        private UIDocument _uidoc;
        private Document _doc;

        #region PROPERTIES

        private List<int> _selectedElement;
        public List<int> SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged(nameof(SelectedElement));
            }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        #endregion

        public SelectFamiliesModel(BaseModel model)
        {
            _uidoc = model.Uidoc;
            _doc = _uidoc.Document;
        }

        public void Run()
        {
            Error = "";

            try
            {
                // Get the line from user selection
                List<ElementId> eleIds = new List<ElementId>();

                IList<Reference> references = _uidoc.Selection.PickObjects(ObjectType.Element, "Select Family Instances");

                foreach (Reference reference in references)
                {
                    try 
                    {
                        if (null != reference)
                        { 
                            Element ele = _doc.GetElement(reference);

                            if (null != ele)
                            {
                                if (ele is FamilyInstance familyInstance)
                                { 
                                    ElementId eleId = reference.ElementId;

                                    if (eleId != ElementId.InvalidElementId)
                                    { 
                                        eleIds.Add(eleId);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                if (eleIds.Count == 0)
                    Error = "No selection";

                List<int> eleIdInts = eleIds.Count() != 0 ? eleIds.Select(x => x.IntegerValue).ToList() : null;

                SelectedElement = (Error.Length == 0)
                    ? eleIdInts
                    : null;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                Error = "Selection cancelled";
                SelectedElement = null;
            }
            catch (Exception e)
            {
                Error = e.Message;
                SelectedElement = null;
            }
        }

        #region INOTIFYPROPERTYCHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}