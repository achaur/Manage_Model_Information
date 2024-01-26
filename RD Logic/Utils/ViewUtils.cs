using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ManageInfo_Core
{
    public static class ViewUtils
    {
        public static bool PlanTypeOfView(View view)
        {
            if (view.ViewType == ViewType.AreaPlan ||
                view.ViewType == ViewType.CeilingPlan ||
                view.ViewType == ViewType.FloorPlan)
                return true;

            return false;
        }

        public static bool TwoDTypeOfView(View view)
        {
            if (view.ViewType == ViewType.AreaPlan ||
                view.ViewType == ViewType.CeilingPlan ||
                view.ViewType == ViewType.FloorPlan ||
                view.ViewType == ViewType.Elevation ||
                view.ViewType == ViewType.Section)
                return true;

            return false;
        }

        /// <summary>
        /// Filter for FilteredElementCollector to get elements that only cuts the current view.
        /// </summary>
        /// <returns>ElementIntersectsSolidFilter element.</returns>
        public static ElementIntersectsSolidFilter GetViewCutIntersectFilter(View view)
        {
            // Solid of view section plane for filtering
            IList<CurveLoop> viewCrop = view.GetCropRegionShapeManager().GetCropShape();
            
            // ElementIntersectsSolidFilter has issues with finding intersections
            // if no edges intersects the elements so need to move view boundaries back a little
            Transform transform = Transform.CreateTranslation(view.ViewDirection.Negate());
            foreach (CurveLoop curveLoop in viewCrop)
                curveLoop.Transform(transform);
            
            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(viewCrop, view.ViewDirection, 2);

            ElementIntersectsSolidFilter intersectFilter = new ElementIntersectsSolidFilter(solid);

            return intersectFilter;
        }
    }
}