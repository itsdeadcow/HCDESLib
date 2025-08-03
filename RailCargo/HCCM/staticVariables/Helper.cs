using System.Collections;
using System.Linq;
using System.Text;
using RailCargo.HCCM.Input;

namespace RailCargo.HCCM.staticVariables
{
    public class Helper
    {
        public static void Print(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public static string StringifyCollection(IEnumerable collection)
        {
            if (collection.Cast<object>().Any() == false)
            {
                return "[]";
            }
            StringBuilder builder = new StringBuilder();
            foreach (var item in collection)
            {
                
                if (item is IEnumerable innercollection && !(item is UsedTrains) &&  !(item is string))
                {
                    builder.Append("[");
                    var innerResult = StringifyCollection(innercollection);
                    if (!string.IsNullOrEmpty(innerResult))
                    {
                        builder.Append(innerResult.TrimEnd(',', ' '));
                    }

                    builder.Append("], \n");
                }
                else if (item is UsedTrains)
                {
                    var element = (UsedTrains)item;
                    builder.Append("[");
                    builder.Append(element.TrainId + ",");
                    builder.Append(element.StartLocation + ",");
                    builder.Append(element.StartTime + ",");

                    builder.Append("], ");
                }
                else
                {
                    builder.Append(item + ", ");
                }
            }

            return builder.ToString();
        }
    }
}