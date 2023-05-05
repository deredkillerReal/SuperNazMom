using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
//[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]



public class Player : MonoBehaviour
{
    static Player[] players;
    public Animator animator;
    public AudioManager audioManager;
    float maxHealth = 100;
    float health = 100;
    
    [Header("debug")]
    public int debugDamagePlayer=0;
    private void Awake()
    {
        if(players==null)players=new Player[4];
        bool hasPlaced=false;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) { players[i] = this; hasPlaced = true; break; }
        }
        if (!hasPlaced) Debug.LogError("player array full too many players");
    }
    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        
        
    }
    void Update()
    {
        if (debugDamagePlayer!=0) { changeHealth(debugDamagePlayer);
            debugDamagePlayer = 0;
        }
    }

    public void changeHealth(float hp=10)
    {
        health=(Mathf.Clamp(health+hp, 0, maxHealth));
        Debug.Log(health);
        if (health == 0) Die();
        else animator.SetTrigger("hurt");
    }

    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Player>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerInput>().enabled = false;


        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Debug.LogError("yaYEET am die");
        animator.SetTrigger("die");
        //Destroy(gameObject);
    }


}
