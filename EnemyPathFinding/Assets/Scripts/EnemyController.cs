using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyController : MonoBehaviour, IDamageable
{
    public int HP;
    public GameObject target;
    public bool OnVisionRange = false, OnAttackRange = false, runAway = false;
    public Pathfinding _chaseB;
    public StateSO currentNode;
    public List<StateSO> Nodes;
    public float AttackRange = 2f;
    public Vector3 lastPlayerPosition;
    public EnemyFOV EnemyFOV;
    public Pathfinding Pathfinding;

    void Start()
    {
        EnemyFOV = GetComponent<EnemyFOV>();
        Pathfinding = GetComponent<Pathfinding>();
        _chaseB = GetComponent<Pathfinding>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            _chaseB.target = target;
            OnVisionRange = true;
            lastPlayerPosition = EnemyFOV.GetLastPlayerPosition(target);
            CheckEndingConditions();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (EnemyFOV.CheckPlayerInVision(other.gameObject))
            {
                OnAttackRange = Vector3.Distance(transform.position, other.transform.position) < AttackRange;
                lastPlayerPosition = EnemyFOV.GetLastPlayerPosition(other.gameObject);
                CheckEndingConditions();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnVisionRange = false;
            CheckEndingConditions();
            if (HP < 100)
            {
                Pathfinding.PatrolAround(lastPlayerPosition);
            }
        }
    }

    private void Update()
    {
        currentNode.OnStateUpdate(this);
    }

    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
        {
            if (condition.CheckCondition(this) == condition.answer) ExitCurrentNode();
        }
    }

    public void ExitCurrentNode()
    {
        foreach (StateSO stateSO in Nodes)
        {
            if (stateSO.StartCondition == null || stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
            {
                EnterNewState(stateSO);
                break;
            }
        }
        currentNode.OnStateEnter(this);
    }

    private void EnterNewState(StateSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }
    public void TakeDamage(float damage)
    {
        HP -= (int)damage;
        if (HP <= 25)
        {
            runAway = true;
        }
        CheckEndingConditions();
    }
}
