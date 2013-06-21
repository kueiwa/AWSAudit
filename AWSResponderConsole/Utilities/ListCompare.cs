using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ListCompare<T>
    {
        public static ListComparisonResults<T> CompareLists<T>(List<T> a, List<T> b)
        {
            ListComparisonResults<T> results = new ListComparisonResults<T>();
            GenericComparer<T> ucomp = new GenericComparer<T>();
            results.Current = b;
            results.Union = b.Union(a, ucomp).ToList<T>();
            results.Intersect = b.Intersect(a, ucomp).ToList<T>();
            results.Additions = b.Except(a, ucomp).ToList<T>();
            results.Deletions = a.Except(b, ucomp).ToList<T>();
            return results;
        }
        public static bool AreEqual<T>(List<T> a, List<T> b)
        {
            bool results = true;
            GenericComparer<T> ucomp = new GenericComparer<T>();
            results = !(b.Except(a, ucomp).ToList<T>().Count > 0 || !results);
            results = !(a.Except(b, ucomp).ToList<T>().Count > 0 || !results);

            return results;
        }
        public static bool DictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            var comparer = EqualityComparer<TValue>.Default;

            foreach (KeyValuePair<TKey, TValue> kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!comparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }


}
