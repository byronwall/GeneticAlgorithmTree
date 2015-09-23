using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
    class DataPointManager
    {
        public string[] classes;
        public Dictionary<int, List<double>> ranges = new Dictionary<int, List<double>>();
        public List<DataPoint> dataPoints = new List<DataPoint>();
        public int paramCount;

        public void DetermineClasses()
        {
            classes = dataPoints.GroupBy(x => x.Classification).Select(x => x.Key).ToArray();
        }

        public void DetermineRanges()
        {
            int paramCount = dataPoints[0].Data.Length;
            ranges.Clear();

            for (int i = 0; i < paramCount; i++)
            {
                double max = dataPoints.Max(x => x.Data[i]);
                double min = dataPoints.Min(x => x.Data[i]);

                ranges.Add(i, new List<double> { min, max });
            }
        }
    }
}
