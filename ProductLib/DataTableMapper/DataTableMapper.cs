using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace ProductLib.DataTableMapper
{
    public interface IDataTableMapper<T> where T : new()
    {
        public List<T> Import(DataTable table);
        public DataTable ExportTemplate();
    }

    public static class TableMapper<T> where T:new()
    {
      
        public static DataTable TemplateExport(bool onlySettable = true)
        {
            Type type = typeof(T);
            var properties = type.GetProperties().ToList();

            if (onlySettable)
            {
                properties = properties.Where(x => x.CanWrite).ToList();
            }
            
            DataTable table = new DataTable();
            foreach (PropertyInfo property in properties)
            {
                if ((property.GetMethod.ReturnType.IsPrimitive || property.GetMethod.ReturnType == typeof(string)))
                {
                    table.Columns.Add(property.Name);    
                }
            }
            return table;
        }

        public static DataTable Export(List<T> instances)
        {
            DataTable dt = new DataTable();
            Type type = typeof(T);
            var properties = type.GetProperties().ToList();

            foreach (var p in properties)
            {
                dt.Columns.Add(p.Name);
            }

            foreach (var item in instances)
            {
                DataRow r = dt.NewRow();
                foreach (PropertyInfo p in properties)
                {
                    var value = p.GetValue(item);
                    if (value != null)
                    {
                        r[p.Name] = value.ToString();
                    }
                }
            }

            return dt;

        }

        public static List<T> ReadThroughJson(DataTable table)
        {
            //big hack of parsing
            JsonArray jsonArray = new JsonArray();
         
            Dictionary<int, string> headers = new Dictionary<int, string>();
            for (int loop =0 ; loop < table.Columns.Count; loop++)
            {
                headers.Add(loop, table.Columns[loop].ColumnName);
            }
            foreach (DataRow row in table.Rows)
            {
                JsonObject jo = new JsonObject();
                foreach (var header in headers)
                {
                    jo[header.Value] = row[header.Key].ToString();
                }
                jsonArray.Add(jo);
            }

            return JsonConvert.DeserializeObject<List<T>>(jsonArray.ToString());

        }
    }
}