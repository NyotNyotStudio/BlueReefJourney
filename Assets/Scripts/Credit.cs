using UnityEngine;
using UnityEngine.SceneManagement;

public class Credit : MonoBehaviour
{
    [Header("Credit Text")]
    public RectTransform creditText;
    public float scrollSpeed = 50f;

    [Header("Back Button")]
    public string mainMenuScene = "MainMenu";

    void Update()
    {
      
        creditText.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }


    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
