using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Zmìnìno ze string na int pro naèítání podle ID
    IEnumerator LoadLevelAsync(int sceneId)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneId);

        // Zabrání pokraèování, dokud není scéna plnì naètena
        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    // Tuto metodu teï mùžeš napojit na tlaèítko a pøedat jí èíslo scény
    public void LoadLevelById(int sceneId)
    {
        StartCoroutine(LoadLevelAsync(sceneId));
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Hra se vypíná."); // Pomùcka pro testování v Editoru, kde Quit() nic nedìlá
    }
}