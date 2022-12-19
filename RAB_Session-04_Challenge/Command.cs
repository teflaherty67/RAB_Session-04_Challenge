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
                    CurveElement curve = (CurveElement)element;

                    if (curve.CurveElementType == CurveElementType.ModelCurve)
                        lineList.Add(curve);
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("Reveal Message");

            WallType curWT1 = Utils.GetWallTypeByName(doc, "Storefront");
            WallType curWT2 = Utils.GetWallTypeByName(doc, "Generic - 8\"");

            MEPSystemType pipeSystemType = Utils.GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = Utils.GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = Utils.GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = Utils.GetDuctTypeByName(doc, "Default");

            TaskDialog.Show("Results", "You have selected " + lineList.Count + " lines.");

            return Result.Succeeded;
        }
    }
}
