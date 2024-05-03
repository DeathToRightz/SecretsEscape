using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    private RaycastHit sightline;
    public bool seePlayer;
    [SerializeField] Transform guardFOV;
    public NavMeshAgent guardAgent;
    
    public void SightCone()
    {
        for (int i = -45; i <= 45; i += 15)
        {
            if (Physics.Raycast(guardFOV.position, Quaternion.Euler(0,i,0) * transform.forward, out sightline, 10))
            {
                if (sightline.transform.name == "Player")
                {
                     seePlayer = true;
                }
                else if (sightline.transform.name != "Player")
                {
                     seePlayer = false;
                }
            }
            Debug.DrawRay(guardFOV.position, Quaternion.Euler(0, i, 0) * transform.forward * 10, Color.red);
        }        
    }
}
