using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FEML
{
    internal class Serialization
    {
        public static string Serialize(Dictionary<string, object> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in data)
            {
                sb.Append(item.Key.ToString());
                sb.Append(" = ");

                if (item.Value is Dictionary<string, object>)
                {
                    AppendStruct(sb, (Dictionary<string, object>)item.Value);
                }
                else if (item.Value is List<object>)
                {
                    AppendArray(sb, (List<object>)item.Value);
                }
                else
                {
                    sb.Append(Convert.ToString(item.Value, CultureInfo.InvariantCulture));
                }

                sb.AppendLine(";");
            }

            return sb.ToString();
        }

        private static void AppendArray(StringBuilder sb, List<object> input)
        {
            sb.Append("[");
            var last = input.Last();
            foreach (var field in input)
            {
                sb.Append(field);
                if (field != last)
                    sb.Append(", ");
            }
            sb.Append("]");
        }

        private static void AppendStruct(StringBuilder sb, Dictionary<string, object> input, int depth = 0)
        {
            sb.Append("{").AppendLine();
            foreach (var field in input)
            {
                if (field.Value is Dictionary<string, object>)
                {
                    sb.Append("\t").Append($"{field.Key} = ");
                    AppendStruct(sb, (Dictionary<string, object>)field.Value, depth + 1);
                    sb.AppendLine();
                }
                else if (field.Value is List<object>)
                {
                    sb.Append("\t").Append($"{field.Key} = ");
                    AppendArray(sb, (List<object>)field.Value);
                    sb.AppendLine();
                }
                else
                {
                    for (int i = 0; i < depth; i++)
                        sb.Append("\t");
                    sb.Append("\t").AppendLine($"{field.Key} = {Convert.ToString(field.Value, CultureInfo.InvariantCulture)};");
                }
            }

            for (int i = 0; i < depth; i++)
                sb.Append("\t");
            sb.Append("}");
            if (depth > 0)
                sb.Append(";");
        }
    }
}