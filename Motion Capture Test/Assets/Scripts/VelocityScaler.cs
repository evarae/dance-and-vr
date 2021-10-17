using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VelocityScaler : MonoBehaviour
{

    public VelocityTracker[] velTrackList;

    [Range (0,100)]
    public float maxSpeed = 10f;

    [Range (0,100)]
    public float minSpeed = 0f;

    [Range (0,1)]
    public float minScale = 1f;

    [Range (1, 100)]
    public float maxScale = 5f;

    public float delta = 0.1f;

    private float ratio;
    private float prevRatio;

    private Vector3 _initScale;
    private Transform _tf;

    void Start()
    {
        _initScale = gameObject.transform.localScale;
        
        ratio = 0;
        prevRatio = 0;
    }

    void Update()
    {
        float sum = 0f;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getSpeed();
        }

        float speed = sum/(velTrackList.Length); //get average speed

        ratio = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        ratio = ratio*ratio;

        //Smooth out transitions using delta
        if(ratio > prevRatio){
            float maxRatio = prevRatio + delta*Time.deltaTime;
            ratio = Math.Min(maxRatio, ratio);
        } else if (ratio < prevRatio){
            float minRatio = prevRatio - delta*Time.deltaTime;
            ratio = Math.Max(minRatio, ratio);
        }

        float scale = ((maxScale - minScale)*(ratio*ratio) + minScale);

        //Update the scale of the transform
        gameObject.transform.localScale = new Vector3(_initScale.x, scale*_initScale.y, _initScale.z);

        prevRatio = ratio;

    }

        //     float sum = 0f;
        // foreach (var velTrack in velTrackList)
        // {
        //     sum = sum + velTrack.getSpeed();
        // }

        // float speed = sum/(velTrackList.Length); //get average speed

        // if(speed > prevSpeed){
        //     float max = prevSpeed + delta*Time.deltaTime;
        //     speed = Math.Min(max, speed);
        // } else if (speed < prevSpeed){
        //     float min = prevSpeed - delta*Time.deltaTime;
        //     speed = Math.Max(min, speed);
        // }

        // if(speed < minSpeed){
        //     ratio = 0;
        // } else if(speed > minSpeed + maxSpeed){
        //     ratio = 1;
        // } else {
        //     ratio = (speed/(minSpeed + maxSpeed));
        // }

        // // //Smooth out transitions using delta
        // // if(ratio > prevRatio){
        // //     float maxRatio = prevRatio + delta*Time.deltaTime;
        // //     ratio = Math.Min(maxRatio, ratio);
        // // } else if (ratio < prevRatio){
        // //     float minRatio = prevRatio - delta*Time.deltaTime;
        // //     ratio = Math.Max(minRatio, ratio);
        // // }

        // float scale = ((maxScale - minScale)*ratio + minScale);

        // //Update the scale of the transform
        // gameObject.transform.localScale = new Vector3(_initScale.x, scale*_initScale.y, _initScale.z);

        // prevRatio = ratio;
        // prevSpeed = speed;

    
}
