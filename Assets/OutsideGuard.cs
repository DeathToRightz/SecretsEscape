using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class OutsideGuard : Guard
{
    [SerializeField] GameObject[] guard1Wapoints;
    private Vector3[] guard1WapointsLocations;
    Vector3[] guard2Wapoints;
    public int guardWaypointIndex = 0;
    private enum Guard
    {
        Guard1,
        Guard2
    }
    [SerializeField] Guard guardVersion;
    private void Awake()
    {
        guardAgent = GetComponent<NavMeshAgent>();
        ObtainWaypointLocations(guard1Wapoints);     

    }

    // Update is called once per frame
    void Update()
    {
        SightCone();

        if( guardVersion.Equals(Guard.Guard1) )
        {
            if(!guardAgent.pathPending && guardAgent.remainingDistance < 0.5f)
            {
                PatrolRoute(guard1WapointsLocations);
            }
           
        }
        else
        {
            PatrolRoute(guard2Wapoints);
        }
        
    }

    void PatrolRoute(Vector3[] guardWaypoints)
    {
        if (guardWaypoints.Length == 0)
        {
            return;
        }
        guardAgent.destination = guardWaypoints[guardWaypointIndex];
        guardWaypointIndex = (guardWaypointIndex + 1) % guardWaypoints.Length;     
    }

    void ObtainWaypointLocations(GameObject[] guardWaypoints)
    {
        guard1WapointsLocations = new Vector3[guardWaypoints.Length];
        for(int i = 0; i <= guardWaypoints.Length-1; i++)
        {
            guard1WapointsLocations[i] = guardWaypoints[i].transform.position;
        }
        
        
    }
}
