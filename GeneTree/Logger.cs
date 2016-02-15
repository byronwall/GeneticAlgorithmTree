using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GeneTree
{
	public class Logger
	{
		StringBuilder sb = new StringBuilder();

		private static Logger _instance = new Logger();

		public static Logger GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Logger();
			}
			return _instance;
		}

		public static void WriteLine(string message)
		{
			_instance.sb.AppendLine(message);
		}
		
		public static void WriteLine(object message){
			WriteLine(message.ToString());
		}

		public static string GetStringAndFlush()
		{
			string output = _instance.sb.ToString();
			_instance.sb.Clear();
			return output;
		}
	}
}

