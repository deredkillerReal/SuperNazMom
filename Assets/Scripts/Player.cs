using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
//[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]



public class Player : MonoBehaviour
{
    public Animator animator;
    public AudioManager audioManager;
    float maxHealth = 100;
    float health = 100;
    [Header("debug")]
    public int debugDamagePlayer = 0;
    private PlayerManager playerManager;
    public Slider healthSlider;

    void Start()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        healthSlider = playerManager.AddPlayer(this);
        audioManager = FindAnyObjectByType<AudioManager>();

    }
    void Update()
    {
        if (debugDamagePlayer != 0)
        {
            changeHealth(debugDamagePlayer);
            debugDamagePlayer = 0;
        }
    }

    public void changeHealth(float hp = 10)
    {
        health = (Mathf.Clamp(health + hp, 0, maxHealth));
        healthSlider.value = health;
        Debug.Log(health);
        if (health == 0)
        {
            Die();
            audioManager.Play("Die");
        }
        else
        {
            animator.SetTrigger("hurt");
            audioManager.Play("Hurt_1");
        }
    }


    public void buttonReset()
    {
        SceneManager.LoadScene("scene1");
    }

    private void Die()
    {
        for (int i = 0; i < PlayerManager.players.Length; i++)
        {
            if (PlayerManager.players[i] = this)
            {
                PlayerManager.players[i] = null;
                break;
            }
        }
        GetComponent<Collider2D>().enabled = false;
        //GetComponent<Player>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        //GetComponent<PlayerInput>().enabled = false;


        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Debug.LogError("yaYEET am die");
        animator.SetTrigger("Die");
        //Destroy(gameObject);
    }



}
