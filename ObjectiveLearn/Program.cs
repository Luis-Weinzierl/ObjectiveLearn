using Eto.Forms;
using Microsoft.Extensions.Configuration;
using ObjectiveLearn.Shared;
using ObjectiveLearn.Models;
using System;
using Shared.Localisation;

namespace ObjectiveLearn
{
    internal class Program
	{
		[STAThread]
		static void Main()
		{
            var configBuilder = new ConfigurationBuilder()
                            .AddJsonFile($"appsettings.json", true, true);

            var config = configBuilder.Build();
			
			ConfigManager.Init(config);

            var languageBuilder = new ConfigurationBuilder()
                            .AddJsonFile($"{ConfigManager.GetConfig(Config.Language)}.json", true, true);

            var language = languageBuilder.Build();

            LanguageManager.Init(language);
            TLName.Init();

            new Application(Eto.Platform.Detect).Run(new MainForm());
		}
	}
}
