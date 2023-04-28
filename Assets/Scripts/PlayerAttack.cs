using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//using static PlayerAttack;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;

public class PlayerAttack : MonoBehaviour
{

    [Header("DEBUG")] public AttacksSug a;
    public AttacksSug b;
    public AttacksSug c;
    public bool att = false;

    private AttacksSug attackToQueue=AttacksSug.Empty;
    [SerializeField] UnityEngine.Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] int attackDamage = 25;

    [SerializeField] float timeToBreakCombo = 5.0f;
    float TimeLastAttack = 0;
    float AttackCooldown = 0.5f;
    float AttackCooldownIn = 0f;
    private bool canAttack = true;

    public LayerMask enemyLayer;

    public Animator animator;
    Player player;
    ComboManager comboManager;

    public GameObject punchEffect;

    void Start()
    {
        player = GetComponent<Player>();
        Invoke("shet", 0.5f);
    }
    void shet()
    {
        Dictionary<AttacksSug, AttacksSug[]> specialMovesPPP = new Dictionary<AttacksSug, AttacksSug[]>();

        comboManager = new ComboManager(specialMovesPPP);

    }

    [SerializeField] private VisualEffect ve;

    private void Update()
    {
        if (att)
        {
            att = false;
            //comboManager = new ComboManager();
            comboManager.addAttack(a);
            comboManager.addAttack(b);
            comboManager.addAttack(c);
            AttacksSug ass = comboManager.calculateAttack();
            if (ass == AttacksSug.Stab) Debug.Log("stab shet");
            if (ass == AttacksSug.SuperHeavy) Debug.Log("supah smash");
            if (ass == AttacksSug.Heavy)
            {
                Debug.Log("Heavy");
            }
            if (ass == AttacksSug.Light) Debug.Log("light");
            if (ass == AttacksSug.SpecialDefault) Debug.Log("Lazer BEANSSS");
            if (ass == AttacksSug.Stomp) Debug.Log("stomp");
            if (ass == AttacksSug.StompExtended) Debug.Log("supah Stomp");
        }

    }




    void AttackManager(AttacksSug inputAttack)
    {


        switch (checkCanAttack())
        {
            case -1:
                return;
                break;
            case 0:

                //QueueAttack

                break;
            case 1:
                bool breakCombo = (Time.time - TimeLastAttack > timeToBreakCombo);
                if (breakCombo) comboManager.clearArray();
                comboManager.addAttack(inputAttack);
                Attack(comboManager.calculateAttack());

                break;
        }


    }

    private int checkCanAttack()
    {
        /*
         if (Time.time - TimeLastAttack > AttackCooldown) canAttack = true;
        else if (Time.time - TimeLastAttack > AttackCooldown - 0.2)
        {
            Debug.Log("0.2 seconds before end");
            Invoke("Attack", Time.time - TimeLastAttack);
        }
        */

        if (Time.time - AttackCooldownIn > 0)
        {

            return 1;//1== good, can attack
        }
        else if (Time.time - AttackCooldownIn > -0.2f)
        {
            Debug.Log("Need to Queue Next Attack");
            return 0;//0== Queue, attack later
        }
        return -1;//-1== Bad,Dont Attack
    }

    void Attack(AttacksSug attackToPerform)
    {


        //how t oconvert if block to switch
        if (attackToPerform == AttacksSug.Stab) Debug.Log("stab shet");
        if (attackToPerform == AttacksSug.SuperHeavy) Debug.Log("supah smash");
        if (attackToPerform == AttacksSug.Heavy)
        {
            Debug.Log("Heavy");
            ve.Play();
        }
        if (attackToPerform == AttacksSug.Light)
        {
            Debug.Log("light");
            Instantiate(punchEffect, transform, true);
        }
        if (attackToPerform == AttacksSug.SpecialDefault) Debug.Log("Lazer BEANSSS");
        if (attackToPerform == AttacksSug.Stomp) Debug.Log("stomp");
        if (attackToPerform == AttacksSug.StompExtended) Debug.Log("supah Stomp");

        //animator.SetTrigger("lightAttack");

        //detect enemies
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        //do something to enemy
        foreach (Collider2D enemy in enemiesHit)
        {
            if (enemy.gameObject != gameObject)
            {
                //if(animator.GetFloat("AttackWindow")>0f);
                enemy.GetComponent<Player>().changeHealth(-attackDamage);
                //enemy.GetComponent<PlayerAttack>().animator.SetTrigger("hurt");
            }
        }
        float thisAttackCooldown = 0.5f;
        AttackCooldownIn = Time.time + thisAttackCooldown;
        TimeLastAttack = Time.time;
    }
    private void Block()
    {
        Debug.Log("blockiing");
    }

    public void onBlock(InputAction.CallbackContext context)
    {

        Block();


        Dictionary<AttacksSug, AttacksSug[]> SpecialMoves = new Dictionary<AttacksSug, AttacksSug[]>();
        //SpecialMoves.Add(AttacksSug.a, new AttacksSug[] { AttacksSug. , AttacksSug. , AttacksSug. }); //adding a key/value using the Add() method

        SpecialMoves.Add(AttacksSug.Stab, new AttacksSug[] { AttacksSug.Light, AttacksSug.Light, AttacksSug.Light });
        SpecialMoves.Add(AttacksSug.SuperHeavy, new AttacksSug[] { AttacksSug.Heavy, AttacksSug.Heavy, AttacksSug.Heavy });
        SpecialMoves.Add(AttacksSug.Stomp, new AttacksSug[] { AttacksSug.Light, AttacksSug.Heavy, AttacksSug.Light });


    }
    public void onHeavy(InputAction.CallbackContext context)
    {

        AttackManager(AttacksSug.Heavy);
    }
    public void onLight(InputAction.CallbackContext context)
    {

        AttackManager(AttacksSug.Light);
    }
    public void onSpecial(InputAction.CallbackContext context)
    {

        AttackManager(AttacksSug.SpecialDefault);
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public enum AttacksSug
    {
        Light = 01,
        Heavy = 02,
        SpecialDefault = 03,
        Empty = -01,

        Stab = 11,
        SuperHeavy = 12,
        Stomp = 13,
        StompExtended = 14


    }



    private class ComboManager
    {
        AttacksSug[] attackQueue = new AttacksSug[3];
        public static ComboManager comboManager;
        public Dictionary<AttacksSug, AttacksSug[]> specialMoves = new Dictionary<AttacksSug, AttacksSug[]>();

        public ComboManager(Dictionary<AttacksSug, AttacksSug[]> specialMovesPlayer)
        {
            //if (comboManager != null) { Debug.Log("comboManager has already spawned"); return; }
            //else comboManager = this;

            clearArray();


            specialMoves.Add(AttacksSug.Stab, new AttacksSug[] { AttacksSug.Light, AttacksSug.Light, AttacksSug.Light });
            specialMoves.Add(AttacksSug.SuperHeavy, new AttacksSug[] { AttacksSug.Heavy, AttacksSug.Heavy, AttacksSug.Heavy });
            specialMoves.Add(AttacksSug.Stomp, new AttacksSug[] { AttacksSug.Light, AttacksSug.Heavy, AttacksSug.Empty });
            specialMoves.Add(AttacksSug.StompExtended, new AttacksSug[] { AttacksSug.Light, AttacksSug.Heavy, AttacksSug.Heavy });
        }



        public void addAttack(AttacksSug newAttack)
        {
            for (int i = 0; i < attackQueue.Length; i++)
            {
                if (attackQueue[i] == AttacksSug.Empty)
                {
                    attackQueue[i] = newAttack;
                    return;
                }

            }
            Debug.Log("array full need to clear");
            clearArray();
            attackQueue[0] = newAttack;
        }
        public AttacksSug calculateAttack()
        {
            //dictionary.Keys.Toarray doesnt seem to work
            AttacksSug[] keys = specialMoves.Keys.ToArray<AttacksSug>();
            bool flag = true;
            //Debug.Log(attackQueue[0] + "," + attackQueue[1] + "," + attackQueue[2]);


            //iterates over every key in the dictionary
            for (int i = 0; i < keys.Length; i++)
            {

                //checks if this dictionary entry is equals to current attack Queue
                for (int j = 0; j < attackQueue.Length; j++)
                {
                    if (attackQueue[j] != specialMoves[keys[i]][j])
                    {
                        flag = false;
                        break;

                    }
                }
                if (flag) return keys[i];
                flag = true;
            }
            //Debug.Log("no combo only last attack");
            return getLastAttack();



            //Debug.Log(attackQueue[i]+"   "+ specialMoves[AttacksSug.Stab][i]);




        }
        public void clearArray()
        {

            /*break combo*/
            //Array.Fill(attackQueue, AttacksSug.Empty,);
            attackQueue = Enumerable.Repeat(AttacksSug.Empty, attackQueue.Length).ToArray();
        }
        private AttacksSug getLastAttack()
        {
            for (int i = attackQueue.Length - 1; i >= 0; i--)
            {
                if (attackQueue[i] != null && attackQueue[i] != AttacksSug.Empty)
                {

                    return attackQueue[i];

                }
            }
            return AttacksSug.Empty;
        }
    }
}
