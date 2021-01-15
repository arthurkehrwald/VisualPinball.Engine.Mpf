using System;
using System.Linq;
using System.Threading.Tasks;
using VisualPinball.Engine.Mpf;

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
