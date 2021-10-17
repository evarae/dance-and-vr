using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SineMeshGeneratorAmp : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    [Range (0.01f, 20f)]
    public float frequencyMod = 1f;

    [Range (0.01f, 5f)]
    public float wavelengthMod = 1f;

    [Range (0.01f, 10f)]
    public float gridSize = 1f;

    [Range (0f, 10f)]
    public float _flatRad = 4f; //There is no height change within the flat radius

    [Range (0f, 20f)]
    public float _fullRad = 7f; //Between the flat rad and full rad, the height of the sin wave will increase to it's regular maximum

    //Stuff for changing the amplitude according to velocity:
    public VelocityTracker[] velTrackList;
    [Range (0,100)]
    public float maxSpeed = 2f;

    [Range (0,100)]
    public float minSpeed = 0f;

    public float delta = 0.1f;

    [Range (0,5)]
    public float maxAmp = 1f;

    [Range (0,1)]
    public float minAmp = 0.2f;

    private float amplitude;
    private float prevAmplitude;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        amplitude = minAmp;
        prevAmplitude = minAmp;

    }

    void Update(){
        CreateShape();
        UpdateMesh();
    }

    void CreateShape(){
        vertices = new Vector3[(xSize + 1)*(zSize + 1)];

        //Calculate the amplitude mod

        float sum = 0;
        foreach (var velTrack in velTrackList)
        {
            sum = sum + velTrack.getSpeed();
        }

        float speed = sum/(velTrackList.Length); //get average speed

        if(speed < minSpeed){
            amplitude = minAmp;
        } else if(speed > minSpeed + maxSpeed){
            amplitude = maxAmp;
        } else {
            amplitude = (speed/(minSpeed + maxSpeed))*(maxAmp-minAmp) + minAmp;
        }

        //Smooth out transitions using deltaVolume
        if(amplitude > prevAmplitude){
            float max = prevAmplitude + delta*Time.deltaTime;
            amplitude = Math.Min(max, amplitude);
        } else if (amplitude < prevAmplitude){
            float min = prevAmplitude - delta*Time.deltaTime;
            amplitude = Math.Max(min, amplitude);
        }

        //Generate vertices

        for(int i = 0, z = 0; z <= zSize; z++){
            for(int x = 0; x <= xSize; x++){

                var radDist = Math.Pow((Math.Pow(x-xSize/2, 2) + Math.Pow(z-zSize/2,2)), 0.5f); //centered around xSize/2, zSize/2
                var height = amplitude*Math.Sin(frequencyMod*Time.time + radDist/wavelengthMod);

                if(radDist < _flatRad){ //if point is within the flat radius
                    height = 0;
                } else if((_flatRad <= radDist) && (radDist < _fullRad)){ //else if in between flat and full radius, increase to max height with a gradient
                    var scalar = 1 - (_fullRad - radDist)/(_fullRad-_flatRad);
                    height = scalar*height;
                }

                vertices[i] = new Vector3(x*gridSize,(float)height*gridSize,z*gridSize);
                i++;
            }
        }

        generateTriangles();
        prevAmplitude = amplitude;
    }

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    // private void OnDrawGizmos(){

    //     if(vertices == null){
    //         return;
    //     }

    //     for(int i =0; i < vertices.Length; i++){
    //         Gizmos.DrawSphere(vertices[i], .1f);
    //     }
    // }

    private void generateTriangles(){
        int vert = 0;
        int tris = 0;

        triangles = new int[xSize*zSize*6];

        for(int z = 0; z < zSize; z++){
            for(int x = 0; x < xSize; x++){

                //Create a quad of two triangles

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
}

