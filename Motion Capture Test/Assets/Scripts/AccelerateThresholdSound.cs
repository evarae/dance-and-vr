using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateThresholdSound : MonoBehaviour
{

    public AccelerationThresholdColour threshCol;
    
    public VelocityTracker[] velTrackList;
    public string[] soundNames;

    public bool greaterThan = true; //if false, triggered when acceleration is below an amount

    [Range(-20f, 20f)]
    public float threshold = 2f;

    private AudioManager am;
    private string playing;    

    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update(){

        float sum = 0f;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getAcceleration();
        }
        
        float acceleration = sum/(velTrackList.Length);

        bool isPlaying = am.isPlaying(playing);

        if (((greaterThan && acceleration > threshold) || (!greaterThan && acceleration < threshold)) && isPlaying == false ){
            
            int randomIndex = Random.Range(0, soundNames.Length);
            playing = soundNames[randomIndex];
            
            am.Play(playing);
            if(threshCol != null){
                threshCol.flash();
            }

            print("reached threshold, acceleration is: " + acceleration + ", playing: " + playing);
        }

    }
}
