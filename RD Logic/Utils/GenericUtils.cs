using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ManageInfo_Core
{
    public static class GenericUtils
    {
        public const int decimalPercision = 10;

        public static decimal DecimalTruncate(decimal inputDecimal)
        {
            decimal decimalTrunc = Math.Truncate(inputDecimal * 1000000) / 1000000;
            return decimalTrunc;
        }

        public static string FormatTime(TimeSpan timeSpan)
        {
            string formattedTime;
            double workingDays;
            double hours;
            double minutes;

            if (timeSpan.TotalMinutes == 0 && timeSpan.TotalSeconds != 0)
                minutes = 1;
            else
                minutes = Math.Round(timeSpan.TotalMinutes, 0);

            if (timeSpan.TotalHours == 0 && timeSpan.TotalMinutes == 0)
                return "Nothing to model";

            if (timeSpan.TotalHours > 9)
            {
                workingDays = Math.Round(timeSpan.TotalHours / 9, 0);
                hours = Math.Round(workingDays % 9, 0);

                formattedTime = $"Time required to model is {workingDays} working days, {hours} hours, " +
                    $"and {minutes} minutes";
            }
            else
                formattedTime = $"Time required to model is {(int)timeSpan.TotalHours} hours, and {minutes} minutes";

            return formattedTime;
        }


        //test comment
        public static ICollection<ElementId> GetCategoryElementsIdsViewFilter(Document doc, 
            BuiltInCategory builtInCategory, bool curView, ElementIntersectsSolidFilter intersectFilter)
        {
            FilteredElementCollector collector = (curView == false) ?
                (new FilteredElementCollector(doc)) :
                (new FilteredElementCollector(doc, doc.ActiveView.Id));

            ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategory);
            ICollection<ElementId> eles = null;

            if (intersectFilter == null)
                eles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElementIds();
            else
                eles = collector.WherePasses(filter).WherePasses(intersectFilter).WhereElementIsNotElementType().ToElementIds();

            return eles;
        }

        public static ICollection<ElementId> GetCategoryElementsIds(Document doc, BuiltInCategory builtInCategory)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            ElementCategoryFilter filter = new ElementCategoryFilter(builtInCategory);

            ICollection<ElementId> eles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElementIds();

            return eles;
        }

        public static string EstimateTimeAnnotate(bool Hagasha, bool Haga, bool Mecher, bool WP, bool Cost,
                                                  bool Junior, bool Middle, bool Senior,
                                                  bool CurrentView, bool WholeModel,
                                                  Document doc, View view)
        {
            double totaltime = 0.0;
            int numberEles = 0;
            List<Element> elesToAnnotate = new List<Element>();

            // specify list of elements to annotate for hagasha
            if (Hagasha)
            {

            }

            // specify list of elements to annotate for haga
            if (Haga)
            {

            }

            // specify list of elements to annotate for mecher
            if (Mecher)
            {

            }
            // specify list of elements to annotate for wp
            if (WP)
            {

            }
            // specify list of elements to annotate for cost estimation
            if (Cost)
            {

            }

            if (CurrentView)
                numberEles += RetrieveElementsInfoUtils.GetNumberOfElementsInModel(doc, view,
                    BuiltInCategory.OST_Walls, BuiltInCategory.OST_Windows, BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_Floors, BuiltInCategory.OST_Ceilings, BuiltInCategory.OST_Stairs,
                    BuiltInCategory.OST_Rooms);
            if (WholeModel)
                numberEles += RetrieveElementsInfoUtils.GetNumberOfElementsInModel(doc,
                    BuiltInCategory.OST_Walls, BuiltInCategory.OST_Windows, BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_Floors, BuiltInCategory.OST_Ceilings, BuiltInCategory.OST_Stairs,
                    BuiltInCategory.OST_Rooms);

            if (numberEles == 0)
                return "Nothing to annotate";

            totaltime = 5.0 * numberEles;

            //correlate value according to the speed
            if (Junior)
                totaltime = totaltime * 2;

            if (Middle)
                totaltime = totaltime * 1.5;

            TimeSpan timeSpan = TimeSpan.FromSeconds(totaltime);

            return FormatTime(timeSpan);
        }
        public static string EstimateTimeModel(int numberOfFloors, int numberOfTypicalFloors, double area,
                                               bool coarse, bool medium, bool fine,
                                               bool junior, bool middle, bool senior)
        {
            double totaltime = 0.0;
            double levelOfDetail = 1;
            double speed = 1;
            double timePerArea = 60;
            int numberF = numberOfFloors - numberOfTypicalFloors + 1;

            totaltime = area * timePerArea * numberF;

            if (medium) levelOfDetail = 1.5;
            if (fine) levelOfDetail = 2;

            if (middle) speed = 0.75;
            if (senior) speed = 0.25;

            totaltime = totaltime * levelOfDetail * speed;

            TimeSpan timeSpan = TimeSpan.FromSeconds(totaltime);

            return FormatTime(timeSpan);
        }
    }
}
