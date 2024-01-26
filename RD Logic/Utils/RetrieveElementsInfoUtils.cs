using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ManageInfo_Core
{
    public class RetrieveElementsInfoUtils
    {
        private static List<BuiltInCategory> cats = new List<BuiltInCategory> { BuiltInCategory.OST_Walls,
                                                                                BuiltInCategory.OST_Floors,
                                                                                BuiltInCategory.OST_Roofs,
                                                                                BuiltInCategory.OST_StructuralColumns,
                                                                                BuiltInCategory.OST_StructuralFraming,
                                                                                BuiltInCategory.OST_Ceilings
                                                                               };

        public static List<BuiltInCategory> GetObservableCategories(bool[,] categoriesMatrix)
        {
            List<BuiltInCategory> observableCats = new List<BuiltInCategory>();

            for (int i = 0; i < cats.Count(); i++)
            {
                for (int k = i; k < cats.Count(); k++)
                {
                    if (categoriesMatrix[i, k] != false)
                    {
                        if (!observableCats.Contains(cats[i]))
                        {
                            observableCats.Add(cats[i]);
                        }
                        if (i != k)
                        { 
                            if (!observableCats.Contains(cats[k]))
                            {
                                observableCats.Add(cats[k]);
                            }
                        }

                        categoriesMatrix[k, i] = false;
                    }
                }
            }

            return observableCats;
        }

        public static string CheckElementsOwnershipStatus(Document Doc, bool[,] categoriesMatrix, bool currentView)
        {
            int totalCount = 0;

            List<BuiltInCategory> observableCats = GetObservableCategories(categoriesMatrix);

            if (observableCats.Count() == 0)
                return "";

            FilteredElementCollector col = null;

            if (currentView == true)
            {
                col = new FilteredElementCollector(Doc, Doc.ActiveView.Id);
            }
            else
            { 
                col = new FilteredElementCollector(Doc);
            }
            
            foreach (BuiltInCategory cat in observableCats)
            {
                List<ElementId> eleIds = col.OfCategory(cat).
                        WhereElementIsNotElementType().
                        ToElementIds().ToList();

                if (eleIds != null)
                { 
                    //check ownership
                    int countOfNotIObtainableEles = eleIds.Count(x => WorksharingUtils.GetCheckoutStatus(Doc, x) == CheckoutStatus.OwnedByOtherUser);

                    if (countOfNotIObtainableEles > 0)
                        totalCount += countOfNotIObtainableEles;
                }
            }

            string res = (totalCount > 0) ? $"{totalCount} elements will not be edited because of ownership." : "";

            return res;
        }

        public static bool WorksharedElementIsObtainable(Document doc, ElementId eleId)
        {
            if (doc.IsWorkshared)
            { 
                if (WorksharingUtils.GetCheckoutStatus(doc, eleId) == CheckoutStatus.OwnedByOtherUser)
                {
                    return false;
                }
            }

            return true;
        }

        public static int GetNumberOfElementsInModel(Document doc, params BuiltInCategory[] builtInCategoryList)
        {
            int count = 0;

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                ICollection<Element> eles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

                count += eles.Count;
            }
            return count;
        }

        public static int GetNumberOfElementsInModel(Document doc, Autodesk.Revit.DB.View view, params BuiltInCategory[] builtInCategoryList)
        {
            int count = 0;

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                ICollection<Element> eles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

                count += eles.Count;
            }
            return count;
        }

        public static List<Element> GetElementsOfMultipleCategories(Document doc, Autodesk.Revit.DB.View view, params BuiltInCategory[] builtInCategoryList)
        {
            List<Element> elements = new List<Element>();

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                IList<Element> elesOfCurrentCategory = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                elements.AddRange(elesOfCurrentCategory);
            }
            return elements;
        }

        public static List<Element> GetElementsOfMultipleCategories(Document doc, params BuiltInCategory[] builtInCategoryList)
        {
            List<Element> elements = new List<Element>();

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                IList<Element> elesOfCurrentCategory = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                elements.AddRange(elesOfCurrentCategory);
            }
            return elements;
        }

        public static List<IList<Element>> GetListsOfElementsOnCurrentView(Document doc, Autodesk.Revit.DB.View view,
            params BuiltInCategory[] builtInCategoryList)
        {
            List<IList<Element>> elements = new List<IList<Element>>();

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                IList<Element> elesOfCurrentCategory = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

                elements.Add(elesOfCurrentCategory);
            }
            return elements;
        }

        public static List<IList<Element>> GetListsOfElements(Document doc,
            params BuiltInCategory[] builtInCategoryList)
        {
            List<IList<Element>> elements = new List<IList<Element>>();

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                IList<Element> elesOfCurrentCategory = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

                elements.Add(elesOfCurrentCategory);
            }
            return elements;
        }

        public static List<ICollection<ElementId>> GetCollectionsOfElementIdsOnCurrentView(Document doc,
            params BuiltInCategory[] builtInCategoryList)
        {
            List<ICollection<ElementId>> elements = new List<ICollection<ElementId>>();

            for (int i = 0; i < builtInCategoryList.Length; i++)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategoryList[i]);

                ICollection<ElementId> elesOfCurrentCategory = collector.WherePasses(filter).WhereElementIsNotElementType().ToElementIds();

                elements.Add(elesOfCurrentCategory);
            }
            return elements;
        }

        //comment
        public static int GetNumberElementsOnCurrentView(Document doc, Autodesk.Revit.DB.View view,
            BuiltInCategory builtInCategory)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

            ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategory);

            ICollection<Element> eles = collector.WherePasses(filter).ToElements();

            return eles.Count;
        }
        public static IList<Element> GetSpecificElementsOnCurrentView(Document doc, Autodesk.Revit.DB.View view,
            BuiltInCategory builtInCategory)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

            ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategory);

            IList<Element> eles = collector.WherePasses(filter).ToElements();

            return eles;
        }

        public static List<ElementId> GetBasicWallsIdsView(Document doc, bool currentView)
        {
            FilteredElementCollector collector;
            if (currentView == true)
                collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            else
                collector = new FilteredElementCollector(doc);

            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            ICollection<ElementId> elesIds = collector.WherePasses(filter).WhereElementIsNotElementType().ToElementIds();


            List<ElementId> basicWalls = new List<ElementId>();

            foreach (ElementId eleId in elesIds)
            {
                Wall wall = doc.GetElement(eleId) as Wall;
                if (null == wall.CurtainGrid)
                    basicWalls.Add(eleId);
            }

            return basicWalls;
        }

        public static ICollection<ElementId> GetCategoryElementsIdsView(Document doc, BuiltInCategory builtInCategory, bool currentView)
        {
            FilteredElementCollector collector;
            if (currentView == true)
                collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            else
                collector = new FilteredElementCollector(doc);

            ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategory);
            ICollection<ElementId> eles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElementIds();

            return eles;
        }
    }
}
