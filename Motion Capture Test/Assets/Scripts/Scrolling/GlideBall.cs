using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideBall : MonoBehaviour
{
    public float killTimer = 100f;
    public float freezeTimer = 30f;
    private SinusoidalMovement sine;
    private VelocityColour velCol;

    void Start()
    {
        //GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        sine = gameObject.AddComponent(typeof(SinusoidalMovement)) as SinusoidalMovement;
        sine.distanceScale = 0.1f;
        setKillTimer();
        setFreezeTimer();
    }

    void Update()
    {
        
    }

    public void setFreezeTimer(){
        StartCoroutine(freeze(freezeTimer));
    }

    public void setKillTimer(){
        StartCoroutine(kill(killTimer));
    }

    public void setVelocityColour(VelocityTracker[] velTrackList){
        
        //Add velocity colour
        velCol = GetComponent<VelocityColour>();
        
        if(velCol == null){
            velCol = gameObject.AddComponent(typeof(VelocityColour)) as VelocityColour;
        }

        velCol.velTrackList = velTrackList;
    }

    IEnumerator kill(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }


    IEnumerator freeze(float time){
        yield return new WaitForSeconds(time);
        Destroy(velCol);
        Destroy(sine);
    }


}
