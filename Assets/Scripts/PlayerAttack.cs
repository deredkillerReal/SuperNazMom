using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;
using UnityEngine.Pool;
public class PlayerAttack : MonoBehaviour
{
    private ObjectPool<GameObject> pool;
    [SerializeField] float aaa;
    [SerializeField] AttackStats canISerializeThis;
    [Header("DEBUG")]
    [SerializeField] AttacksSug a;
    [SerializeField] AttacksSug b;
    [SerializeField] AttacksSug c;
    [SerializeField] bool debugbutton = false;

    private AttacksSug attackToQueue = AttacksSug.Empty;
    private Coroutine QueueCoroutine;
    private bool shouldStopCoroutine = false;
    [SerializeField] UnityEngine.Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] int attackDamage = 25;

    [SerializeField] float timeToBreakCombo = 5.0f;
    float TimeLastAttack = 0;
    float AttackCooldownIn = 0.5f;
    private bool canAttack = true;

    [SerializeField] LayerMask enemyLayer;

    Player player;
    ComboManager comboManager;

    [SerializeField] GameObject punchEffect;
    [SerializeField] private VisualEffect ve;

    void Start()
    {
        /*pool = new ObjectPool<GameObject>(
            () =>{ return Instantiate(punchEffect); },
            effect => { effect.SetActive(true); },
            effect => { effect.SetActive(false); },
            effect => { Destroy(gameObject); },
            true, 6);*/
        player = GetComponent<Player>();
        Invoke("initiateComboManager", 0.1f);
    }

    void initiateComboManager()
    {
        Dictionary<AttacksSug, AttacksSug[]> specialMovesPPP = new Dictionary<AttacksSug, AttacksSug[]>();

        comboManager = new ComboManager(specialMovesPPP);
        QueueCoroutine = StartCoroutine(CheckAttackqueue());

    }
    void Update()
    {
        //this is for Debugging purposes
        if (debugbutton)
        {
            debugbutton = false;
            comboManager.addAttack(a);
            comboManager.addAttack(b);
            comboManager.addAttack(c);
            AttacksSug ass = comboManager.calculateAttack();
            Attack(ass);
        }


        //if (attackToQueue != AttacksSug.Empty)
        //{
        //    AttackManager(attackToQueue);
        //}

    }



    void AttackManager(AttacksSug inputAttack)
    {


        switch (checkCanAttack())
        {
            case -1://attack on cooldown,cant attack, etc
                return;

            case 0://attack needs to be queued
                 {
                    attackToQueue = inputAttack;
                    if (shouldStopCoroutine) StopCoroutine(QueueCoroutine);

                    QueueCoroutine = StartCoroutine(CheckAttackqueue());
                }
                break;
            case 1:
                if (shouldStopCoroutine) StopCoroutine(QueueCoroutine);
                bool breakCombo = (Time.time - TimeLastAttack > timeToBreakCombo);
                if (breakCombo) comboManager.clearArray();
                comboManager.addAttack(inputAttack);
                Attack(comboManager.calculateAttack());

                attackToQueue = AttacksSug.Empty;
                break;
        }


    }
    IEnumerator CheckAttackqueue()
    {
        while (attackToQueue != AttacksSug.Empty)
        {
            AttackManager(attackToQueue);
            yield return null;
        }
    }

    int checkCanAttack()
    {

        if (Time.time - AttackCooldownIn > 0)
        {

            return 1;//1== good, can attack
        }
        else if (Time.time - AttackCooldownIn > -0.2f)
        {
            return 0;//0== Queue, attack later
        }
        return -1;//-1== Bad,Dont Attack

    }

    [System.Serializable]
    public class AttackStats
    {
        public float damage { get; set; }
        public float cooldown { get; set; }
        public float range { get; set; }
        public VisualEffect vfx { get; set; }
        public float AttackDelay { get; set; }//after how long should the attack deal damage
        public string animationName { get; set; }

        //float finishedAttackCooldown;
        public AttackStats(float damage, float cooldown, float range, VisualEffect vfx, float AttackDelay, string animationName)
        {
            this.damage = damage;
            this.cooldown = cooldown;
            this.range = range;
            this.vfx = vfx;
            this.AttackDelay = AttackDelay;
            this.animationName = animationName;

        }
    }
    void Attack(AttacksSug attackToPerform)
    {
        AttackStats stats;
        switch (attackToPerform)
        {
            case AttacksSug.Light:
                stats = new(10, 0.55f, 2, punchEffect.GetComponent<VisualEffect>(), 0.55f, "light");

                break;
            case AttacksSug.Heavy:
                stats = new(20, 0.7f, 2, punchEffect.GetComponent<VisualEffect>(), 0.5f, "heavy");

                break;
            case AttacksSug.SpecialDefault:
                stats = new(15f, 1f, 8, punchEffect.GetComponent<VisualEffect>(), 0.3f, "special");
                break;
            case AttacksSug.Stab:
                stats = new(20, 0.8f, 2, punchEffect.GetComponent<VisualEffect>(), 1f, "stab");
                break;
            case AttacksSug.Stomp:
                stats = new(20, 0.9f, 2, punchEffect.GetComponent<VisualEffect>(), 1f, "stomp");
                break;
            case AttacksSug.StompExtended:
                stats = new(20, 0.9f, 2, punchEffect.GetComponent<VisualEffect>(), 1f, "super stomp");
                break;
            case AttacksSug.SuperHeavy:
                stats = new(20, 1.5f, 2, punchEffect.GetComponent<VisualEffect>(), 1f, "super heavy");
                break;
            default:
                stats = new(0, 0f, 0, punchEffect.GetComponent<VisualEffect>(), 0, "light");
                Debug.LogError("wtf this aint supposed to happen" + attackToPerform);
                break;


        }
        attackRange = stats.range;

        player.animator.SetTrigger(stats.animationName);
        StartCoroutine(attackCoroutine(stats, attackToPerform));
        AttackCooldownIn = Time.time + stats.cooldown;
        TimeLastAttack = Time.time;
    }
    IEnumerator attackCoroutine(AttackStats stats, AttacksSug toPerform)
    {
        yield return new WaitForSeconds(stats.AttackDelay);
        ve.Play();
        Collider2D[] enemiesHit;
        if (toPerform == AttacksSug.SpecialDefault)
            enemiesHit = Physics2D.OverlapAreaAll(transform.position, new Vector2(transform.position.x + stats.range, transform.position.y - 0.8f), enemyLayer);
        else
            enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, stats.range, enemyLayer);

        foreach (Collider2D enemy in enemiesHit)
        {
            if (enemy.gameObject != gameObject)
            {
                //if(animator.GetFloat("AttackWindow")>0f);
                enemy.GetComponent<Player>().changeHealth(-stats.damage);
                enemy.GetComponent<Player>().animator.SetTrigger("hurt");
            }
        }


    }


    void Block()
    {
        player.animator.SetTrigger("block");
    }

    public void onBlock(InputAction.CallbackContext context)
    {
        Block();
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
        Gizmos.color = Color.yellow;

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
        public Dictionary<AttacksSug, AttacksSug[]> specialMoves = new Dictionary<AttacksSug, AttacksSug[]>();

        public ComboManager(Dictionary<AttacksSug, AttacksSug[]> specialMovesPlayer)
        {
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
            AttacksSug[] keys = specialMoves.Keys.ToArray<AttacksSug>();
            bool flag = true;
            //Debug.Log(attackQueue[0] + "," + attackQueue[1] + "," + attackQueue[2]);


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
        }
        public void clearArray()
        {
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
