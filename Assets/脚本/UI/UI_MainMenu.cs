using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;

    private void Start()
    {
        if (SaveManager.instance == null || SaveManager.instance.HasSavedData() == false)
            continueButton.SetActive(false);
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.DeleteSavedData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
        Debug.Log("Exit game called at " + Time.time);

        if (SaveManager.instance != null)
        {
            try
            {
                SaveManager.instance.SaveGame();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Save failed during exit: " + e.Message);
            }
        }

        var editorType = System.Type.GetType("UnityEditor.EditorApplication, UnityEditor");
        if (editorType != null)
        {
            var prop = editorType.GetProperty("isPlaying");
            if (prop != null)
                prop.SetValue(null, false);
        }
        else
        {
            Application.Quit();
        }
    }

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
