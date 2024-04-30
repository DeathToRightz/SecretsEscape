using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    RaycastHit sightline;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward * 15, out sightline);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 30f, 0) * transform.forward, Color.red);
        if (sightline.transform.gameObject.name == "Player")
        {
            Debug.Log("I see you");
        }



    }


}
