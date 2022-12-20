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
            WallType wallT1 = Utils.GetWallTypeByName(doc, "Storefront");
            WallType wallT2 = Utils.GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType pipeSystemType = Utils.GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = Utils.GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = Utils.GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = Utils.GetDuctTypeByName(doc, "Default");

            // loop through selected CurveElements

            List<ElementId> collection = new List<ElementId>();

            Transaction t = new Transaction(doc);
            t.Start("Reveal Message");

            foreach (CurveElement curCurve in filteredList)
            {
                // get Curve and GraphicsStyle for each CurveElement

                Curve elemCurve = curCurve.GeometryCurve;
                GraphicsStyle curGS = curCurve.LineStyle as GraphicsStyle;

                // create wall, duct or pipe

                if (curGS.Name != "A-GLAZ" && curGS.Name != "A-WALL" && curGS.Name != "M-DUCT" && curGS.Name != "P-PIPE")
                {
                    collection.Add(curCurve.Id);
                    continue;
                }

                XYZ startPoint = elemCurve.GetEndPoint(0);
                XYZ endPoint = elemCurve.GetEndPoint(1);

                switch (curGS.Name)
                {
                    case "A-GLAZ":
                        Wall newWall1 = Utils.CreateWall(doc, elemCurve, wallT1, curLevel);
                        break;

                    case "A-WALL":
                        Wall newWall2 = Utils.CreateWall(doc, elemCurve, wallT2, curLevel);
                        break;

                    case "M-DUCT":
                        Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id,
                            curLevel.Id, startPoint, endPoint);
                        break;

                    case "P-PIPE":
                        Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id,
                            curLevel.Id, startPoint, endPoint);
                        break;

                    default:
                        break;
                }
            }

            // hide lines

            doc.ActiveView.HideElements(collection);

            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }
    }
}
