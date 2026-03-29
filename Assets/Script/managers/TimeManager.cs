using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("Reference")]
    public TextMeshProUGUI timerText;

    [Header("Nastavení")]
    public float soulGainInterval = 10f;

    private float elapsedTime;
    private float intervalCounter;

    void Start()
    {
        if (timerText == null)
        {
            GameObject timerObj = GameObject.FindWithTag("Timer");
            if (timerObj != null) timerText = timerObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Stopky
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Interval pro pasivní duše
        intervalCounter += Time.deltaTime;

        if (intervalCounter >= soulGainInterval)
        {
            intervalCounter = 0; // Reset poèítadla

            if (SoulManager.Instance != null)
            {
                SoulManager.Instance.AddPassiveSouls();
                Debug.Log("<color=green>TimeManager:</color> Interval vypršel, posílám duše!");
            }
            else
            {
                Debug.LogWarning("<color=red>TimeManager:</color> Nemùžu najít SoulManager.Instance!");
            }
        }
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
        intervalCounter = 0f;
        if (timerText != null) timerText.text = "00:00";
    }
}