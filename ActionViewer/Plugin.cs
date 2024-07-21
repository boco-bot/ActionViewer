﻿using ActionViewer.Windows;
using ActionViewer.Windows.Config;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using System.Reflection;

namespace ActionViewer
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "ActionViewer";

        private const string commandName = "/av";
        private const string configCommandName = "/avcfg";

        [PluginService] public static ITargetManager TargetManager { get; private set; } = null!;

        [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
        [PluginService] public static IDataManager DataManager { get; private set; } = null;

        public readonly WindowSystem WindowSystem = new("ActionViewer");
        public readonly MainWindow MainWindow;
        public readonly ConfigWindow ConfigWindow;
        public Configuration Configuration { get; init; }
        public IActionViewer ActionViewer { get; init; }
        public const string Authors = "boco-bot, ClassicRagu";
        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
		public readonly ExcelSheet<Lumina.Excel.GeneratedSheets2.Action> ActionSheet;

		public Plugin(IDalamudPluginInterface pluginInterface)
        {
            Services.Initialize(pluginInterface);

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(Services.PluginInterface);

            this.MainWindow = new MainWindow(this);
            this.ConfigWindow = new ConfigWindow(this);

			this.WindowSystem.AddWindow(this.MainWindow);
            this.WindowSystem.AddWindow(this.ConfigWindow);

            PluginInterface.UiBuilder.Draw += this.DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += this.DrawConfigUI;

            this.ActionSheet = DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Action>();

			ActionViewer = new ActionViewer();

            // you might normally want to embed resources and load them from the manifest stream
            //PluginUi = new PluginUI(Configuration, ActionViewer);

            Services.Commands.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "View a list of the Essence and Lost Actions of nearby players"
            });
            Services.Commands.AddHandler(configCommandName, new CommandInfo(OnConfigCommand)
            {
                HelpMessage = "Open the config for Action Viewer"
            });

            //Services.PluginInterface.UiBuilder.Draw += DrawUI;
            //Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            Services.Commands.RemoveHandler(commandName);
        }

        private void OnCommand(string command, string args)
        {
            this.MainWindow.IsOpen ^= true;
        }

        private void OnConfigCommand(string command, string args)
        {
            this.ConfigWindow.IsOpen ^= true;
        }

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            this.ConfigWindow.IsOpen = true;
        }

        //private void DrawConfigUI()
        //{
        //    this.PluginUi.SettingsVisible = true;
        //}
    }
}
