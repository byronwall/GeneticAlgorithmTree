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
		public int _count = 0;
		public int _columnsWithData;

		public ConfusionMatrix(int size)
		{
			this._size = size;
			_values = new int[size, size];
		}
		
		public void AddItem(double row, double column)
		{
			AddItem((int)row, (int)column);
		}
		
		public void AddItem(int row, int column)
		{
			_values[row, column]++;
			_count++;
		}
		
		public double GetRowWithMaxCount()
		{
			
			double max = double.MinValue;
			int max_index = 0;
			
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					if (_values[i, j] > max)
					{
						max_index = i;
						max = _values[i, j];
					}
				}
			}
			
			return max_index;
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
			
			return sb.ToString();
		}
		
		public IEnumerable<Tuple<int,double>> GetRowsOrderedByCount()
		{
			
			//will be row, value
			List<Tuple<int, double>> values = new List<Tuple<int, double>>();
			
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					if (_values[i, j] > 0)
					{
						values.Add(Tuple.Create(i, (double)_values[i, j]));
					}
				}
			}
			
			//loop through tuples and order rows by max value
			return values;
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
				
				if (colTotal > 0)
				{
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
			
			return (obs_acc - exp_acc) / (1 - exp_acc);
		}
		public double[] GetClassProbabilities()
		{
			//this assumes that the matrix is in a form to make predictions
			
			double[] totals = new double[_size];
			
			
			//sum the column totals
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					totals[j] += _values[i, j];
				}
			}
			
			//normalize by count
			
			for (int i = 0; i < _size; i++)
			{
				if (_count > 0)
				{
					
					totals[i] /= _count;
				}
				else
				{
					totals[i] = 1.0 / _size;
				}
			}
			
			return totals;
		}
		public double Sensitivity
		{
			get
			{
				return GetObservedAccuracy();
			}
		}
		public double Specificity
		{
			get
			{
				//TODO need some check to fail this on a non-binary option
				return 1.0 * _values[1, 1] / _count; 
			}
		}
		public double PositivePredictiveValue
		{
			get
			{
				return 1.0 * (_values[0, 0] + 0.5) / (_values[0, 0] + _values[1, 0] + 0.5);
			}
		}
		public double NegativePredictiveValue
		{
			get
			{
				return 1.0 * _values[1, 1] / (_values[1, 0] + _values[1, 1]);
			}
		}
		public double DiagnosticOddsRatio
		{
			get
			{
				return (_values[0, 0] + 0.5) / (_values[1, 0] + 0.5) * (_values[1, 1] + 0.5) / (_values[0, 1] + 0.5);
				//0+0.5 / 2705+0.5 * 8733+0.5 / 0.5
			}
		}
		public double F1Score
		{
			get
			{
				return (2.0 * _values[0, 0] + 0.5) / (2.0 * _values[0, 0] + _values[0, 1] + _values[1, 0] + 0.5);
			}
		}
		public double GiniImpurity
		{
			get
			{
				if (_count == 0)
				{
					return 0.0;
				}
				
				//TODO generate a method for getting row and column sums
				
				//sum the rows for a probability
				double _runningGini = 1.0;
				for (int i = 0; i < _size; i++)
				{
					
					double rowSum = 0.0;
					for (int j = 0; j < _size; j++)
					{
						rowSum += _values[i, j];
					}
					
					_runningGini -= Math.Pow(rowSum / _count, 2);
				}
				
				return _runningGini;
			}
		}
		public double GiniImpuritySqrt
		{
			get
			{
				return Math.Sqrt(GiniImpurity);
			}
		}
		public double PositiveClassProbability
		{
			get
			{
				//this assumes a binary classification problem for now
				if (_count > 0)
				{
					return 1.0 * (_values[1, 0] + _values[1, 1]) / _count;
				}				
				
				return 0.5;
			}
		}
	}
}



