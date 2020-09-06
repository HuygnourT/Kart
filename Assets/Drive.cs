using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] WCs = new WheelCollider[4];
    public GameObject[] WheelMesh;// = new GameObject[4];

    public float torque = 200;
    public float maxSteerAngle = 30;
    public float maxBrakeTorque = 500;

    public AudioSource skidSound;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
            skidTrails[i] = Instantiate(SkidTrailPrefab);

        skidTrails[i].parent = WCs[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null) return;
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }
    

    void Go(float accel,float steer,float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;
        float thrustTorque = accel * torque;

        //handle wheel rotate and moving according to WC
        for(int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            if (i < 2)
                WCs[i].steerAngle = steer;
            else
                WCs[i].brakeTorque = brake; 

            Quaternion quat;
            Vector3 position;
            WCs[i].GetWorldPose(out position, out quat);
            WheelMesh[i].transform.position = position;
            WheelMesh[i].transform.rotation = quat;
        }
        
    }

    void CheckForSkid()
    {
        int numSkidding = 0;
        for(int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);
            
            if(Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if(!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                //StartSkidTrail(i);
            } else
            {
                //EndSkidTrail(i);
            }
        }

        if (numSkidding == 0 && skidSound.isPlaying)
            skidSound.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        Go(a,s,b);
        CheckForSkid();
    }
}
