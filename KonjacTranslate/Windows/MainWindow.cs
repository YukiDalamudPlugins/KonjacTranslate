using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace KonjacTranslate.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin { get; }

    public MainWindow(Plugin plugin) : base(
        "Konjac Translate", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose()
    {

    }

    public override void Draw()
    {
        bool saveConfig = false;

        var enable = plugin.Configuration.Enable;
        if (ImGui.Checkbox("Enable", ref enable))
        {
            plugin.Configuration.Enable = enable;
            saveConfig = true;
        }
        ImGui.Spacing();
        var authKey = plugin.Configuration.DeepLAuthKey;
        if (ImGui.InputText("DeepL Auth Key", ref authKey, 64, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            plugin.Configuration.DeepLAuthKey = authKey;
            plugin.ChatHandler.UpdateTranslator();
            saveConfig = true;
        }
        ImGui.Spacing();
        var minLength = plugin.Configuration.MinLength;
        if (ImGui.InputInt("Sentence Min length", ref minLength))
        {
            plugin.Configuration.MinLength = Math.Max(minLength, 1);
            saveConfig = true;
        }
        var maxLength = plugin.Configuration.MaxLength;
        if (ImGui.InputInt("Sentence Max length", ref maxLength))
        {
            plugin.Configuration.MaxLength = Math.Min(maxLength, 32);
            saveConfig = true;
        }
        if (saveConfig) plugin.Configuration.Save();
    }
}
