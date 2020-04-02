using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool canControl = false;
    public float speedMove = 2.5f;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
            return;

        Vector3 axis = (Vector3.right * Input.GetAxis("Horizontal")) +
                        (Vector3.up * Input.GetAxis("Vertical"));
        axis.Normalize();
        this.transform.position += axis * speedMove * Time.deltaTime;
    }
}
