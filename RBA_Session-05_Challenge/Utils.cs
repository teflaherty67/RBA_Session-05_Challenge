using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBA_Session_05_Challenge
{
    internal static class Utils
    {
        internal static string GetParameterValueByName(Element element, string paramName)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);

            if (paramList != null)
            {
                Parameter param = paramList[0];
                string paramValue = param.AsString();
                return paramValue;
            }

            return "";            
        }

        internal static FamilySymbol GetFamilySymbolByName(Document doc, string familyName, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol));

            foreach(FamilySymbol curFS in collector)
            {
                if (curFS.Name == typeName && curFS.FamilyName == familyName)
                    return curFS;
            }

            return null;
        }

        internal static void SetParameterByName(Element element, string paramName, int value)
        {
            IList<Parameter> paramList = element.GetParameters(paramName);

            if (paramList != null)
            {
                Parameter param = paramList[0];

                param.Set(value);
            }          
        }
    }
}
