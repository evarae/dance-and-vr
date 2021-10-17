using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidalMovement : MonoBehaviour
{
    private Vector3 _startPosition;

    [Range (0.1f, 10f)]
    public float velScale = 1.0f;

    [Range (0.1f, 10f)]
    public float distanceScale = 1.0f;

    [Range (0.0f, 6.283f)]
    public float timeOffset = 0.0f;
    
    void Start (){
        _startPosition = transform.position;
    }
    
    void FixedUpdate(){
        transform.position = (new Vector3(transform.position.x, _startPosition.y + distanceScale*Mathf.Sin((Time.time + timeOffset)*velScale), transform.position.z));
    }
}
