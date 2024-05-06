using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class OutsideGuard : Guard
{
    
    [SerializeField] GameObject[] wayPointsTest;
    [SerializeField] GameObject player;
    [SerializeField] bool chasePlayerConstant;
    
    private Vector3[]  guardWaypointsTest;
    private int guardWaypointIndex = 0;
    private float guardSpeed = 3.5f;
    
    //[SerializeField] Guard guardVersion;
    private void Awake()
    {       
        guardAgent = GetComponent<NavMeshAgent>();
        ObtainWaypointLocations(wayPointsTest);
        if (chasePlayerConstant)
        {
            guardSpeed = 3.5f * 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SightCone();       
        if ( !chasePlayerConstant && !guardAgent.pathPending && guardAgent.remainingDistance < 0.5f )
        {
            
            PatrolRoute(guardWaypointsTest);
        }
        if( chasePlayerConstant  )
        {
           
            guardAgent.destination = player.transform.position;
            
        }


    }
    private void LateUpdate()
    {
        ChasePlayer(seePlayer);
        
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
        guardWaypointsTest = new Vector3[guardWaypoints.Length];
        for(int i = 0; i <= guardWaypoints.Length-1; i++)
        {
            guardWaypointsTest[i] = guardWaypoints[i].transform.position;
        }               
    }

    void ChasePlayer(bool seePlayer)
    {
        if( seePlayer )
        {
            guardAgent.destination = player.transform.position;
            transform.LookAt(player.transform.position);
            guardAgent.speed = guardSpeed * 2;
          
        }
        else
        {
            guardAgent.speed = guardSpeed;
            return;
        }
    }
}
