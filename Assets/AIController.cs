using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive ds;
    public float steeringSensitivity = 0.01f;
    Vector3 target;
    int currentWP = 0;

    // Start is called before the first frame update
    void Start()
    {
        ds = GetComponent<Drive>();
        target = circuit.waypoints[currentWP].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = ds.rb.gameObject.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, ds.rb.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);
        float accel = 1f;
        float brake = 0;

        //if (distanceToTarget < 5) { brake = 0.8f; accel = 0.1f; }

        ds.Go(accel, steer, brake);

        

        if(distanceToTarget < 2) // threshold, make larger if car starts to circle waypoint
        {
            currentWP++;
            if (currentWP >= circuit.waypoints.Length) 
                currentWP = 0;
            target = circuit.waypoints[currentWP].transform.position;
        }

        ds.CheckForSkid();
        ds.CalculateEngineSound();

    }
}
