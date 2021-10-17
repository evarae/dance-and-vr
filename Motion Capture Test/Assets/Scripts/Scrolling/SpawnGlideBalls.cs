using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGlideBalls : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GlideBall ballPrefab;
    public float frequency = 4f;
    private Transform tf;
    public float aheadDistance = 10f;
    public float verticleDistance = 2f;
    public VelocityTracker[] velTrackList;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        tf = gameObject.transform;
        StartCoroutine(spawnBalls());

    }

    IEnumerator spawnBalls(){

        while(true){
            GlideBall left = Instantiate(ballPrefab, tf.position + new Vector3(aheadDistance, verticleDistance, 5), Quaternion.identity);
            left.setVelocityColour(velTrackList);
            GlideBall right = Instantiate(ballPrefab, tf.position + new Vector3(aheadDistance, verticleDistance, -5), Quaternion.identity);
            right.setVelocityColour(velTrackList);
            yield return new WaitForSeconds(frequency);
        }
    }
}
