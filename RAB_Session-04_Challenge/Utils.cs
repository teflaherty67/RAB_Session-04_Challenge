using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RAB_Session_04_Skills
{
    internal static class Utils
    {
        internal static WallType GetWallTypeByName(Document doc, string wallType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));

            foreach (WallType curWT in collector)
            {
                if (curWT.Name == wallType)
                    return curWT;
            }

            return null;
        }

        internal static MEPSystemType GetMEPSystemTypeByName(Document doc, string mepSystemType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curST in collector)
            {
                if (curST.Name == mepSystemType)
                    return curST;
            }

            return null;
        }

        internal static PipeType GetPipeTypeByName(Document doc, string pipeType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));

            foreach (PipeType curPT in collector)
            {
                if (curPT.Name == pipeType)
                    return curPT;
            }

            return null;
        }

        internal static DuctType GetDuctTypeByName(Document doc, string ductType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));

            foreach (DuctType curDT in collector)
            {
                if (curDT.Name == ductType)
                    return curDT;
            }

            return null;
        }
    }
}