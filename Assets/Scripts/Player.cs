using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    private AudioManager audioManager;
    float maxHealth = 100;
    float health = 100;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeHealth(float hp)
    {
        health += hp;
        Mathf.Clamp(health, 0, maxHealth);
        Debug.Log(health);
        if (health == 0) Die();
    }

    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;

        Debug.LogError("yaYEET");
        Destroy(gameObject);
    }


}
