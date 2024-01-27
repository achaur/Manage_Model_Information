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

        public List<List<string>> RowData { get; set; }

        #endregion

        #region METHODS
        private protected override void TryExecute()
        {
            //associate retrieved information with current document
            Transaction createSchemaAndStoreData = new Transaction(Doc, "tCreateAndStore");
            createSchemaAndStoreData.Start();
            SchemaBuilder schemaBuilder =
                    new SchemaBuilder(new Guid("720080CB-DA99-40DC-9415-E53F280AA1F0"));
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public); // allow anyone to read the object
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Vendor); // restrict writing to this vendor only
            schemaBuilder.SetVendorId("ADSK"); // required because of restricted write-access
            schemaBuilder.SetSchemaName("ModelInformation");
            // create a field to store a data
            FieldBuilder fieldBuilder =
                    schemaBuilder.AddSimpleField("ModelInformation", typeof(string));
            fieldBuilder.SetSpec(SpecTypeId.Length);


            fieldBuilder.SetDocumentation("A stored location value representing a wiring splice in a wall.");

            Schema schema = schemaBuilder.Finish(); // register the Schema object
            Entity entity = new Entity(schema); // create an entity (object) for this schema (class)
                                                // get the field from the schema
            Field fieldSpliceLocation = schema.GetField("ModelInformation");
            // set the value for this entity
            entity.Set<string>(fieldSpliceLocation, "", UnitTypeId.Meters);

            // Use a filtered element collector to find Project Base Point
            FilteredElementCollector collector = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_SharedBasePoint)
                .WhereElementIsNotElementType();

            // Check if there is a Project Base Point in the model
            Element projectBasePoint = collector.FirstOrDefault();

            //bind data with element that always exists in the project so the data is not lost
            projectBasePoint.SetEntity(entity); // store the entity in the element

            // get the data back from the wall
            Entity retrievedEntity = projectBasePoint.GetEntity(schema);
            XYZ retrievedData =
                    retrievedEntity.Get<XYZ>(schema.GetField("WireSpliceLocation"),
                    UnitTypeId.Meters);
            createSchemaAndStoreData.Commit();
        }

        #endregion
    }
}
