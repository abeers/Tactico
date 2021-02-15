using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
      SceneManager.LoadScene(0);
    }
    public void LoadNextScene()
    {
      int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
