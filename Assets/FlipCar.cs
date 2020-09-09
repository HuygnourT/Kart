using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    public Rigidbody rb;
    float lastTimeChecked;

    void RightCar()
    {
        transform.position += Vector3.up;
        transform.rotation = Quaternion.LookRotation(transform.forward);
        print("Right Car");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.up.y > 0f || rb.velocity.magnitude > 1)
        {
            lastTimeChecked = Time.time;
        }

        if(Time.time > lastTimeChecked + 3)
        {
            RightCar();
        }
    }
}
