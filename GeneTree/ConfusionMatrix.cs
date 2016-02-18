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
		public int _columnsWithData;

		public ConfusionMatrix(int size)
		{
			//TODO allow this matrix to detect when no classifications were made for an entire class
			this._size = size;
			_values = new int[size, size];
		}
		
		public void AddItem(int row, int column)
		{
			_isDirty = true;
			
			_values[row, column]++;
			_count++;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					
					sb.Append(string.Format("{0,6}", _values[i, j]));
					
					if (i == j)
					{
						sb.Append("*");
					}
					
					sb.Append("\t");
				}
				sb.Append("\r\n");
			}
			
			sb.AppendLine(string.Format("obs: {0} \t exp: {1}", GetObservedAccuracy(), GetExpectedAccuracy()));
			sb.AppendLine(string.Format("columns with data = {0}", _columnsWithData));
			
			return sb.ToString();
		}
		
		private double _obsAccuracy;
		private double _expAccuracy;
		private double _kappa;
		
		private bool _isDirty = true;
		
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
			_columnsWithData = 0;
			
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
				
				if(colTotal > 0){
					_columnsWithData++;
				}
				
				innerSum += rowTotal * colTotal;
			}
			return 1.0 * innerSum / Math.Pow(_count, 2);
		}
		public double GetKappa()
		{
			double obs_acc = GetObservedAccuracy();
			double exp_acc = GetExpectedAccuracy();
			
			//TODO implement dirty with a method to ProcessCalcs for the matrix
			_isDirty = false;
			
			return (obs_acc - exp_acc) / (1 - exp_acc);
		}
	}
}



