using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private int _maxHealth = 100;
    [SerializeField]
    public float curentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        curentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = curentHealth/100;
        hpText.text = curentHealth.ToString() + "/" + _maxHealth.ToString() + "HP";

        if(curentHealth > _maxHealth)
        {
            curentHealth = _maxHealth;
        }
        else if(curentHealth < 0)
        {
            curentHealth = 0;
        }
    }
}
