using Eto.Forms;
using Microsoft.Extensions.Configuration;
using ObjectiveLearn.Shared;
using ObjectiveLearn.Models;
using System;
using Shared.Localization;

namespace ObjectiveLearn;

public class Program
{
	[STAThread]
	private static void Main()
	{
        var configBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true);

        var config = configBuilder.Build();
		
		ConfigManager.Init(config);

        var languageBuilder = new ConfigurationBuilder()
                        .AddJsonFile($"{ConfigManager.Get(Config.Language)}.json", true, true);

        var language = languageBuilder.Build();

        LanguageManager.Init(language);
        TlName.Init();

        new Application(Eto.Platform.Detect).Run(new MainForm());
	}
}
