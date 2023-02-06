using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }
}
