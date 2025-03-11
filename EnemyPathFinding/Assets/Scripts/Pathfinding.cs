using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public GameObject pointA, pointB;
    public GameObject currentDestination;
    public bool patrol = true;
    public float radius = 5f;

    public void Start()
    {
        currentDestination = pointA;
        Patrol();
    }

    public void Chase()
    {
        StopAllCoroutines();
        agent.SetDestination(target.transform.position);
    }

    public void StopChase()
    {
        StopAllCoroutines();
    }

    public void StopRunAway()
    {
        StopAllCoroutines();
    }

    public void Patrol()
    {
        StopAllCoroutines();
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        while (patrol)
        {
            agent.SetDestination(currentDestination.transform.position);
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
            currentDestination = currentDestination == pointA ? pointB : pointA;
            yield return new WaitForSeconds(1f);
        }
    }
    public void RunAway()
    {
        StopAllCoroutines();
        Vector3 directionAway = (transform.position - target.transform.position).normalized;
        float fleeDistance = 10f;
        Vector3 newTargetPosition = transform.position + (directionAway * fleeDistance);
        agent.SetDestination(newTargetPosition);
    }

    public void PatrolAround(Vector3 center)
    {
        float fristX = Random.Range(center.x - radius, center.x + radius);
        float fristZ = Random.Range(center.z - radius, center.z + radius);
        float secondX = Random.Range(center.x - radius, center.x + radius);
        float secondZ = Random.Range(center.z - radius, center.z + radius);

        pointA.transform.position = new Vector3(fristX, center.y, fristZ);
        pointB.transform.position = new Vector3(secondX, center.y, secondZ);
        StartCoroutine(PatrolRoutine());
    }
}