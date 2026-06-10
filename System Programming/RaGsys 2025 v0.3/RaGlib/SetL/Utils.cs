using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public static class Utils
    {
        public static Dictionary<K,V> Clone<K,V>(this Dictionary<K,V> dictionary)
        {
            return dictionary.ToDictionary(k => k.Key, v => v.Value);
        }

        public static string[] Split(string str, string[] del)
        {
            var result = new LinkedList<string>();
            int cf = 0;
            for(int i = 0; i < str.Length; i++)
            {
                foreach(var d in del)
                {
                    if(str.Substring(i).StartsWith(d))
                    {
                        result.AddLast(str.Substring(cf, i - cf));
                        cf = i + d.Length;
                        i += d.Length - 1;
                        break;
                    }
                }
            }
            result.AddLast(str.Substring(cf));
            return result.ToArray();
        }
    }
}
