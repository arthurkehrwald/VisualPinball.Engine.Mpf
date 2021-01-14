using MpfLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpfTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			using (var client = await new MpfClient().Connect()) {
				var coils = await client.KnownCoils();
				Console.WriteLine($"Known Coils ({coils.Count()}): ");
				foreach (var coil in coils) {
					Console.WriteLine("  - " + coil);
				}
			}
		}
	}
}
