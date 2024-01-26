using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Core
{
    public class SolidsUtils
    {

        #region SOLID UTILS
        public static Face GetSolidBottomFace(Solid solid)
        {
            if (solid != null)
            {
                UV uv = new UV(0, 0);
                FaceArray faces = solid.Faces;

                foreach (Face face in faces)
                {
                    if (face is PlanarFace planarFace)
                    {
                        XYZ planarFaceNormal = planarFace.ComputeNormal(uv);

                        if (Math.Round(planarFaceNormal.Z, 1) == -1)
                        {
                            return planarFace;
                        }
                    }
                }
            }

            return null;
        }

        public static List<XYZ> GetSolidPoints(Solid solid)
        {
            List<XYZ> points = new List<XYZ>();
            FaceArray faces = solid.Faces;

            if (null != faces)
            { 
                foreach (Face face in faces)
                {
                    IList<CurveLoop> edgeLoops = face.GetEdgesAsCurveLoops();
                    foreach (CurveLoop edgeLoop in edgeLoops)
                    {
                        foreach (Curve curve in edgeLoop)
                        {
                            XYZ startPoint = curve.GetEndPoint(0);
                            points.Add(startPoint);
                        }
                    }
                }
            }

            return points;
        }

        public static Solid GetSolidFromFamilyInstance(Element familyInstance, Options options)
        {
            GeometryElement geomElem = familyInstance.get_Geometry(options);

            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;

                    if (geomInstance != null)
                    {
                        GeometryElement geomEle = geomInstance.GetInstanceGeometry();

                        foreach (GeometryObject geO in geomEle)
                        {
                            if (geO is Solid)
                            {
                                Solid s = geO as Solid;

                                if (s != null)
                                {
                                    if (s.Volume > 0)
                                    {
                                        return s;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (geomObj is Solid)
                {
                    Solid s = geomObj as Solid;

                    if (s != null)
                    {
                        if (s.Volume > 0)
                        {
                            return s;
                        }
                    }
                }
            }
            return null;
        }

        public static List<Solid> GetSolidListsFromFamilyInstance(Element familyInstance, Options options)
        {
            List<Solid> solids = new List<Solid>();

            GeometryElement geomElem = familyInstance.get_Geometry(options);

            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;

                    if (geomInstance != null)
                    {
                        GeometryElement geomEle = geomInstance.GetInstanceGeometry();

                        foreach (GeometryObject geO in geomEle)
                        {
                            if (geO is Solid)
                            {
                                Solid s = geO as Solid;

                                if (s != null)
                                {
                                    if (s.Volume > 0)
                                    {
                                        solids.Add(s);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (geomObj is Solid)
                {
                    Solid s = geomObj as Solid;

                    if (s != null)
                    {
                        if (s.Volume > 0)
                        {
                            solids.Add(s);
                        }
                    }
                }
            }
            return solids;
        }

        public static Solid GetSolidFromElement(Element element, Options options)
        {
            GeometryElement geometryElement1 = element.get_Geometry(options);

            foreach (GeometryObject geometryObject in geometryElement1)
            {
                if (geometryObject is Solid solid)
                {
                    if (solid != null)
                    {
                        if (solid.Volume > 0)
                        {
                            return solid;
                        }
                    }
                }
            }
            return null;
        }

        public static bool SolidsIntersectionIsValid(Solid solid1, Solid solid2)
        {
            Solid intersectionSolid = null;

            try
            {
                intersectionSolid = BooleanOperationsUtils.ExecuteBooleanOperation
                        (solid1, solid2, BooleanOperationsType.Intersect);
            }
            catch
            {
                return false;
            }

            if (intersectionSolid != null && Math.Abs(intersectionSolid.Volume) > 0.000001)
            {
                return true;
            }

            return false;
        }

        public static bool SolidsIntersect(Solid solid1, Solid solid2)
        {
            if (solid1 == null || solid2 == null)
                return false;

            try
            {
                Solid intersectionSolid = null;
                intersectionSolid = BooleanOperationsUtils.ExecuteBooleanOperation
                            (solid1, solid2, BooleanOperationsType.Intersect);

                if (null != intersectionSolid && Math.Abs(intersectionSolid.Volume) > 0.000001)
                {
                    return true;
                }

                Solid unionSolid = null;

                unionSolid = BooleanOperationsUtils.ExecuteBooleanOperation
                        (solid1, solid2, BooleanOperationsType.Union);

                int sumFaces = Math.Abs(solid1.Faces.Size + solid2.Faces.Size);
                int unionFaces = Math.Abs(unionSolid.Faces.Size);

                //volumes
                decimal solid1Volume = GenericUtils.DecimalTruncate((decimal)solid1.Volume);
                decimal solid2Volume = GenericUtils.DecimalTruncate((decimal)solid2.Volume);
                decimal originalSolidsVolumesSum = GenericUtils.DecimalTruncate((decimal)solid1Volume + (decimal)solid2Volume);
                decimal unionSolidVolume = GenericUtils.DecimalTruncate((decimal)unionSolid.Volume);

                double sumArea = Math.Round(Math.Abs(solid1.SurfaceArea + solid2.SurfaceArea), 5);
                double unionArea = Math.Round(Math.Abs(unionSolid.SurfaceArea), 5);

                if (originalSolidsVolumesSum > unionSolidVolume || sumFaces > unionFaces)
                {
                    return true;
                }

                /*
                if (sumArea == unionArea && sumFaces == unionFaces && intersectionSolid.Volume < 0.00001)
                    return false;

                */

                if (SolidsHaveAdjacentFaces(solid1, solid2))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static List<Element> GetIntersectingSolids(Element elementCut, Document Doc, BuiltInCategory builtInCategory)
        {
            FilteredElementCollector col = new FilteredElementCollector(Doc, Doc.ActiveView.Id);

            Solid gSolid = (elementCut is FamilyInstance) ?
                (GetSolidFromFamilyInstance(elementCut, new Options())) :
                (GetSolidFromElement(elementCut, new Options()));

            ElementIntersectsSolidFilter filter = new ElementIntersectsSolidFilter(gSolid);

            List<Element> intersectingEles = col.OfCategory(builtInCategory).WherePasses(filter).ToElements().ToList();

            return intersectingEles;
        }

        public static bool SolidsHaveAdjacentFaces(Solid solid1, Solid solid2)
        {
            FaceArray faces1 = solid1.Faces;
            FaceArray faces2 = solid2.Faces;

            foreach (Face face1 in faces1)
            {
                foreach (Face face2 in faces2)
                {
                    if (FacesAreParalel(face1, face2) == true)
                    {
                        Surface surface1 = face1.GetSurface();
                        double distance = 10000000;
                        UV uv;

                        surface1.Project(GetFacePoints(face2)[0], out uv, out distance);

                        if (Math.Abs(distance) < 0.00001)
                        {
                            Solid createdSolid1 = GeometryCreationUtilities.
                            CreateExtrusionGeometry(face1.GetEdgesAsCurveLoops(),
                            face1.ComputeNormal(new UV(0, 0)), 0.1);
                            Solid createdSolid2 = GeometryCreationUtilities.
                            CreateExtrusionGeometry(face2.GetEdgesAsCurveLoops(),
                            face1.ComputeNormal(new UV(0, 0)), 0.2);

                            if (SolidsIntersectionIsValid(createdSolid1, createdSolid2) == true)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public static List<XYZ> GetFacePoints(Face face)
        {
            List<XYZ> points = new List<XYZ>();

            IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                foreach (XYZ p in GetCurveLoopsPoints(curveLoop))
                {
                    points.Add(p);
                }
            }
            return points;
        }

        public static List<XYZ> GetCurveLoopsPoints(CurveLoop curveLoop)
        {
            List<XYZ> points = new List<XYZ>();

            foreach (Curve curve in curveLoop)
            {
                XYZ point0 = curve.GetEndPoint(0);
                XYZ point1 = curve.GetEndPoint(1);

                if (points == null)
                {
                    points.Add(point0);
                    points.Add(point1);
                }
                if (!points.Contains(point0))
                    points.Add(point0);
                if (!points.Contains(point1))
                    points.Add(point1);
            }
            return points;
        }

        public static bool FacesAreParalel(Face face1, Face face2)
        {
            XYZ faceNormal1 = face1.ComputeNormal(new UV(0, 0));
            XYZ faceNormal2 = face2.ComputeNormal(new UV(0, 0));

            double angleInRadians = faceNormal1.AngleTo(faceNormal2);

            if (Math.Abs(angleInRadians - Math.PI) < 0.01 || Math.Abs(angleInRadians) < 0.01)
                return true;

            return false;
        }
        #endregion
    }
}
