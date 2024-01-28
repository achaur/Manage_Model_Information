using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace ManageInfo_Core
{
    public static class ManageExtensibleStorageUtils
    {
        public static string _schemaName = "ModelInformation";

        public static Element GetELementAssociatedWithData(Document document)
        {
            // Use a filtered element collector to find Project Base Point
            FilteredElementCollector collector = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_SharedBasePoint)
                .WhereElementIsNotElementType();

            // Get Project Base Point in the model
            Element projectBasePoint = collector.FirstOrDefault();

            return projectBasePoint;
        }
        public static List<List<string>> RetrieveSchemaData(Schema schema, Element element)
        {
            Entity retrievedEntity = element.GetEntity(schema);
            // get the data from the element it is associated with
            List<List<string>> retrievedData =
                    retrievedEntity.Get<List<List<string>>>(schema.GetField(_schemaName),
                    UnitTypeId.Meters);

            return retrievedData;
        }
    }
}
