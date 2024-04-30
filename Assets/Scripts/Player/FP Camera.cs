using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamera : MonoBehaviour
{
    [SerializeField] Transform  orientation;
    [SerializeField] float sensX, sensY;
    float yRotation, xRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        float mouseX = Input.GetAxis("Mouse X")  * sensX * 0.01f;
        float mouseY = Input.GetAxis("Mouse Y")  * sensY * 0.01f;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void FixedUpdate()
    {
       
    }
}
