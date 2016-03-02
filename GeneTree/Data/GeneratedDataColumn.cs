using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace GeneTree
{
	public class GeneratedDataColumn : DoubleDataColumn
	{
		//this will work to create linear functions of doubles first
		public double _scaling = 1.0;

		public DoubleDataColumn _baseColumn;

		public FormulaOptions _formula;

		public enum FormulaOptions
		{
			LN,
			TANH,
			SQRT,
			SQR,
			INV,
			NONE,
			NORM_01,
			NORM_STDEV
		}

		public override string ToString()
		{
			return string.Format("{0:0.00}*{2}({1})", _scaling, _baseColumn, _formula);
		}

		public void CreateValues(DataPointManager dp_mgr)
		{
			foreach (var baseValue in _baseColumn._values)
			{
				DataValue dv_new = new DataValue();
				switch (this._formula)
				{
					case FormulaOptions.LN:
						if (baseValue._value > 0)
						{
							dv_new._value = Math.Log(baseValue._value);
						}
						else
						{
							dv_new._isMissing = true;
							this._hasMissingValues = true;
						}
						break;
					case FormulaOptions.TANH:
						dv_new._value = Math.Tanh(baseValue._value);
						break;
					case FormulaOptions.SQRT:
						if (baseValue._value > 0)
						{
							dv_new._value = Math.Sqrt(baseValue._value);
						}
						else
						{
							dv_new._isMissing = true;
							this._hasMissingValues = true;
						}
						break;
					case FormulaOptions.SQR:
						dv_new._value = Math.Pow(baseValue._value, 2);
						break;
					case FormulaOptions.INV:
						if (baseValue._value != 0.0)
						{
							dv_new._value = 1 / baseValue._value;
						}
						else
						{
							dv_new._isMissing = true;
							this._hasMissingValues = true;
						}
						break;
					case FormulaOptions.NONE:
						dv_new._value = baseValue._value;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				dv_new._value *= _scaling;
				this._values.Add(dv_new);
			}
			//do a zip with the original DataPoints
			var items = dp_mgr._dataPoints.Zip(this._values, (dp, dv) =>
				{
					dp._data.Add(dv);
					return true;
				});
			//TODO remove this sloppy bit that forces execution above
			foreach (var item in items)
			{
			}
			this.ProcessRanges();
		}

		public static GeneratedDataColumn CreateNewRandom(GeneticAlgorithmManager ga_mgr)
		{
			//do some work to create the new column
			var column = new GeneratedDataColumn();
			column._scaling = ga_mgr.rando.NextDouble() * 20 - 10.0;
			//this allows probs to change after each generation if desired
			var operations = new List<Tuple<GeneratedDataColumn.FormulaOptions, double>>();
			
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.INV, 10.0));
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.LN, 10.0));
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.SQR, 10.0));
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.SQRT, 10.0));
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.TANH, 10.0));
			operations.Add(Tuple.Create(GeneratedDataColumn.FormulaOptions.NONE, 10.0));
			var operation_picker = WeightedSelector.Create(operations);
			column._formula = operation_picker.PickRandom(ga_mgr.rando);
			//forces to be a double data column here
			DataColumn baseColumn = null;
			do
			{
				int index = ga_mgr.rando.Next(ga_mgr.dataPointMgr._columns.Count);
				baseColumn = ga_mgr.dataPointMgr._columns[index];
			}
			while (baseColumn._type != DataValueTypes.NUMBER);
			
			column._baseColumn = baseColumn as DoubleDataColumn;
			column._type = DataValueTypes.NUMBER;
			
			column.CreateValues(ga_mgr.dataPointMgr);
			
			return column;
		}
	}
}




