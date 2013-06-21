using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AWSResponderConsole
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        #region IEqualityComparer<T> Members
        /// <summary>
        /// Sourced from http://www.planetsourcecode.com/vb/scripts/ShowCode.asp?txtCodeId=8445&lngWId=10
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            Type type = typeof(T);
            if (typeof(IEquatable<T>).IsAssignableFrom(type)) return EqualityComparer<T>.Default.Equals(x, y);
            Type enumerableType = type.GetInterface(typeof(IEnumerable<>).FullName);
            if (enumerableType != null)
            {
                Type elementType = enumerableType.GetGenericArguments()[0];
                Type elementComparerType = typeof(GenericComparer<>).MakeGenericType(elementType);
                object elementComparer = Activator.CreateInstance(elementComparerType);
                Type rawType = typeof(ListCompare<>);
                Type specificType = rawType.MakeGenericType(elementType);
                MethodInfo mi = specificType.GetMethod("AreEqual");
                mi = mi.MakeGenericMethod(elementType);

                if (x.GetType().ToString().Contains("Dictionary"))
                {
                    PropertyInfo[] props = elementType.GetProperties();
                    mi = specificType.GetMethod("DictionaryEqual");
                    mi = mi.MakeGenericMethod(new Type[] { props[0].PropertyType, props[1].PropertyType });
                }
                bool result = (bool)mi.Invoke(null, new object[] { x, y });
                //The following throws am AmbiguousException
                //result = (bool)typeof(Enumerable).GetMethod("SequenceEqual").MakeGenericMethod(elementType)
                //.Invoke(null, new object[] { x, y, elementComparer });
                return result;
            }
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (x == null && y != null)
                {
                    return false;
                }
                else
                {

                    Type propComparerType = typeof(GenericComparer<>).MakeGenericType(prop.PropertyType);
                    object propComparer = Activator.CreateInstance(propComparerType);
                    if (y == null ||
                        !((bool)typeof(IEqualityComparer<>)
                    .MakeGenericType(prop.PropertyType)
                    .GetMethod("Equals")
                    .Invoke(propComparer, new object[] { prop.GetValue(x, null), prop.GetValue(y, null) })
                    )
                    )
                        return false;
                }
            }
            return true;
        }
        public int GetHashCode(T obj)
        {
            System.Reflection.PropertyInfo[] arrPropInfo = obj.GetType().GetProperties();
            string str = string.Empty;
            Enumerable.Range(0, arrPropInfo.Length)
                .ToList()
                .ForEach(i => str += string.Concat(arrPropInfo[i].Name, "_"));
            str = str.Substring(0, str.Length - 1);
            return str.GetHashCode();

        }

        public static ListComparisonResults<T> CompareLists<T>(List<T> a, List<T> b)
        {
            ListComparisonResults<T> results = new ListComparisonResults<T>();
            GenericComparer<T> ucomp = new GenericComparer<T>();
            results.Union = b.Union(a, ucomp).ToList<T>();
            results.Intersect = b.Intersect(a, ucomp).ToList<T>();
            results.Additions = b.Except(a, ucomp).ToList<T>();
            results.Deletions = a.Except(b, ucomp).ToList<T>();
            return results;
        }
        #endregion
    }
}
