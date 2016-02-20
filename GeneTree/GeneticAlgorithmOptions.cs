﻿using System;
using System.Linq;
namespace GeneTree
{
	public class GeneticAlgorithmOptions
	{
		public double eval_percentClass_min = 0.1;

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

		public double eval_class_power = 0.1;

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

		public double eval_coverage_power = 0.1;

		public double Eval_coverage_power
		{
			get
			{
				return eval_coverage_power;
			}
			set
			{
				eval_coverage_power = value;
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
		
		public double prob_population_to_keep = 0.03;
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

		public double prob_ops_change = 0.9;
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

		public double prob_ops_delete = 0.4;
		public double Prob_ops_delete
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

		public double prob_ops_swap = 0.2;
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
		
		public double prob_to_keep_data = 0.10;
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

		public int max_node_count_for_new_tree = 40;
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

		public int seq_inner_population = 1000;
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

		public int seq_inner_generations = 30;
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

		public int seq_middle_generations = 50;
		public int Seq_middle_generations
		{
			get
			{
				return seq_middle_generations;
			}
			set
			{
				seq_middle_generations = value;
			}
		}

		public int seq_outer_generations = 100;
		public int Seq_outer_generations
		{
			get
			{
				return seq_outer_generations;
			}
			set
			{
				seq_outer_generations = value;
			}
		}

		public int seq_inner_run = 10;
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

		public int seq_outer_run = 1;
		public int Seq_outer_run
		{
			get
			{
				return seq_outer_run;
			}
			set
			{
				seq_outer_run = value;
			}
		}
	}
}

