using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GeneTree
{
	public class ConfusionMatrix
	{
		public int[,] _values;
		public int _size;
		public int _count;

		public ConfusionMatrix(int size)
		{
			this._size = size;
			_values = new int[size, size];
		}
		
		public void AddItem(int row, int column)
		{
			_values[row, column]++;
			_count++;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					sb.Append(_values[i, j]);
					sb.Append("\t");
				}
				sb.Append("\r\n");
			}
			
			sb.AppendLine(string.Format("obs: {0} \t exp: {1}", GetObservedAccuracy(), GetExpectedAccuracy()));
			
			return sb.ToString();
		}
		
		public double GetObservedAccuracy()
		{
			int correct = 0;
			for (int i = 0; i < _size; i++)
			{
				correct += _values[i, i];
			}
			
			return 1.0 * correct / _count;
		}
		public double GetExpectedAccuracy()
		{
			int innerSum = 0;
			for (int i = 0; i < _size; i++)
			{
				int rowTotal = 0;
				int colTotal = 0;
				for (int j = 0; j < _size; j++)
				{
					rowTotal += _values[i, j];
					colTotal += _values[j, i];
				}
				
				innerSum += rowTotal * colTotal;
			}
			return 1.0 * innerSum / Math.Pow(_count, 2);
		}
		public double GetKappa()
		{
			double obs_acc = GetObservedAccuracy();
			double exp_acc = GetExpectedAccuracy();
			
			return (obs_acc - exp_acc) / (1 - exp_acc);
		}
	}
}



