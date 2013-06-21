using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class ListComparisonResults<T>
    {
        public List<T> Current { get; set; }
        public List<T> Union { get; set; }
        public List<T> Intersect { get; set; }
        public List<T> Additions { get; set; }
        public List<T> Deletions { get; set; }
        public ListComparisonResults()
        {
            Current = new List<T>();
            Union = new List<T>();
            Intersect = new List<T>();
            Additions = new List<T>();
            Deletions = new List<T>();
        }
    }
}
