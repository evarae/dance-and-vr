using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
    private Vector3 lastPos;
    private Vector3 lastVel;
    private Vector3 curVel;
    private float curSpeed;
    private float lastSpeed;
    private float acceleration;

    // Next update in second
    private float nextUpdate = 0.1f;

    private float timeElapse = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastVel = new Vector3(0,0,0);
        curVel = new Vector3(0,0,0);
        lastSpeed = 0f;
        curSpeed = 0f;
    }

     
    // Update is called once per frame
    void Update(){
     
        // If the next update is reached
        if(Time.time>=nextUpdate){
            // Change the next update (current second+1)
            nextUpdate+=timeElapse;
            // Call your fonction
            UpdateVelocities();
        }
    }

    // Update is called once per frame
    void UpdateVelocities()
    {
        lastVel = curVel;
        lastSpeed = curSpeed;

        curVel = (transform.position - lastPos)/timeElapse;

        //print("position:" + transform.position + " lastPosition: " + lastPos + "last-pos: " + (transform.position-lastPos));
        curSpeed = curVel.magnitude;
        acceleration = (curSpeed - lastSpeed)/timeElapse;
        lastPos = transform.position;
    }

    public Vector3 getVelocity(){
        return curVel;
    }

    public float getSpeed(){
        return curSpeed;
    }

    public Vector3 getLastVelocity(){
        return lastVel;
    }

    public float getLastSpeed(){
        return lastSpeed;
    }

    public float getAcceleration(){
        return acceleration;
    }

    public float getHeight(){
        return transform.position.y;
    }

}
