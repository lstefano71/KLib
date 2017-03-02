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
		private const string vName = "UserListIndex";
		private const string registryRoot = @"Software\Native Instruments";
		private const string contentDir = "ContentDir";

		static void Main(string[] args)
		{
			if (args.Length == 0) {
				Help();
				return;
			}
			if (args[0] == "import")
				Import();
			else
				Export();
		}

		private static void Help()
		{
			var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var name = System.IO.Path.GetFileName(location);
			Console.WriteLine("Usage:\n{0} import < nomefile\n{0} export > nomefile", name);
		}

		static void Export()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(registryRoot))
			using (var lk = Registry.LocalMachine.OpenSubKey(registryRoot)) {
				var q = from a in key.GetSubKeyNames()
								from r in key.OpenSubKey(a).Use()
								from r1 in lk.OpenSubKey(a).Use()
								let d = r.GetValue(vName)
								where d != null
									&& r.GetValueKind(vName) == RegistryValueKind.DWord
								let n = (int)d
								orderby n ascending
								select new {
									Index = n,
									Name = a,
									Path = r1!= null ? r1.GetValue(contentDir) as string : null
								};

				foreach (var s in q.WithIndex()) {
					Console.WriteLine($"# {s.Item2.Index:D3}-{s.Item2.Name}");
					Console.WriteLine($"{s.Item2.Name} # {s.Item2.Path ?? ""}");
				}
			}

		}

		static void Import()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(registryRoot)) {
				var q = from l in Console.In.FromReader()
								let s = Parse(l.Item2)
								where !string.IsNullOrEmpty(s)
								from r in key.OpenSubKey(s, true).Use()
								select new {
									Name = s,
									Found = r != null,
									Line = l.Item1
								};

				var list = q.ToList();

				foreach (var s in list.Where(x => x.Found).WithIndex()) {
					using (var r = key.OpenSubKey(s.Item2.Name, true)) {
						Console.WriteLine($"{s.Item1:D3}-{s.Item2.Name}\t\t\t\t# line: {s.Item2.Line}");
						r.SetValue(vName, s.Item1);
					}
				}

				foreach (var s in list.Where(x => !x.Found)) {
					Console.WriteLine($"Ignored: Line {s.Line}, \"{s.Name}\"");
				}
			}
		}

		private static string Parse(string s)
		{
			var i = s.IndexOf("#");
			return s.Substring(0, i >= 0 ? i : s.Length).Trim();
		}
	}
}
