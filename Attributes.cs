using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            HolaMundo h = new HolaMundo() { Fecha = "2018-07-06T00:00:00-05:00" };
            Console.WriteLine("{0}", h.Fecha);
            h.SetType();
            Console.WriteLine("{0}",h.Fecha);
            Console.ReadKey(); 
        }
    }



    public class HolaMundo
    {
        [Loquesea(/*cultureInfo = "", format = "", */type = dataTypeEnum.DATETIME)]
        public string Fecha { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LoqueseaAttribute : Attribute
    {
        public dataTypeEnum type { get; set; }

        /// <summary>
        /// default value yyyy-MM-dd HH:mm:ss
        /// List format http://www.csharp-examples.net/string-format-datetime/
        /// </summary>
        public string format { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// default value en-US
        /// List culture info https://msdn.microsoft.com/en-us/library/hh441729.aspx
        /// </summary>
        public string cultureInfo { get; set; } = "en-US";
    }

    public static class Extension
    {
        public static void SetType<TEntity>(this TEntity entity) where TEntity : class
        {
            Type typeEntity = typeof(TEntity);
            PropertyInfo[] properties = typeof(TEntity).GetProperties();
            Parallel.ForEach(properties, (p) =>
            {
                if (p.GetCustomAttributes(true).FirstOrDefault() is LoqueseaAttribute attribute)
                {
                    string _format = attribute.format;
                    string _culteInfo = attribute.cultureInfo;
                    string _value = GetValue(p, entity);
                    dataTypeEnum dataType = attribute.type;

                    switch (dataType)
                    {
                        case dataTypeEnum.DATETIME:
                            object result = GetDateTime(_value, _format, _culteInfo);
                            p.SetValue(entity, result); 
                            break;
                        case dataTypeEnum.INT:
                            break;
                        case dataTypeEnum.DECIMAL:
                            break;
                        default:
                            break;
                    }

                }
            });
        }

        private static string GetValue<TEntity>(PropertyInfo propertyInfo, TEntity entity) where TEntity : class
        {
            object result = propertyInfo.GetValue(entity);

            if (result is string)
                return (string)result;
            else
            {
                try
                {
                    return (string)result;
                }
                catch (InvalidCastException ex)
                {
                    throw ex;
                }
            }
        }

        private static object GetDateTime(string p_value, string p_format, string p_cultureInfo)
        {
            DateTime _resultDate = DateTime.MinValue;
            try
            {
                //p_cultureInfo = string.IsNullOrEmpty(p_cultureInfo) ? "en-US" : p_cultureInfo;

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(p_cultureInfo);
                _resultDate = DateTime.Parse(p_value,cultureInfo);
                p_format = "{0:"+ p_format + "}";
                return string.Format(p_format, _resultDate); 
            }
            catch (Exception)
            {
                return p_value;
            }
        }


    }

    public enum dataTypeEnum
    {
        DATETIME = 1,
        INT = 2,
        DECIMAL = 3
    }


}
