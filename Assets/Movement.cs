using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public Transform mainTarget; // Assign this in the Inspector
    private NavMeshAgent agent;
    public float aggroRadius = 10f;
    private Transform currentTarget;
    public int TeamNumber;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = mainTarget; // Start by moving towards the main target
        agent.SetDestination(currentTarget.position);
        Debug.Log($"Target locked: {currentTarget.position}");
    }

    void Update()
    {
        // Check for enemies within aggro radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Entity") && hitCollider.transform != this.transform) // Assuming enemies have the tag "Enemy"
            {
                //Debug.Log($"Object in ragne: {hitCollider.gameObject.name}");
                Movement enemy = hitCollider.GetComponent<Movement>();
                if (enemy !=null && enemy.TeamNumber != TeamNumber)
                {
                    //for later
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    Debug.Log($"Distance to {enemy.name}: {distance}");
                    Transform newTarget = hitCollider.transform;
                    // Move towards the current target
                    if (newTarget != null && newTarget != currentTarget)
                    {
                        agent.SetDestination(newTarget.position);
                        Debug.Log($"{gameObject.name} target switched: {newTarget.position} ({newTarget.gameObject.name})");
                    }
                }

                //TODO: Implement GoalStrategy

                break;
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the aggro radius in the scene view for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }

}
