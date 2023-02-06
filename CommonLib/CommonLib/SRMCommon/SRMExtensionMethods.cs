using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    public static class SRMExtensionMethods
    {
        public static bool SRMEqualWithIgnoreCase(this String sStr, string tStr)
        {
            return sStr.Trim().Equals(tStr.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool SRMContainsWithIgnoreCase(this IDictionary<string, string> dictionary, string keyName)
        {
            return dictionary.Keys.Any(keyItem => keyItem.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool SRMContainsWithIgnoreCase(this IDictionary<string, DataTable> dictionary, string keyName)
        {
            return dictionary.Keys.Any(keyItem => keyItem.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool SRMContainsWithIgnoreCase(this IEnumerable<string> list, string keyName)
        {
            return list.Any(keyItem => keyItem.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool SRMContainsWithIgnoreCase(this string[] stringArray, string keyName)
        {
            return stringArray.Any(keyItem => keyItem.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool SRMContainsWithIgnoreCase(this ArrayList array, string keyName)
        {
            bool contains = false;
            foreach(var item in array)
            {
                if (Convert.ToString(item).SRMEqualWithIgnoreCase(keyName))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

    public class Enum<T> where T : struct, IConvertible
    {
        public static int Count
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");

                return Enum.GetNames(typeof(T)).Length;
            }
        }

        public static T Parse(string description)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            Dictionary<string, string> dctDescription = Enum<T>.NameVsDescription;
            return (T)Enum.Parse(typeof(T), dctDescription.FirstOrDefault(x => x.Value == description).Key);
        }

        public static T Parse(string description, bool ignoreCase)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            Dictionary<string, string> dctDescription = Enum<T>.NameVsDescription;
            var value = dctDescription.FirstOrDefault(x => x.Value.Equals(description, StringComparison.OrdinalIgnoreCase));
            return (T)Enum.Parse(typeof(T), dctDescription.FirstOrDefault(x => x.Value.Equals(description, StringComparison.OrdinalIgnoreCase)).Key);
        }

        public static bool TryParse(string description, out T result)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            Dictionary<string, string> dctDescription = Enum<T>.NameVsDescription;
            return Enum.TryParse(dctDescription.FirstOrDefault(x => x.Value == description).Key, out result);
        }

        public static bool TryParse(string description, bool ignoreCase, out T result)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            Dictionary<string, string> dctDescription = Enum<T>.NameVsDescription;
            var value = dctDescription.FirstOrDefault(x => x.Value.Equals(description, StringComparison.OrdinalIgnoreCase));
            return Enum.TryParse(dctDescription.FirstOrDefault(x => x.Value.Equals(description, StringComparison.OrdinalIgnoreCase)).Key, out result);
        }

        public static string GetDescription(object value)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static List<string> GetDescriptions()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            List<string> lstDescription = new List<string>();
            Enum.GetNames(typeof(T)).ToList().ForEach(name =>
            {
                object value = Enum.Parse(typeof(T), name);
                Type type = value.GetType();
                if (name != null)
                {
                    FieldInfo field = type.GetField(name);
                    if (field != null)
                    {
                        DescriptionAttribute attr =
                               Attribute.GetCustomAttribute(field,
                                 typeof(DescriptionAttribute)) as DescriptionAttribute;
                        if (attr != null)
                        {
                            lstDescription.Add(attr.Description);
                        }
                    }
                }
            }
            );
            return lstDescription;
        }

        public static Dictionary<string, string> NameVsDescription
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");
                Dictionary<string, string> dctDescription = new Dictionary<string, string>();
                Enum.GetNames(typeof(T)).ToList().ForEach(name =>
                {
                    object value = Enum.Parse(typeof(T), name);

                    Type type = value.GetType();
                    if (name != null)
                    {
                        FieldInfo field = type.GetField(name);
                        if (field != null)
                        {
                            DescriptionAttribute attr =
                                   Attribute.GetCustomAttribute(field,
                                     typeof(DescriptionAttribute)) as DescriptionAttribute;
                            if (attr != null)
                            {
                                dctDescription.Add(name, attr.Description);
                            }
                        }
                    }
                }
                );
                return dctDescription;
            }
        }

        public static Dictionary<int, string> ValueVsDescription
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");
                Dictionary<int, string> dctDescription = new Dictionary<int, string>();
                Enum.GetNames(typeof(T)).ToList().ForEach(name =>
                {
                    object value = Enum.Parse(typeof(T), name);
                    Type type = value.GetType();
                    if (name != null)
                    {
                        FieldInfo field = type.GetField(name);
                        if (field != null)
                        {
                            DescriptionAttribute attr =
                                   Attribute.GetCustomAttribute(field,
                                     typeof(DescriptionAttribute)) as DescriptionAttribute;
                            if (attr != null)
                            {
                                dctDescription.Add(Convert.ToInt32(value), attr.Description);
                            }
                        }
                    }
                }
                );
                return dctDescription;
            }
        }
    }
}
