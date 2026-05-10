using UnityEngine;
using LibSM64;
using SFB;

public class RomButton : MonoBehaviour
{
    public void LoadNewRom()
    {
        var extensions = new[] {
            new ExtensionFilter("Valid ROM files", "z64"),
            new ExtensionFilter("All Files", "*" ),
        };
        try
        {
            string tempPath = "";
            tempPath = StandaloneFileBrowser.OpenFilePanel("Open SM64 US ROM File", Application.dataPath + "/../", extensions, false)[0];
            SM64Context.SetNewRom(tempPath);
        }
        catch { Debug.Log("dialog cancel?"); }
    }
}