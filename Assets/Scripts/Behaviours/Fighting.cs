using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Fighting : MonoBehaviour
{
    [HideInInspector]
    public bio b;
    public sbyte UnitTypeID = 0;
    public GM gm;


    public bool Attacking = false;

    public GameObject Target;
    private Animator anim;
    private AIBase ab;
    private float TimerAttackSpeed = 0;
    private float TimerTargetCheck = 0;
    void Start()
    {
        ab = GetComponent<AIBase>();
        b = GetComponent<bio>();
        try
        {
            anim = GetComponent<Animator>();
        }
        catch
        {
            Debug.Log("No animation attached " + gameObject.name);
        }
    }

    
    void Update()
    {
        if (Attacking)
        {
            if (Target != null)
            {
                Hit(Target);
            }
            else
            {
                Attacking = false;
            }
        }
        else
        {
            if (gameObject.name.Contains("Pack"))
            {
                anim.SetBool("AttackStarted", false);
            }
        }
    }
    void FixedUpdate()
    {
        anim.SetFloat("Speed", ab.velocity.sqrMagnitude);
    }

    public int Hit(GameObject target) 
    {
        Target = target;
        if (Vector3.Distance(transform.position, target.transform.position) < b.AttackRange)
        {
            if (gameObject.name.Contains("Pack"))
            {
                anim.SetBool("AttackStarted", true);
            }
            if (TimerAttackSpeed > b.AttackSpeed)
            {
                TimerAttackSpeed = 0;
                if (target.gameObject.tag != "artificial")
                {
                    bio AttackedBio = target.GetComponent<bio>();
                    AttackedBio.HPCurrent -= b.Damage;
                    //проверка на смерть
                    if (AttackedBio.HPCurrent <= 0)
                    { //убиваем
                        target.GetComponent<bio>().Death(gameObject);
                        return 2;
                    }
                }
                else
                { //ломаем здание или ресурс
                   
                }
                return 1;
            }
            else
            {
                TimerAttackSpeed += Time.deltaTime;
            }
        }
        else
        {
            TimerAttackSpeed = 0;
            if (gameObject.name.Contains("Pack"))
            {
                anim.SetBool("AttackStarted", false);
            }
        }
        return 0;
    }
}
