using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualPinball.Engine.Mpf;

namespace MpfTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var mpfApi = new MpfApi(@"C:\Development\VisualPinball.Engine.Mpf\VisualPinball.Engine.Mpf\machine");

			await mpfApi.Launch();

			await mpfApi.Start(new Dictionary<string, bool> {
				{"sw_11", false},
			});

			var descr = await mpfApi.GetMachineDescription();
			Console.WriteLine($"Description: {descr}");
		}
	}
}
