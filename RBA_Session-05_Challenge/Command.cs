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

            int overallCounter = 0;

            // Part 2) get rooms, loop through rooms & insert correct furniture

            FilteredElementCollector colRooms = new FilteredElementCollector(doc);
            colRooms.OfCategory(BuiltInCategory.OST_Rooms);

            using (Transaction t = new Transaction(doc))                
            {
                t.Start("Insert Desk family");

                foreach (SpatialElement room in colRooms)
                {
                    int counter = 0;
                    
                    string furnSet = Utils.GetParameterValueByName(room, "Furniture Set");
                    Debug.Print(furnSet);

                    LocationPoint roomLocation = room.Location as LocationPoint;
                    XYZ inspoint = roomLocation.Point;

                    foreach(FurnitureSet curSet in furnitureSetList)
                    {
                        if(curSet.Set == furnSet)
                        {
                            foreach(string furnPiece in curSet.Furniture)
                            {
                                foreach(FurnitureType curType in furnitureTypeList)
                                {
                                    if(curType.Name == furnPiece.Trim())
                                    {
                                        FamilySymbol curFS = Utils.GetFamilySymbolByName(doc,
                                            curType.Family, curType.Type);
                                        curFS.Activate();

                                        FamilyInstance instance = doc.Create.NewFamilyInstance(inspoint,
                                            curFS, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        counter++;
                                        overallCounter++;
                                    }
                                }
                            }
                        }
                    }

                    Utils.SetParameterByName(room, "Furniture Count", counter);
                }

                t.Commit();
            }

            TaskDialog.Show("Complete", "Inserted " + overallCounter.ToString() + " pieces of furniture");

            return Result.Succeeded;
        }
    }
}
