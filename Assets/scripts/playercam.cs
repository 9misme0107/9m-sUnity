using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class playercam : MonoBehaviour
{
    public float senX;
    public float senY;
    public Transform orientaion;
    float xRotation;
    float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X")*Time.deltaTime*senX;
        float mouseY = Input.GetAxisRaw("Mouse Y")*Time.deltaTime*senY;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);

        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);
        orientaion.rotation = Quaternion.Euler(0,yRotation,0);
    }
}
