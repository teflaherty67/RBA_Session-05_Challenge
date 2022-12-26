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

            return Result.Succeeded;
        }
    }
}
