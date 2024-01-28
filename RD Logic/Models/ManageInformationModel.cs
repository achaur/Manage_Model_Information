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
            Transaction createSchemaAndStoreData = new Transaction(Doc, "tCreateAndStore");
            createSchemaAndStoreData.Start();

            SchemaBuilder schemaBuilder =
                    new SchemaBuilder(new Guid("971AFB6F-9A52-4DCD-BD99-B184AA12455F"));
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public); // allow anyone to read the object
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public); // restrict writing to this vendor only
            schemaBuilder.SetVendorId("ADSK"); // required because of restricted write-access
            string schemaName = ManageExtensibleStorageUtils._schemaName;
            schemaBuilder.SetSchemaName(schemaName);

            // create a field to store a data
            schemaBuilder.AddSimpleField(schemaName, typeof(string));

            Schema schema = schemaBuilder.Finish(); // register the Schema object
            Entity entity = new Entity(schema); // create an entity (object) for this schema (class)
                                                // get the field from the schema
            Field fieldSpliceLocation = schema.GetField(schemaName);
            // set the value for this entity
            entity.Set<string>(fieldSpliceLocation, "g", UnitTypeId.Meters);

            Element elementToAssociateWithData = 
                ManageExtensibleStorageUtils.GetELementAssociatedWithData(Doc);

            //bind data with element that always exists in the project so the data is not lost
            elementToAssociateWithData.SetEntity(entity); // store the entity in the element

            // get the data back from the element
            Entity retrievedEntity = elementToAssociateWithData.GetEntity(schema);

            string retrievedData =
                    retrievedEntity.Get<string>(schema.GetField(schemaName),
                    UnitTypeId.Meters);

            createSchemaAndStoreData.Commit();
        }

        #endregion
    }
}
