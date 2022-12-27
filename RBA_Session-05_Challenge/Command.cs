#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RBA_Session_05_Challenge
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Part 1) get data ino list of classes

            List<string[]> furnitureTypes = Utils.GetFurnitureTypes();
            List<string[]> furnitureSets= Utils.GetFurnitureSets();

            List<FurnitureType> furnitureTypeList = new List<FurnitureType>();
            List<FurnitureSet> furnitureSetList = new List<FurnitureSet>();

            foreach (string[] furType in furnitureTypes)
            {
                FurnitureType curType = new FurnitureType(furType[0], furType[1], furType[2]);
                furnitureTypeList.Add(curType);                
            }

            foreach (string[] furSet in furnitureSets)
            {
                FurnitureSet curType = new FurnitureSet(furSet[0], furSet[1], furSet[2]);
                furnitureSetList.Add(curType);
            }

            furnitureTypeList.RemoveAt(0);
            furnitureSetList.RemoveAt(0);

            // Part 2) get rooms, loop through rooms & insert correct furniture

            FilteredElementCollector colRooms = new FilteredElementCollector(doc);
            colRooms.OfCategory(BuiltInCategory.OST_Rooms);

            foreach (SpatialElement room in colRooms)
            {
                string furnSet = Utils.GetParameterValueByName(room, "Furniture Set");
                Debug.Print(furnSet);

                LocationPoint roomLocation = room.Location as LocationPoint;
                XYZ inspoint = roomLocation.Point;

                FamilySymbol curFS = Utils.GetFamilySymbolByName(doc, "Desk", "60in x 30in");

                FamilyInstance instance = doc.Create.NewFamilyInstance(inspoint,
                    curFS, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }

            return Result.Succeeded;
        }
    }
}
