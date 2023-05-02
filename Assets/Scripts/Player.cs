using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    private AudioManager audioManager;

    float maxHealth = 100;
    float health = 100;
    
    [Header("debug")]
    public int debugDamagePlayer=0;

    void Start()
    {
        //audioManager = GetComponent<AudioManager>();
        
    }
    void Update()
    {
        if (debugDamagePlayer!=0) { changeHealth(debugDamagePlayer);
            debugDamagePlayer = 0;
        }
    }

    public void changeHealth(float hp=10)
    {
        health += hp;
        health = Mathf.Max(health, 0);
        Debug.Log(Mathf.Clamp(-15, 0, maxHealth));
        Debug.Log(health);
        if (health == 0) Die();
        else animator.SetTrigger("hurt");
    }

    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;

        Debug.LogError("yaYEET");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("die");
        //Destroy(gameObject);
    }


}
