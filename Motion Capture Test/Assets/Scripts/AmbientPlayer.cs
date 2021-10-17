using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmbientPlayer : MonoBehaviour
{

    

    [Range(0f,1f)]
    public float volume = 1f;

    public int detail = 500;
    public float delta = 0.01f;

    public VelocityTracker[] velTrackList;
    public string slow;
    public string fast;
    public string constant;
    
    private float slowVol;
    private float fastVol;
    private float constVol;

    private float[] values;
    private bool increasing;
    private AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        slowVol = 1;
        fastVol = 0;
        constVol = volume/2;

        increasing = true;

        values = new float[detail];
        for (int i = 0; i < values.Length; i++){
            values[i] = 0;
        }

        am = FindObjectOfType<AudioManager>();
        am.SetVolume(slow, slowVol);
        am.SetVolume(fast, fastVol);
        am.SetVolume(constant, constVol);
        am.Play(slow);
        am.Play(fast);
        am.Play(constant);
    }

    void FixedUpdate()
    {
        float sum = 0f;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getSpeed();
        }

        float speed = sum/(velTrackList.Length); //get average speed

        float oldSum = 0;
        float newSum = 0;

        for(int i =0; i < values.Length/2; i++){
            oldSum += values[i];
            values[i] = values[i+1];
        }
        for(int i = values.Length/2; i < values.Length-1; i++){
            newSum += values[i];
            values[i] = values[i+1];
        }
        values[values.Length -1] = speed;

        increasing = (newSum > oldSum);
        

        if(increasing){
            slowVol = Math.Max(slowVol - delta*Time.deltaTime, 0);
            fastVol = Math.Min(fastVol + delta*Time.deltaTime, 1);
            
        } else if(!increasing){
            slowVol = Math.Min(slowVol + delta*Time.deltaTime, 1);
            fastVol = Math.Max(fastVol - delta*Time.deltaTime, 0);
        }

        am.SetVolume(slow, slowVol*volume);
        am.SetVolume(fast, fastVol*volume);
        print("increasing: " + increasing + ", slow: " + slowVol + " fast: " + fastVol);
    }
}
