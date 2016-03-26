using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MoreLinq;
namespace GeneTree
{
	public class WeightedSelector<T>
	{
		public List<Tuple<T, double>> _items = new List<Tuple<T, double>>();

		private double max_weight
		{
			get
			{
				return _items.Sum(c => c.Item2);
			}
		}

		public T PickRandom(Random rando)
		{
			double test_val = rando.NextDouble() * max_weight;
			double total = 0;
			
			if(_items.Count ==0){
				return default(T);
			}

			foreach (var item in _items)
			{
				total += item.Item2;
				if (test_val <= total)
				{
					return item.Item1;
				}
			}
			
			return _items[0].Item1;
		}

		public WeightedSelector(IEnumerable<Tuple<T, double>> items)
		{
			_items.AddRange(items);
			return;
			
			//TODO remove this extra stuff
			foreach (var item in items)
			{
				if (item.Item2 > 0)
				{
					_items.Add(item);
				}
			}
			
			if (_items.Count == 0)
			{
				throw new Exception("no items were added to picker since all were 0");
			}
		}
	}
	public static class WeightedSelector
	{
		public static WeightedSelector<T> Create<T>(IEnumerable<Tuple<T,double>> listItems)
		{
			return new WeightedSelector<T>(listItems);
		}
	}
}



