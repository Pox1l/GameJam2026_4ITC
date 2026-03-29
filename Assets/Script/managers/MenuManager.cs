using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;
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

    public void SetVolume(float volume)
    {
        mixer.SetFloat("Effects", volumeSlider.value);
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