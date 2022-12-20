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

            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            List<CurveElement> lineList = new List<CurveElement>();

            foreach (Element element in pickList)
            {
                if (element is CurveElement)
                {
                    CurveElement curve = element as CurveElement;                    
                    lineList.Add(curve);
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("Reveal Message");

            Level curLevel = Utils.GetLevelByName(doc, "Level 1");
            WallType curWT1 = Utils.GetWallTypeByName(doc, "Storefront");
            WallType curWT2 = Utils.GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType pipeSystemType = Utils.GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = Utils.GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = Utils.GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = Utils.GetDuctTypeByName(doc, "Default");

            foreach (CurveElement curCurve in lineList)
            {
                GraphicsStyle curGS = curCurve.LineStyle as GraphicsStyle;
                Debug.Print(curGS.Name);

                Curve curve = curCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                switch (curGS.Name)
                {
                    case "A-GALZ":
                        Wall newWall1 = Wall.Create(doc, curve, curWT1.Id, curLevel.Id, 20, 0, false, false);
                        break;

                    case "A-WALL":
                        Wall newWall2 = Wall.Create(doc, curve, curWT2.Id, curLevel.Id, 20, 0, false, false);
                        break;

                    case "M-DUCT":
                        Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, curLevel.Id, startPoint, endPoint);
                        break;

                    case "P-PIPE":
                        Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, curLevel.Id, startPoint, endPoint);
                        break;

                    default:
                        break;
                }
            }   

            return Result.Succeeded;
        }
    }
}
