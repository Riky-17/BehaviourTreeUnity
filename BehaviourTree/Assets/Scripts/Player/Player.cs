using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 dir;
    float speed = 10;

    void Update()
    {
        dir = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
            dir += Vector3.forward;
        if(Input.GetKey(KeyCode.S))
            dir -= Vector3.forward;
        if(Input.GetKey(KeyCode.D))
            dir += Vector3.right;
        if(Input.GetKey(KeyCode.A))
            dir -= Vector3.right;

        dir = dir.normalized;
        transform.position += speed * Time.deltaTime * dir;
    }
}
