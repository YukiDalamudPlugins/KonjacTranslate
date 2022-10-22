using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using KonjacTranslate.Windows;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState;
using KonjacTranslate.Chat;

namespace KonjacTranslate
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Konjac Translate";
        private const string CommandName = "/kj";
        private Window MainWindow { get; init; }
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public ChatGui ChatGui { get; init; }
        public ClientState ClientState { get; set; }
        public Configuration Configuration { get; init; }

        public WindowSystem WindowSystem = new("KonjacTranslate");
        public ChatHandler ChatHandler { get; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] ChatGui chatGui,
            [RequiredVersion("1.0")] ClientState clientState)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ClientState = clientState;
            this.ChatGui = chatGui;


            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            this.ChatHandler = new ChatHandler(this);

            this.MainWindow = new MainWindow(this);
            this.WindowSystem.AddWindow(this.MainWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Konjac Translate configuration window"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;
        }

        public void Dispose()
        {
            this.ChatHandler.Dispose();
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            OpenConfigUi();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        private void OpenConfigUi()
        {
            MainWindow.IsOpen = true;
        }

    }
}
