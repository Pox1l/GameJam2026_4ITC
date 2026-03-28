using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float damage = 5f;

    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerStats>(out PlayerStats playerStats) && playerStats != null)
        {
            playerStats.currentHealth -= damage;
            Destroy(gameObject);
        }
    }
}
