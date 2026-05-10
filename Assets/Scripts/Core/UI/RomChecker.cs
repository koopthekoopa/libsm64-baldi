using UnityEngine;
using UnityEngine.UI;
using LibSM64;
using TMPro;

public class RomChecker : MonoBehaviour
{
    public Button startButton;

    public void Start()
    {
        SM64Context.OpenRom();
    }

    public void Update()
    {
        GetComponent<TMP_Text>().enabled = !SM64Context.CheckRom();
        startButton.enabled = SM64Context.CheckRom();
    }
}