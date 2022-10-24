using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    [SerializeField] int loadingID = 3;

    public static SceneControl instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public void LoadScene(int id)
    {
        SceneManager.LoadScene(loadingID);
        
        StartCoroutine(LoadScene_Enum(id));
    }
    IEnumerator LoadScene_Enum(int id)
    {
        yield return new WaitForSecondsRealtime(2f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
