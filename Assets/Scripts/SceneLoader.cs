using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextScene = "Credit";
    public float delay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadAfterDelay());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextScene);
    }
}
