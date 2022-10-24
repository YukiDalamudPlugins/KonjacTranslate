using System;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using Dalamud.Utility;
using DeepL;

namespace KonjacTranslate.Chat
{
    public class ChatHandler : IDisposable
    {
        private Translator? translator { get; set; }

        private Plugin plugin { get; }
        public ChatHandler(Plugin plugin)
        {
            this.plugin = plugin;
            UpdateTranslator();
            this.plugin.ChatGui.ChatMessage += OnChatMessage;
        }

        public void Dispose()
        {
            plugin.ChatGui.ChatMessage -= OnChatMessage;
            if (translator != null)
                translator.Dispose();
        }

        private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (translator == null || !plugin.Configuration.Enable || !plugin.ClientState.IsLoggedIn || !plugin.ClientState.LocalPlayer) return;
            if (type == XivChatType.Party || type == XivChatType.CrossParty)
            {
                if (sender.TextValue.EndsWith(plugin.ClientState.LocalPlayer!.Name.TextValue)) return;
                if (IsFitTextLength(message.TextValue) && IsJapanese(message.TextValue))
                {
                    try
                    {
                        var task = translator.TranslateTextAsync(new[] { message.TextValue }, LanguageCode.Japanese, LanguageCode.Chinese);
                        task.Wait();
                        var translations = task.Result;
                        plugin.ChatGui.Print($"({sender.TextValue}) {translations[0].Text}");
                        PluginLog.Debug($"({sender.TextValue}) {translations[0].Text}");
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error(ex.ToString());
                    }
                }
            }
        }

        public void UpdateTranslator()
        {
            if (!plugin.Configuration.Enable || plugin.Configuration.DeepLAuthKey.Trim().Length == 0)
            {
                translator = null;
                return;
            }
            try
            {
                translator = new Translator(plugin.Configuration.DeepLAuthKey);
                plugin.ChatGui.Print($"[{plugin.Name}] translator ready!");
            }
            catch (Exception ex)
            {
                plugin.ChatGui.PrintError($"[{plugin.Name}] token is invalid!");
                PluginLog.Error(ex.ToString());
                translator = null;
            }
        }

        private bool IsFitTextLength(string text)
        {
            return text.Length <= plugin.Configuration.MaxLength && text.Length >= plugin.Configuration.MinLength;
        }
        public static bool IsJapanese(string text)
        {
            return text.Any(e => (e >= 0x3040 && e <= 0x309F) || (e >= 0x30A0 && e <= 0x30FF));
        }
    }
}
