using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib.Extensions;
using Microsoft.Win32;

namespace KLib
{
	class Program
	{
		static void Main(string[] args)
		{
			Import();
		}

		static void Export()
		{
			string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey)) {
				var q = from a in key.GetSubKeyNames()
								from r in key.OpenSubKey(a).Use()
								let d = r.GetValue("DisplayName")
								where d != null
								select d.ToString();

				foreach (var s in q.WithIndex()) {
					Console.WriteLine("{0}: {1}",s.Item1,s.Item2);
				}
			}

		}

		static void Import()
		{

			var q = from l in Console.In.FromReader()
							let s = l.Trim()
							where !string.IsNullOrEmpty(s) && s[0] != '#'
							select s;

			foreach (var s in q.WithIndex()) {
				Console.WriteLine("{0}: {1}", s.Item1, s.Item2);
			}
		}
	}
}
