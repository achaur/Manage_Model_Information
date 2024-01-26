using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Core
{
    public class BoundingBoxUtils
    {
        #region BOUNDING BOX UTILS

        public static double[] volumesOfElementsToJoin = new double[] { -0.1, 0.2};
        public static double[] volumesOfElementsToUnjoin = new double[] { 0.2 };
        public static IList<Element> GetAdjacentElementsBoundingBox(Element elementCut, Document Doc, ICollection<ElementId> elementCutIds, double tolerance)
        {
            BoundingBoxXYZ bb = elementCut.get_BoundingBox(Doc.ActiveView);
            if (bb == null) { return null; }
             
            Outline outline = new Outline(bb.Min, bb.Max);

            BoundingBoxIntersectsFilter intersectBoxFilterOverlap = new BoundingBoxIntersectsFilter(outline, tolerance);

            // Apply filter to elements to find only elements that overlap the given element.
            IList<Element> elementsCutOverlap = new FilteredElementCollector(Doc, elementCutIds)
                .WherePasses(intersectBoxFilterOverlap).ToElements();

            IList<Element> elementsAlreadyChecked = GetIntersectingElementsBoundingBox(elementCut, Doc, elementCutIds, volumesOfElementsToJoin[0]);

            List<Element> uncheckedElements = new List<Element>();

            foreach (Element ele in elementsCutOverlap)
            {
                if (!elementsAlreadyChecked.Contains(ele))
                    uncheckedElements.Add(ele);
            }

            return elementsCutOverlap;
        }

        public static IList<Element> GetIntersectingElementsBoundingBoxWithinRange(Element elementCut, Document Doc,
            ICollection<ElementId> elementCutIds, double lowerRange, double upperRange)
        {
            // Apply filter to elements to find only elements that overlap the given element.
            IList<Element> elementsCutIntersect = GetIntersectingElementsBoundingBox(elementCut, Doc, elementCutIds, lowerRange);

            IList<Element> elementsCutNearby = GetIntersectingElementsBoundingBox(elementCut, Doc, elementCutIds, upperRange);

            List<Element> elementsAdjacent= new List<Element>();

            foreach (Element ele in elementsCutNearby)
            {
                if (!elementsCutIntersect.Contains(ele))
                { 
                    elementsAdjacent.Add(ele);
                }
            }

            return elementsAdjacent;
        }

        public static IList<Element> GetIntersectingElementsBoundingBox(Element elementCut, Document Doc,
            ICollection<ElementId> elementCutIds, double tolerance)
        {
            BoundingBoxXYZ bb = elementCut.get_BoundingBox(Doc.ActiveView);

            if (bb == null) { return null; }

            Outline outline = new Outline(bb.Min, bb.Max);

            BoundingBoxIntersectsFilter intersectBoxFilterOverlap = new BoundingBoxIntersectsFilter(outline, tolerance);

            // Apply filter to elements to find only elements that overlap the given element.
            IList<Element> elementsCutOverlap = new FilteredElementCollector(Doc, elementCutIds)
                    .WherePasses(intersectBoxFilterOverlap).ToElements();

            return elementsCutOverlap;
        }

        public static bool BoundingBoxIsInside(BoundingBoxXYZ inner, BoundingBoxXYZ outer)
        {
            bool inside =
                outer.Min.X <= inner.Min.X &&
                outer.Max.X >= inner.Max.X &&
                outer.Min.Y <= inner.Max.Y &&
                outer.Max.Y >= inner.Max.Y &&
                outer.Min.Z <= inner.Min.Z &&
                outer.Max.Z >= inner.Max.Z;

            return inside;
        }
        #endregion
    }
}
