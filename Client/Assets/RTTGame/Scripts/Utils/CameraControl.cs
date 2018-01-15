using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float Speed = 10;
    private Vector3 xSpeed = new Vector3(1, 0, 0);
    private Vector3 ySpeed = new Vector3(0, 0, 1);

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.A))
        {
            this.transform.localPosition -= xSpeed * Time.deltaTime * Speed;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            this.transform.localPosition += xSpeed * Time.deltaTime * Speed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.localPosition += ySpeed * Time.deltaTime * Speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.localPosition -= ySpeed * Time.deltaTime * Speed;
        }
    }
}
