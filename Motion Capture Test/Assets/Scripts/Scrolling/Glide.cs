using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{
    [Range (0.1f, 15f)]
    public float speed = 1f;

    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        tf.position = tf.position + new Vector3(-speed*Time.deltaTime,0,0);
    }
}
