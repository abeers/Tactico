using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Dropdown gameModeDropdown;
    [SerializeField] Dropdown playerDropdown;

    public void Start()
    {
      gameModeDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("gameMode");
      playerDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("playerMode");
    }

    public void SetGameMode()
    {
      PlayerPrefs.SetInt("gameMode", gameModeDropdown.GetComponent<Dropdown>().value);
    }

    public void SetPlayerMode()
    {
      PlayerPrefs.SetInt("playerMode", playerDropdown.GetComponent<Dropdown>().value);
    }
}
