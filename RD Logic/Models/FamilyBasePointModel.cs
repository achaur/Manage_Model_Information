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
    public class FamilyBasePointModel : BaseModel
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
            string familyName = FamilyShortName.Split('.')[0];
            bool familyIsAlreadyUploaded = null != FindFamilyByName(familyName, Doc);
            if (!File.Exists(FamilyName))
            {
                if (!familyIsAlreadyUploaded)
                { 
                    Result.Result = "Family file does not exist.";
                    return;
                }
            }

            using (Transaction trans = new Transaction(Doc, TransactionName))
            {
                trans.Start();

                List<ElementId> eleIds = new List<ElementId>();

                foreach (int selectedElementItem in SelectedElement)
                {
                    ElementId eleId = new ElementId(selectedElementItem);

                    if (eleId != ElementId.InvalidElementId)
                    {
                        eleIds.Add(eleId);
                    }
                }

                if (eleIds.Count != 0)
                {
                    //override graphic settings of elements 
                    if (Halftone == true || TransparencyNumber > 0)
                    {
                        foreach (ElementId eleId in eleIds)
                        {
                            try
                            { 
                                OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();
                                if (Halftone == true)
                                { 
                                    // Set halftone to true (visible in halftone)
                                    overrideSettings.SetHalftone(true);
                                }
                                if (TransparencyNumber != 0)
                                { 
                                    // Set transparency value (0 = fully opaque, 100 = fully transparent)
                                    overrideSettings.SetSurfaceTransparency(50); // Set to 50% transparency                            
                                }

                                // Apply the override to the selected element
                                Doc.ActiveView.SetElementOverrides(eleId, overrideSettings);
                            }
                            catch(Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    //load family to the project
                    Family family = null;
                    List<XYZ> points = new List<XYZ>();

                    if (Doc.LoadFamily(FamilyName, out family))
                    {
                        //String name = family.Name;
                        FamilySymbol familySymbolToInsert = FindFamilyByName(familyName, Doc);

                        if (null != familySymbolToInsert)
                        {
                            if (familySymbolToInsert.IsActive != true)
                            familySymbolToInsert.Activate();

                            //get location point for each family
                            foreach (ElementId eleId in eleIds)
                            { 
                                FamilyInstance familyInstance = Doc.GetElement(eleId) as FamilyInstance;
                                if (null != familyInstance)
                                {
                                    ElementId levelId = familyInstance.LevelId;
                                    Level level = Doc.GetElement(levelId) as Level;
                                    Location loc = familyInstance.Location;

                                    if (loc is LocationPoint locPoint)
                                    { 
                                        if (null != locPoint)
                                        {
                                            XYZ point = locPoint.Point;

                                            if (FamilyIsModelInPlace(familyInstance) == true)
                                            {
                                                point = GetModelInPlaceCentroid(familyInstance);
                                            }

                                            if (null != point)
                                            {
                                                FamilyInstance newFamilyInstance = 
                                                    Doc.Create.NewFamilyInstance(point, familySymbolToInsert, 
                                                    level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                                                Parameter newfamilyInstanceElevationFromLevelParameter = newFamilyInstance.LookupParameter("Elevation from Level");
                                                Parameter familyInstanceElevationFromLevelParameter = familyInstance.LookupParameter("Elevation from Level");

                                                if (null != newfamilyInstanceElevationFromLevelParameter &&
                                                    null != familyInstanceElevationFromLevelParameter)
                                                {
                                                    double offsetFromLevel = familyInstanceElevationFromLevelParameter.AsDouble();
                                                    newfamilyInstanceElevationFromLevelParameter.Set(offsetFromLevel);
                                                }

                                                Parameter familyInstanceSillHeightParameter = familyInstance.LookupParameter("Sill Height");
                                                if (null != familyInstanceSillHeightParameter &&
                                                    null != newfamilyInstanceElevationFromLevelParameter)
                                                {
                                                    double sillHeight = familyInstanceSillHeightParameter.AsDouble();
                                                    newfamilyInstanceElevationFromLevelParameter.Set(sillHeight);
                                                }

                                                Parameter familyInstanceOffsetFromHostParameter = familyInstance.LookupParameter("Offset from Host");
                                                if (null == familyInstanceElevationFromLevelParameter &&
                                                    null != familyInstanceOffsetFromHostParameter)
                                                {
                                                    double offsetFromHost = familyInstanceOffsetFromHostParameter.AsDouble();
                                                    Parameter newfamilyInstanceOffsetFromHostParameter = newFamilyInstance.LookupParameter("Offset from Host");
                                                    newfamilyInstanceOffsetFromHostParameter.Set(offsetFromHost);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Family is already uploaded
                    else
                    {
                        //Find family by family name
                        FamilySymbol familySymbolToInsert = FindFamilyByName(familyName, Doc);

                        if (null != familySymbolToInsert)
                        {
                            if (familySymbolToInsert.IsActive != true)
                                familySymbolToInsert.Activate();
                            //get location point for each family
                            foreach (ElementId eleId in eleIds)
                            {
                                FamilyInstance familyInstance = Doc.GetElement(eleId) as FamilyInstance;
                                //Check if family instance is model in place
                                ElementId familyInstanceTypeId = familyInstance.GetTypeId();
                                FamilySymbol famSymbol = Doc.GetElement(familyInstanceTypeId) as FamilySymbol;


                                if (null != familyInstance)
                                {
                                    bool elementIsInlace = familyInstance.Symbol.Family.IsInPlace;

                                    ElementId levelId = familyInstance.LevelId;
                                    Level level = Doc.GetElement(levelId) as Level;
                                    Location loc = familyInstance.Location;

                                    if (loc is LocationPoint locPoint)
                                    { 
                                        if (null != locPoint)
                                        {
                                            XYZ point = locPoint.Point;

                                            if (FamilyIsModelInPlace(familyInstance) == true)
                                            {
                                                point = GetModelInPlaceCentroid(familyInstance);
                                            }

                                            if (null != point)
                                            {
                                                FamilyInstance newFamilyInstance = 
                                                    Doc.Create.NewFamilyInstance(point, familySymbolToInsert, level, 
                                                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                                Parameter newfamilyInstanceElevationFromLevelParameter = 
                                                    newFamilyInstance.LookupParameter("Elevation from Level");
                                                Parameter familyInstanceElevationFromLevelParameter = 
                                                    familyInstance.LookupParameter("Elevation from Level");

                                                if (null != newfamilyInstanceElevationFromLevelParameter &&
                                                    null != familyInstanceElevationFromLevelParameter)
                                                {
                                                    double offsetFromLevel = familyInstanceElevationFromLevelParameter.AsDouble();
                                                    newfamilyInstanceElevationFromLevelParameter.Set(offsetFromLevel);
                                                }

                                                Parameter familyInstanceSillHeightParameter = familyInstance.LookupParameter("Sill Height");
                                                if (null != familyInstanceSillHeightParameter &&
                                                    null != newfamilyInstanceElevationFromLevelParameter)
                                                {
                                                    double sillHeight = familyInstanceSillHeightParameter.AsDouble();
                                                    newfamilyInstanceElevationFromLevelParameter.Set(sillHeight);
                                                }
                                                Parameter familyInstanceOffsetFromHostParameter = familyInstance.LookupParameter("Offset from Host");
                                                if (null == familyInstanceElevationFromLevelParameter &&
                                                    null != familyInstanceOffsetFromHostParameter)
                                                {
                                                    double offsetFromHost = familyInstanceOffsetFromHostParameter.AsDouble();
                                                    Parameter newfamilyInstanceOffsetFromHostParameter = newFamilyInstance.LookupParameter("Offset from Host");
                                                    newfamilyInstanceOffsetFromHostParameter.Set(offsetFromHost);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                trans.Commit();
            }
        }

        public static FamilySymbol FindFamilyByName(string name, Document doc)
        {
            // Create a FilteredElementCollector for FamilySymbol
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .WhereElementIsElementType();
            List<FamilySymbol> familySymbols = collector.Cast<FamilySymbol>().ToList();

            foreach (FamilySymbol familySymbol in familySymbols)
            {
                Family family = familySymbol.Family;
                if (null != family)
                { 
                    string familyName = family.Name;
                    if (null != familyName)
                    {
                        if (name == familyName)
                        {
                            return familySymbol;                        
                        }
                    }
                }
            }

            return null;
        }

        public static bool FamilyIsModelInPlace(FamilyInstance familyInstance)
        {
            if (null != familyInstance)
            {
                FamilySymbol symbol = familyInstance.Symbol;
                if (null != symbol)
                { 
                    Family fam = symbol.Family;
                    if (null != fam)
                    {
                        bool modelInInPlace = fam.IsInPlace;

                        return modelInInPlace;
                    }
                }
            }

            return false;
        }

        public static XYZ GetModelInPlaceCentroid(FamilyInstance famInstance)
        {
            Solid solid = SolidsUtils.GetSolidFromFamilyInstance(famInstance, new Options());

            if (null != solid)
            { 
                XYZ centroid = solid.ComputeCentroid();

                if (null != centroid)
                { 
                    List<XYZ> points = SolidsUtils.GetSolidPoints(solid);
                    List<double> elevations = new List<double>() { centroid.Z };

                    if (null != points)
                    { 
                        if (points.Count() > 0)
                        {
                            foreach (XYZ point in points)
                            { 
                                elevations.Add(point.Z);
                            }
                        }
                    }
                    double min = elevations.Min();
                    XYZ newCentroid = new XYZ(centroid.X, centroid.Y, min);

                    return newCentroid;
                }
            }

            return null;
        }

        #endregion
    }
}
