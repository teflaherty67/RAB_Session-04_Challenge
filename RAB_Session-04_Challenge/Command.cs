#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAB_Session_04_Challenge
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

            // prompt user to select elements

            TaskDialog.Show("Select lines", "Select lines to convert to Revit elements.");
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            // filter selected elements

            List<CurveElement> filteredList = new List<CurveElement>();            

            foreach (Element element in pickList)
            {
                if (element is CurveElement)
                {
                    CurveElement curve = element as CurveElement;                    
                    filteredList.Add(curve);
                }
            }            

            // get types by name

            Level curLevel = Utils.GetLevelByName(doc, "Level 1");
            WallType curWT1 = Utils.GetWallTypeByName(doc, "Storefront");
            WallType curWT2 = Utils.GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType pipeSystemType = Utils.GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = Utils.GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = Utils.GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = Utils.GetDuctTypeByName(doc, "Default");

            // loop through selected CurveElements

            Transaction t = new Transaction(doc);
            t.Start("Reveal Message");

            foreach (CurveElement curCurve in filteredList)
            {
                // get Curve and GraphicsStyle for each CurveElement

                Curve elemCurve = curCurve.GeometryCurve;
                GraphicsStyle curGS = curCurve.LineStyle as GraphicsStyle;

                // create wall, duct or pipe

                switch (curGS.Name)
                {
                    case "A-GLAZ":
                        Wall newWall1 = Wall.Create(doc, elemCurve, curWT1.Id, curLevel.Id, 20, 0, false, false);
                        break;

                    case "A-WALL":
                        Wall newWall2 = Wall.Create(doc, elemCurve, curWT2.Id, curLevel.Id, 20, 0, false, false);
                        break;

                    case "M-DUCT":
                        // Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, curLevel.Id, startPoint, endPoint);
                        break;

                    case "P-PIPE":
                        // Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, curLevel.Id, startPoint, endPoint);
                        break;

                    default:
                        break;
                }
            }
            
            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }
    }
}
