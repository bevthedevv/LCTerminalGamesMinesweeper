using BepInEx;
using LCTerminalGames;

namespace LethalCompanyTemplate
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            TerminalGames.games.Add(new TerminalGame(
                "Minesweeper",
                "Classic Minesweeper game",
                0.1f,
                Minesweeper.RunGame,
                Minesweeper.StartGame,
                Minesweeper.HandleKey
            ));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}