using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("BasementScene"); 
    }
    public void Exit()
    {
        Application.Quit();
    }
}
