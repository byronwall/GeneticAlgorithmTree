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
using System.Threading;
using System.Threading.Tasks;

namespace GeneTree
{
	public class GeneticAlgorithmManager
	{
		public GeneticAlgorithmOptions _gaOptions = new GeneticAlgorithmOptions();
		
		//TODO allow this to take a seed for reproducibiltiy later
		public Random rando = new Random();
		public DataPointManager dataPointMgr = new DataPointManager();
		
		public event EventHandler<EventArg<GeneticAlgorithmUpdateStatus>> ProgressUpdated;
		
		public void OnProgressUpdated(int progress)
		{
			var update = new GeneticAlgorithmUpdateStatus();
			update.progress = progress;
			update.status = Logger.GetStringAndFlush();
			
			var handler = ProgressUpdated;
			if (handler != null)
			{
				handler(this, new EventArg<GeneticAlgorithmUpdateStatus>(update));
			}
		}
		
		private List<Tree> CreateRandomPoolOfTrees(int size)
		{
			var trees = new List<Tree>();
			
			Parallel.For(0, size, index =>
				{
					var tree = GeneticOperations.CreateRandomTree(this);
					
					lock (trees)
					{
						trees.Add(tree);
					}
				});
			return trees;
		}

		public void CreatePoolOfGoodTrees()
		{
			long ticks = DateTime.Now.Ticks;
			string dirPath = @"tree outputs\" + ticks;
			
			_gaOptions.populationSize = _gaOptions.seq_inner_population;			
			_gaOptions.generations = _gaOptions.seq_inner_generations;
			
			for (int run = 0; run < _gaOptions.seq_inner_run; run++)
			{
				Logger.WriteLine("starting run at run {0}", run);
				
				dataPointMgr.UpdateSubsetOfDatapoints(_gaOptions.prob_to_keep_data, rando);
					
				List<Tree> trees = ProcessTheNextGeneration();
				
				if (!Directory.Exists(dirPath))
				{
					Directory.CreateDirectory(dirPath);
				}
				
				var bestTree = trees[0];
				bestTree.WriteToXmlFile(Path.Combine(dirPath, 
						string.Format("{0} {1:0.0000}.xml", run, bestTree._prevResults.AverageLoss)));
			}
		}
		
		private List<Tree> ScoreTreesAndReturnKept(IEnumerable<Tree> trees, int generation)
		{
			List<Tree> keeps = new List<Tree>();
			
			Logger.WriteLine("point count: " + dataPointMgr._pointsToTest.Count);
			
			foreach (var tree in trees)
			{
				GeneticAlgorithmRunResults results = null;
				
				//TODO figure out why a NullRefExc came through here
				if (tree == null)
				{
					continue;
				}
				
				//quick pass through for trees already processed
				if (!tree._isDirty)
				{
					tree._source = "pass through";
					keeps.Add(tree);
					continue;
				}
				
				results = new GeneticAlgorithmRunResults(this);
				tree.ProcessDataThroughTree(dataPointMgr, results, dataPointMgr._pointsToTest);
				
				tree.RemoveZeroCountNodes();
				GeneticOperations.PruneTreeOfUselessNodes(tree);
				
				//will add kepeers if there was no previous result or if the score improved or randomly
				if (tree._prevResults == null || results.MetricResult > tree._prevResults.MetricResult)
				{
					//only add the new tree to the results if the score improved
					keeps.Add(tree);
				}
				
				tree._isDirty = false;
				tree._prevResults = results;
				
				//now run a set of values through for the cross validation
				//TODO determine when to do the CV step, how many points to use, and what to do with the results
				
				/*TODO uncomment to get CV back, consider impact of node level matrices changing)
				var cv_results = new GeneticAlgorithmRunResults(this);
				tree.ProcessDataThroughTree(dataPointMgr, cv_results, dataPointMgr._pointsNotUsedToTest.TakeEvery(5));				
				double loss_ratio = results.AverageLoss / cv_results.AverageLoss;				
				Logger.WriteLine(loss_ratio);
				*/
			}
			
			return keeps;
		}
		
		public List<Tree> ProcessTheNextGeneration()
		{
			//TODO make this a parameter
			
			var starter = CreateRandomPoolOfTrees(_gaOptions.populationSize * 1);
			
			return ProcessTheNextGeneration(starter);
		}

		public List<Tree> ProcessTheNextGeneration(List<Tree> treesInPopulation)
		{
			//do an initial scoring
			treesInPopulation = ScoreTreesAndReturnKept(treesInPopulation, 0);
			
			for (int generationNumber = 0; generationNumber < _gaOptions.generations; generationNumber++)
			{
				var newTreesThisGen = new List<Tree>();
				
				Logger.WriteLine("generation: " + generationNumber);
				
				//this allows probs to change after each generation if desired
				var operations = new List<Tuple<GeneticOperations.GeneticOperation, double>>();
					
				operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.SwapNodesBetweenTrees, _gaOptions.Prob_ops_swap));
				operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.SplitNodeAndOptimizeTests, _gaOptions.prob_node_split));
				operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.CreateRandomTree, _gaOptions.Prob_ops_new_tree));
				operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.OptimizeSplitForNode, 10.0));
				//operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.OptimizeClassesForTree, 10.0));
				operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.DeleteNodeFromTree, _gaOptions.prob_ops_delete));
				//operations.Add(Tuple.Create((GeneticOperations.GeneticOperation)GeneticOperations.SuperSplit, 5.0));
					
				var operation_picker = new WeightedSelector<GeneticOperations.GeneticOperation>(operations);
				
				Parallel.For(0, _gaOptions.PopulationSize, index =>
					{						
						var operation = operation_picker.PickRandom(rando);
						var newTrees = operation(this, treesInPopulation).ToList();
						
						//TODO determine if this is needed
						lock (newTreesThisGen)
						{
							newTreesThisGen.AddRange(newTrees);
						}
					});
				
				treesInPopulation.AddRange(newTreesThisGen);
				
				treesInPopulation = ScoreTreesAndReturnKept(treesInPopulation, generationNumber);
				
				foreach (var element in treesInPopulation.GroupBy(c=>c._source))
				{
					Logger.WriteLine(string.Format("{0} has {1} = {2:0.000}", 
							element.Key, element.Count(), 
							1.0 * element.Count() / treesInPopulation.Count));
				}
				
				//thin down the herd and take pop size or total
				treesInPopulation = treesInPopulation.Distinct()
					.OrderByDescending(c => c._prevResults.MetricResult)
					.Take((int)(_gaOptions.populationSize * _gaOptions.prob_population_to_keep))
					.ToList();
				
				//output some info on best
				//Logger.WriteLine(string.Join("\r\n", starter.Take(10).Select(c => c._prevResults.ToString())));
				Logger.WriteLine("");
				foreach (var tree in treesInPopulation.Take(10))
				{
					Logger.WriteLine("{0:0.0000} ({2:0.0000}, {3}, {1})", 
						tree._prevResults.MetricResult, 
						tree._source, tree._prevResults.AverageLoss,
						tree._prevResults.tree_nodeCount);
				}
				
				foreach (var tree in treesInPopulation.Take(1))
				{
					Logger.WriteLine("");
					Logger.WriteLine(tree);
					Logger.WriteLine(tree._prevResults);					
				}
				
				OnProgressUpdated(100 * generationNumber / Math.Max(_gaOptions.generations, 1));
			}

			OnProgressUpdated(100);
			
			return treesInPopulation;
		}

		public void LoadDataFile(string csv_path, string config_path)
		{
			dataPointMgr.SetConfiguration(config_path);
			dataPointMgr.LoadFromCsv(csv_path);
		}
		
		public void DoSomePrediction(string path)
		{
			//TODO this method is poorly named and a hack
			PredictionManager predMgr = new PredictionManager(this);
			
			predMgr.LoadTreeAndGenerateResults(path);
			
			OnProgressUpdated(100);
		}
		
		public void DoAllPredictions(string path)
		{
			//TODO this method is poorly named and a hack
			PredictionManager predMgr = new PredictionManager(this);
			
			predMgr.GeneratePredictionsForDataWithAllTrees(path);
			
			OnProgressUpdated(100);
		}
	}
}