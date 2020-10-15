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
			var client = new MpfClient();
			using (await client.Connect()) {
				Console.WriteLine("Known Coils: ");
				foreach (var coil in await client.KnownCoils())
				{
					Console.WriteLine("  - " + coil);
				}
			}
		}
	}
}
