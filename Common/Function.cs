using System.Data;
using System.Reflection;

namespace MESCHECKLIST.Common
{
    public static class Function
    {
        public static List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();
                foreach (DataColumn column in dataTable.Columns)
                {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(column.ColumnName);
                    if (propertyInfo != null && row[column] != DBNull.Value)
                    {
                        try
                        {
                            // Handle nullable types
                            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                            object safeValue = Convert.ChangeType(row[column], propertyType);
                            propertyInfo.SetValue(obj, safeValue, null);
                        }
                        catch (InvalidCastException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
                list.Add(obj);
            }

            return list;
        }
        public static T ToObject<T>(this IDictionary<string, object> source)
       where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType.GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }
        public static string GetXML(DataTable dt)
        {
            string XMl = "";


            if (dt != null)
            {
                XMl = "";
                if (dt.Rows.Count > 0)
                {
                    XMl = "<DataTable>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        XMl = XMl + "<DataRow>";
                        foreach (DataColumn dc in dt.Columns)
                        {
                            XMl = XMl + string.Format("<{0}>{1}</{0}>", dc.ColumnName, EscapeXmlString((dr[dc.ColumnName]).ToString()));
                        }
                        XMl = XMl + "</DataRow>";
                    }
                    XMl = XMl + "</DataTable>";
                }

            }

            return XMl;
        }
        public static string EscapeXmlString(string input)
        {
            return System.Security.SecurityElement.Escape(input);
        }

        public static string MapPath(string path)
        {
            return Path.Combine(
                (string)AppDomain.CurrentDomain.GetData("ContentRootPath"),
                path);
        }

       public static string GetImageMimeType(string path)
        {
            var extension=Path.GetExtension(path);
            switch(extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                    default:
                    return "application/octet-stream";
            }
        }

        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            // Create the result table, and gather all properties of a T
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add columns to DataTable for each property
            foreach (PropertyInfo prop in props)
            {
                // Define the column using the property name and type
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Add rows to DataTable
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    // Retrieve the value of the ith property of item
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static DataTable ConvertObecjtToDataTable<T>(T obj)
        {
            // Create the result table, and gather all properties of a T
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add columns to DataTable for each property
            foreach (PropertyInfo prop in props)
            {
                // Define the column using the property name and type
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    // Retrieve the value of the ith property of item
                    values[i] = props[i].GetValue(obj, null);
                }
                dataTable.Rows.Add(values);
         
            return dataTable;
        }

        public static bool ValidateRights(HttpContext context, string comingUrl)
        {
            var rights = context.Session.GetString("UserRights");

            if (string.IsNullOrEmpty(rights))
            {
                return false;
            }
            else
            {
                var rightsList= rights.Split(',');
                if (rightsList.Contains(comingUrl))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<List<T>> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static string GenerateUniqueCode(string mobileNumber)
        {
            string j = "";
            try
            {
                string milSc = DateTime.Now.Millisecond.ToString();
                string second = DateTime.Now.Second.ToString();
                string totalUsers = mobileNumber.Substring(Math.Max(0, mobileNumber.Length - 2)); // Extract last 2 digits of the mobile number

                j = (milSc.Length > 1 ? milSc : milSc.PadLeft(2, '0')) +
                    totalUsers.PadRight(4, '0') +
                    (second.Length > 1 ? second : second.PadLeft(2, '0'));

                if (j.Length > 6)
                {
                    j = j.Substring(0, 6);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Function at GenerateUniqueCode Exception: " + ex.Message);
            }
            return j;
        }

        public static DataTable ConvertToDataTable<T>(T obj)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get all the properties of the object
            var properties = typeof(T).GetProperties();

            // Add columns to the DataTable
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Add a single row with the object's values
            var row = dataTable.NewRow();
            foreach (var prop in properties)
            {
                row[prop.Name] = prop.GetValue(obj) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);

            return dataTable;
        }
        public static DataTable ConvertListToDataTable<T>(IList<T> list)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get all the properties of the type
            var properties = typeof(T).GetProperties();

            // Create columns in the DataTable based on the properties
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Add rows to the DataTable
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        // Function to Convert List to DataTable
        public static DataTable ConvertListStringToDataTable(List<string> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TeamMembersEmpCode", typeof(string)); // Define column name and type

            foreach (string value in list)
            {
                dt.Rows.Add(value);
            }

            return dt;
        }

    }
}
