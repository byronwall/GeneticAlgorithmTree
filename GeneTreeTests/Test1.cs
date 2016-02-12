/*
 * Created by SharpDevelop.
 * User: byronandanne
 * Date: 2/4/2016
 * Time: 5:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using GeneTree;
using NUnit.Framework;

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
	}
}
