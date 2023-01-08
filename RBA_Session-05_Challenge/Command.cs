#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Forms = System.Windows.Forms;
using Excel = OfficeOpenXml;
using OfficeOpenXml;
using RBA_Session_05_Challenge;

#endregion

namespace RAB_Session_05_Challenge
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

            // prompt user to select Excel file

            Forms.OpenFileDialog selectFile = new Forms.OpenFileDialog();
            selectFile.Filter = "Excel files|*.xls;*.xlsx;*.xlsm";
            selectFile.InitialDirectory = "S:\\";
            selectFile.Multiselect = false;

            string excelFile = "";

            if (selectFile.ShowDialog() == Forms.DialogResult.OK)
                excelFile = selectFile.FileName;

            if (excelFile == "")
            {
                TaskDialog.Show("Error", "Please select an Excel file.");
                return Result.Failed;
            }

            // set EPPlus license context

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // open Excel file

            ExcelPackage excel = new ExcelPackage(excelFile);
            ExcelWorkbook workbook = excel.Workbook;
            ExcelWorksheet setWS = workbook.Worksheets[0];
            ExcelWorksheet typeWS = workbook.Worksheets[1];

            // get row and column count

            int setRows = setWS.Dimension.Rows;
            int typeRows = typeWS.Dimension.Rows;

            // get data into list of classes

            List<FurnitureType> furnTypeList = new List<FurnitureType>();
            List<FurnitureSet> furnSetList = new List<FurnitureSet>();

            for (int i = 1; i <= setRows; i++)
            {
                string setName = setWS.Cells[i, 1].Value.ToString();
                string setRoom = setWS.Cells[i, 2].Value.ToString();
                string setFurn = setWS.Cells[i, 3].Value.ToString();

                FurnitureSet curSet = new FurnitureSet(setName, setRoom, setFurn);
                furnSetList.Add(curSet);
            }

            for (int j = 1; j <= typeRows; j++)
            {
                string typeName = typeWS.Cells[j, 1].Value.ToString();
                string typeFamily = typeWS.Cells[j, 2].Value.ToString();
                string typeType = typeWS.Cells[j, 3].Value.ToString();

                FurnitureType curType = new FurnitureType(typeName, typeFamily, typeType);
                furnTypeList.Add(curType);
            }

            furnTypeList.RemoveAt(0);
            furnSetList.RemoveAt(0);

            int overallCounter = 0;

            // get rooms, loop through & insert correct furniture

            FilteredElementCollector colRooms = new FilteredElementCollector(doc);
            colRooms.OfCategory(BuiltInCategory.OST_Rooms);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Insert furniture sets");

                foreach (SpatialElement room in colRooms)
                {
                    int counter = 0;

                    string furnSet = Utils.GetParameterValueByName(room, "Furniture Set");

                    LocationPoint roomLocation = room.Location as LocationPoint;
                    XYZ insPoint = roomLocation.Point;

                    foreach (FurnitureSet curSet in furnSetList)
                    {
                        if (curSet.Set == furnSet)
                        {
                            foreach (string furnPiece in curSet.Furniture)
                            {
                                foreach (FurnitureType curType in furnTypeList)
                                {
                                    if (curType.Name == furnPiece.Trim())
                                    {
                                        FamilySymbol curFS = Utils.GetFamilySymbolByName(doc, curType.Family, curType.Type);

                                        if (curFS.IsActive == false)
                                            curFS.Activate();

                                        FamilyInstance instance = doc.Create.NewFamilyInstance(insPoint, curFS, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
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

            TaskDialog.Show("complete", "Inserted " + overallCounter.ToString() + " pieces of furniture.");

            return Result.Succeeded;
        }
    }
}