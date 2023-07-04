using Eto.Drawing;
using Eto.Forms;
using Microsoft.Extensions.Configuration;
using ObjectiveLearn.Shared;
using System;
using TankLite;

namespace ObjectiveLearn
{
    internal class Program
	{
		[STAThread]
		static void Main()
		{
            var builder = new ConfigurationBuilder()
                            .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
			
			ConfigManager.Init(config);

            new Application(Eto.Platform.Detect).Run(new MainForm());
		}
	}
}
