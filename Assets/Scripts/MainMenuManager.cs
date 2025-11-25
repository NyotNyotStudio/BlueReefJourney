using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StartGame()
    {

        SceneManager.LoadScene("Scene");
    }

    public void CreditScene()
    {

        SceneManager.LoadScene("Credit");
    }


    public void QuitGame()
    {

        Application.Quit();
        Debug.Log("Game Quit");

    }
}
