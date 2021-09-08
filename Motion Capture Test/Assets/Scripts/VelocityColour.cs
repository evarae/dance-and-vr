using System;
using UnityEngine;

public class VelocityColour : MonoBehaviour
{

    public VelocityTracker[] velTrackList;

    public Color lowColour = new Color(0,0,0,0);
    public Color highColour = new Color(1,1,1,1);

    [Range (0,100)]
    public float maxSpeed = 10f;

    [Range (0,100)]
    public float minSpeed = 0f;

    public float delta = 0.1f;

    private Renderer _renderer;
    private float ratio;
    private float prevRatio;

    void Start()
    {
        //Get the Renderer component
        _renderer = gameObject.GetComponent<Renderer>();

        ratio = 0f;
        prevRatio = 0f;

        _renderer.material.SetColor("_Color", lowColour);
    }

    void Update()
    {
        float sum = 0f;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getSpeed();
        }

        float speed = sum/(velTrackList.Length); //get average speed

        if(speed < minSpeed){
            ratio = 0;
        } else if(speed > minSpeed + maxSpeed){
            ratio = 1;
        } else {
            ratio = (speed/(minSpeed + maxSpeed));
        }

        //Smooth out transitions using delta
        if(ratio > prevRatio){
            float maxRatio = prevRatio + delta*Time.deltaTime;
            ratio = Math.Min(maxRatio, ratio);
        } else if (ratio < prevRatio){
            float minRatio = prevRatio - delta*Time.deltaTime;
            ratio = Math.Max(minRatio, ratio);
        }

        float r = (lowColour.r + (highColour.r - lowColour.r)*ratio);
        float g = (lowColour.g + (highColour.g - lowColour.g)*ratio);
        float b = (lowColour.b + (highColour.b - lowColour.b)*ratio);
        float intensity = (lowColour.a + (highColour.a - lowColour.a)*ratio);

        Color col = new Color(r,g,b,intensity);

        //Call SetColor using the shader property name "_Color"
        _renderer.material.SetColor("_Color",  col);

        prevRatio = ratio;

    }

}
