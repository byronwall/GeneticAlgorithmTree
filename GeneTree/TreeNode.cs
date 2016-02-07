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
	public class TreeNode
	{
		public TreeTest Test;
		
		//TODO, make IsTerminal a different type of node instead of bool switch
		public bool IsTerminal;
        
		public double Classification;

		public override string ToString()
		{
			if (IsTerminal)
			{
				return string.Format("terminal to {0}", Classification.ToString());
			}
			else
			{
				return Test.ToString();
			}
		}
	}
}
