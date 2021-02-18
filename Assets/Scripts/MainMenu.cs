using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Dropdown gameModeDropdown;
    [SerializeField] Dropdown gameLengthDropdown;
    [SerializeField] Dropdown playerModeDropdown;
    [SerializeField] InputField playerNameInput;
    [SerializeField] Dropdown playerColorDropdown;

    public void Start()
    {
      gameModeDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("gameMode");
      gameLengthDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("gameLength");
      playerModeDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("playerMode");
      GetPlayerSettings();
    }

    public void SetGameMode()
    {
      PlayerPrefs.SetInt("gameMode", gameModeDropdown.GetComponent<Dropdown>().value);
    }

    public void SetGameLength()
    {
      PlayerPrefs.SetInt("gameLength", gameLengthDropdown.GetComponent<Dropdown>().value);
    }

    public void SetPlayerMode()
    {
      PlayerPrefs.SetInt("playerMode", playerModeDropdown.GetComponent<Dropdown>().value);
    }

    public void SetPlayerSettings()
    {
      PlayerPrefs.SetString("playerName", playerNameInput.GetComponent<InputField>().text);
      PlayerPrefs.SetInt("playerColor", playerColorDropdown.GetComponent<Dropdown>().value);
    }

    public void GetPlayerSettings()
    {
      playerNameInput.GetComponent<InputField>().text = PlayerPrefs.GetString("playerName");
      playerColorDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("playerColor");
    }
}
