using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
	private void Start()
	{
		if (PlayerPrefs.HasKey("OptionsSet"))
		{
			slider.value = PlayerPrefs.GetFloat("MouseSensitivity");
			if (PlayerPrefs.GetInt("Rumble") == 1)
			{
				rumble.isOn = true;
			}
			else
			{
				rumble.isOn = false;
			}
			if (PlayerPrefs.GetInt("AnalogMove") == 1)
			{
				analog.isOn = true;
			}
			else
			{
				analog.isOn = false;
			}
		}
		else
		{
			PlayerPrefs.SetInt("OptionsSet", 1);
		}
	}
	private void Update()
	{
		PlayerPrefs.SetFloat("MouseSensitivity", slider.value);
		PlayerPrefs.SetInt("Rumble", rumble.isOn ? 1 : 0);
		PlayerPrefs.SetInt("AnalogMove", analog.isOn ? 1 : 0);
		PlayerPrefs.SetInt("SM64Music", sm64music.isOn ? 1 : 0);
	}
	public Slider slider;
	public Toggle rumble;
	public Toggle analog;
	public Toggle sm64music;
}
