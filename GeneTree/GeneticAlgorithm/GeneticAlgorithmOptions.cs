using System;
using System.Linq;
namespace GeneTree
{
	public class GeneticAlgorithmOptions
	{
		public double eval_percentClass_min = 0.75;
		public double Eval_percentClass_min
		{
			get
			{
				return eval_percentClass_min;
			}
			set
			{
				eval_percentClass_min = value;
			}
		}

		public double eval_class_power = 0.10;
		public double Eval_class_power
		{
			get
			{
				return eval_class_power;
			}
			set
			{
				eval_class_power = value;
			}
		}

		public double eval_node_count = 0.0;
		public double Eval_Node_Count
		{
			get
			{
				return eval_node_count;
			}
			set
			{
				eval_node_count = value;
			}
		}
		
		public double test_value_change = 0.1;
		public double Test_value_change
		{
			get
			{
				return test_value_change;
			}
			set
			{
				test_value_change = value;
			}
		}
		
		public double prob_population_to_keep = 0.3;
		public double Prob_population_to_keep
		{
			get
			{
				return prob_population_to_keep;
			}
			set
			{
				prob_population_to_keep = value;
			}
		}
		
		public double prob_node_terminal = 0.5;
		public double Prob_node_terminal
		{
			get
			{
				return prob_node_terminal;
			}
			set
			{
				prob_node_terminal = value;
			}
		}

		public double prob_ops_change = 1;
		public double Prob_ops_change
		{
			get
			{
				return prob_ops_change;
			}
			set
			{
				prob_ops_change = value;
			}
		}
		
		public double prob_ops_new_tree = 10;
		public double Prob_ops_new_tree
		{
			get
			{
				return prob_ops_new_tree;
			}
			set
			{
				prob_ops_new_tree = value;
			}
		}

		public double prob_ops_delete = 10;
		public double Prob_Ops_Delete
		{
			get
			{
				return prob_ops_delete;
			}
			set
			{
				prob_ops_delete = value;
			}
		}

		public double prob_ops_swap = 20;
		public double Prob_ops_swap
		{
			get
			{
				return prob_ops_swap;
			}
			set
			{
				prob_ops_swap = value;
			}
		}
		
		public double prob_node_split = 20;
		public double Prob_node_split
		{
			get
			{
				return prob_node_split;
			}
			set
			{
				prob_node_split = value;
			}
		}
		
		public double prob_to_keep_data = 0.150;
		public double Prob_to_keep_data
		{
			get
			{
				return prob_to_keep_data;
			}
			set
			{
				prob_to_keep_data = value;
			}
		}
		
		public int generations = 10;
		public int Generations
		{
			get
			{
				return generations;
			}
			set
			{
				generations = value;
			}
		}

		public int max_node_count_for_new_tree = 10;
		public int Max_node_count_for_new_tree
		{
			get
			{
				return max_node_count_for_new_tree;
			}
			set
			{
				max_node_count_for_new_tree = value;
			}
		}

		public int populationSize = 200;
		public int PopulationSize
		{
			get
			{
				return populationSize;
			}
			set
			{
				populationSize = value;
			}
		}

		public int seq_inner_population = 120;
		public int Seq_inner_population
		{
			get
			{
				return seq_inner_population;
			}
			set
			{
				seq_inner_population = value;
			}
		}

		public int seq_inner_generations = 40;
		public int Seq_inner_generations
		{
			get
			{
				return seq_inner_generations;
			}
			set
			{
				seq_inner_generations = value;
			}
		}

		public int seq_inner_run = 200;
		public int Seq_inner_run
		{
			get
			{
				return seq_inner_run;
			}
			set
			{
				seq_inner_run = value;
			}
		}

	}
}

