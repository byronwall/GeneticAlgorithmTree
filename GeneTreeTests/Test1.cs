/*
 * Created by SharpDevelop.
 * User: byronandanne
 * Date: 2/4/2016
 * Time: 5:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using GeneTree;


namespace GeneTreeTests
{
	[TestFixture]
	public class Test1
	{
		[Test]
		public void TestMethod()
		{
			ConfusionMatrix cm = new ConfusionMatrix(2);
			
			cm._values[0, 0] = 20;
			cm._values[0, 1] = 5;
			cm._values[1, 0] = 10;
			cm._values[1, 1] = 15;
			
			cm._count = 50;
			
			string test = cm.ToString();
		}
		
		[Test]
		public void WeightedSel()
		{
			List<int> choices = new List<int>{ 1, 2, 3, 4 };
			
			var choice_chooser = new WeightedSelector<int>(choices.Select(c => Tuple.Create(c, c * 1.0)));
			
			Random rando = new Random();
			
			List<int> outputs = new List<int>();
			
			for (int i = 0; i < 10000; i++) {
				outputs.Add(choice_chooser.PickRandom(rando));
			}
			
			var groups = string.Join("|", outputs.GroupBy(c => c).Select((c, d) => c.Key + " " + c.Count()));
		}
		
		[Test]
		public void GiniTest()
		{
			ConfusionMatrix test = new ConfusionMatrix(2);
			test.AddItem(0, 0);
			test.AddItem(0, 0);
			test.AddItem(0, 0);
			test.AddItem(0, 0);
			
			test.AddItem(1, 0);
			test.AddItem(1, 0);
			test.AddItem(1, 1);
			test.AddItem(1, 0);
			test.AddItem(1, 0);
			
			
			Assert.AreEqual(test.GiniImpurity, 40.0 / 81.0);
		}
	}
}
