using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("Reference (Pøiøadí se automaticky)")]
    public TextMeshProUGUI timerText;

    public float soulGainInterval = 10f;

    private float elapsedTime;
    private float intervalCounter;

    void Start()
    {
        // Najde objekt podle Tagu
        if (timerText == null)
        {
            GameObject timerObj = GameObject.FindWithTag("Timer");
            if (timerObj != null) timerText = timerObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        intervalCounter += Time.deltaTime;
        if (intervalCounter >= soulGainInterval)
        {
            intervalCounter -= soulGainInterval;
            if (SoulManager.Instance != null) SoulManager.Instance.AddPassiveSouls();
        }
    }
}