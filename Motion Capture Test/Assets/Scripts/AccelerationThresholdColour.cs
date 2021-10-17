using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AccelerationThresholdColour : MonoBehaviour
{
    public VelocityTracker[] velTrackList;

    //Setting colours

    [Range (0,1)]
    public float lowEmission = 0;

    public Color lowColour = new Color(0,0,0,0);

    [Range (0,1)]
    public float highEmission = 0;

    public Color highColour = new Color(1,1,1,1);

    //Setting threshold

    public bool greaterThan = true; //if false, triggered when acceleration is below an amount.

    [Range(-20f, 20f)]
    public float threshold = 2f;

    public float delta = 0.1f;

    private Renderer _renderer;
    private float ratio;
    private float prevRatio;

    //Track the flash time
    public float flashTime = 0.3f;
    private float startTime;
    private float endTime;
    private bool isFlashing;

    void Start()
    {
        //Get the Renderer component
        _renderer = gameObject.GetComponent<Renderer>();

        ratio = 0f;
        prevRatio = 0f;

        _renderer.material.SetColor("_BaseColor", lowColour);
    }

    void Update()
    {
        
        float sum = 0f;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getSpeed();
        }

        float acceleration = sum/(velTrackList.Length); //get average speed

        if(isFlashing){
            if(Time.time > endTime){
                isFlashing = false;
                ratio = 0;
            } else {
                ratio = 1 - (Math.Abs((startTime + 0.5f*flashTime) - Time.time))/(0.5f*flashTime);
            }
        } else if ( (greaterThan && acceleration > threshold) || (!greaterThan && acceleration < threshold) ){
            print("reached threshold, acceleration is: " + acceleration );
            flash();
        }

        if(prevRatio != ratio){
            float r = (lowColour.r + (highColour.r - lowColour.r)*ratio);
            float g = (lowColour.g + (highColour.g - lowColour.g)*ratio);
            float b = (lowColour.b + (highColour.b - lowColour.b)*ratio);
            float a = (lowColour.a + (highColour.a - lowColour.a)*ratio);

            Color col = new Color(r,g,b,a);

            float emission = (ratio*(highEmission - lowEmission)) + lowEmission;

            //Call SetColor using the shader property name "_Color"
            _renderer.material.SetColor("_BaseColor",  col);
            Material mymat = _renderer.material;
            mymat.SetColor("_EmissionColor", col*emission);
        }

        prevRatio = ratio;
    }

    public void flash(){
            isFlashing = true;
            startTime = Time.time;
            endTime = startTime + flashTime;
    }

}
