using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private int _maxHealth = 100;
    [SerializeField]
    private float _curentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        _curentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = _curentHealth/100;
        hpText.text = _curentHealth.ToString() + "/" + _maxHealth.ToString() + "HP";

        if(_curentHealth > _maxHealth)
        {
            _curentHealth = _maxHealth;
        }
        else if(_curentHealth < 0)
        {
            _curentHealth = 0;
        }
    }
}
