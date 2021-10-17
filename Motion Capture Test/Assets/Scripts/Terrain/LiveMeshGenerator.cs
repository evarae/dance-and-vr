using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LiveMeshGenerator : MonoBehaviour
{
    private Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;


	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int octaves;

	[Range(0,1)]
	public float minPersistance;
    public float maxPersistance;

	public float minLacunarity;
    public float maxLacunarity;

    public float minAltitude;
    public float maxAltitude;


	public int seed;

    private Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colours;


    //STUFF FOR SPEED
    [Range (0,100)]
    public float maxSpeed = 1f;

    [Range (0,100)]
    public float minSpeed = 0f;

    [Range (0,100)]
    public float maxAcceleration = 1f;

    [Range (0,100)]
    public float minAcceleration= 0f;

    [Range (0,100)]
    public float maxHeight = 1f;

    [Range (0,100)]
    public float minHeight= 0f;

    public float delta = 0.1f;

    private float heightRatio;
    private float prevHeightRatio;

    private float speedRatio;
    private float prevSpeedRatio;

    private float accelerationRatio;
    private float prevAccelerationRatio;

    public VelocityTracker[] velTrackList;

    public float _flatRad = 5f;
    public float _fullRad = 7f;
    
    //Tracking old values to freeze them
    private float[] lacunarityList;
    private float[] persistanceList;
    private float[] heightList;
    public int frozen = 20;
    public int freezing = 40;
    public int numBlocks = 5;

    private float offset;
    public float period = 0.15f;
    
    // Start is called before the first frame update
    void Start()
    {
        Color c1 = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        Color c2 = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = c1;
        colorKey[0].time = 0.0f;
        colorKey[1].color = c2;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        if(seed == 0){
            seed = (int)System.DateTime.Now.Ticks;
        }

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //initialise lists
        lacunarityList = new float[(mapWidth-1)+1];
        persistanceList = new float[(mapWidth-1)+1];
        heightList = new float[(mapWidth-1)+1];
        offset = 0f;

        for (int i = 0; i<((mapWidth-1)+1);i++){
            lacunarityList[i] = minLacunarity;
            persistanceList[i] = minPersistance;
            heightList[i] = minAltitude;
        }

        StartCoroutine("Grow");
    }

    void Update(){

        float speedSum = 0f;
        float accSum = 0f;
        float heightSum = 0f;

        foreach (var velTrack in velTrackList)
        {
            speedSum = speedSum + velTrack.getSpeed();
            accSum = accSum + velTrack.getAcceleration();
            heightSum = heightSum + velTrack.getHeight();
        }

        float speed = speedSum/(velTrackList.Length); //get average speed
        float acceleration = accSum/(velTrackList.Length); //get average acc
        float height = heightSum/(velTrackList.Length); //get average acc

        speedRatio = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        accelerationRatio = Mathf.InverseLerp(minAcceleration, maxAcceleration, acceleration);
        heightRatio = Mathf.InverseLerp(minHeight, maxHeight, height);

        //Smooth out transitions using delta
        if(speedRatio > prevSpeedRatio){
            float maxRatio = prevSpeedRatio + delta*Time.deltaTime;
            speedRatio = Math.Min(maxRatio, speedRatio);
        } else if (speedRatio < prevSpeedRatio){
            float minRatio = prevSpeedRatio - delta*Time.deltaTime;
            speedRatio = Math.Max(minRatio, speedRatio);
        }

        if(accelerationRatio > prevAccelerationRatio){
            float maxRatio = prevAccelerationRatio + delta*Time.deltaTime;
            accelerationRatio = Math.Min(maxRatio, accelerationRatio);
        } else if (accelerationRatio < prevAccelerationRatio){
            float minRatio = prevAccelerationRatio - delta*Time.deltaTime;
            accelerationRatio = Math.Max(minRatio, accelerationRatio);
        }

        if(heightRatio > prevHeightRatio){
            float maxRatio = prevHeightRatio + delta*Time.deltaTime;
            heightRatio = Math.Min(maxRatio, heightRatio);
        } else if (heightRatio < prevHeightRatio){
            float minRatio = prevHeightRatio - delta*Time.deltaTime;
            heightRatio = Math.Max(minRatio, heightRatio);
        }

        // speedRatio = speedRatio*speedRatio;
        // heightRatio = heightRatio*heightRatio;
        // accelerationRatio = accelerationRatio*accelerationRatio;

        CreateShape();

        UpdateMesh();

        prevSpeedRatio = speedRatio;
        prevAccelerationRatio = accelerationRatio;
        prevHeightRatio = heightRatio;
    }

	public void CreateShape() {
        float speedLac = Mathf.Lerp(minLacunarity, maxLacunarity, speedRatio);
        float accPersistance = Mathf.Lerp(minPersistance, maxPersistance, accelerationRatio);
        float heightScale = Mathf.Lerp(minAltitude, maxAltitude, heightRatio);

        for(int i = (mapWidth - frozen); i < (mapWidth-1); i++){
            lacunarityList[i] = speedLac;
            persistanceList[i] = accPersistance;
            heightList[i] = heightScale;
            //print("i: " + i + ", lacList[i]: " + lacunarityList[i]);
        }
        //print("In CreateShape, 0 lac entry: " + lacunarityList[0] + ", lacunarity: " + speedLac);

		//float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, accPersistance, speedLac, offset);
        float[,] noiseMap = Noise.GenerateNoiseMapComplex (mapWidth, mapHeight, seed, noiseScale, octaves, persistanceList, lacunarityList, offset, frozen, freezing);

        float[,] radiusMap = ChannelFlatten(noiseMap);
        CalculateColours(radiusMap);

		//MeshData meshData = GenerateTerrainMesh(radiusMap, heightScale);
        MeshData meshData = GenerateTerrainMesh(radiusMap, heightList);
        triangles = meshData.triangles;
        vertices = meshData.vertices;
		
	}

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;

        mesh.RecalculateNormals();
    }

	void OnValidate() {
		if (mapWidth < 1) {
			mapWidth = 1;
		}
		if (mapHeight < 1) {
			mapHeight = 1;
		}
		if (minLacunarity < 1) {
			minLacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}

    private float[,] ChannelFlatten(float[,] noiseMap){

        float[,] output = new float[mapWidth,mapHeight];

        for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

                float height = noiseMap[x,y];
                var radDist = Math.Pow(Math.Pow(y-mapHeight/2, 2), 0.5f); //centered around xSize/2, zSize/2

                if(radDist < _flatRad){ //if point is within the flat radius
                    height = 0;
                } else if((_flatRad <= radDist) && (radDist < _fullRad)){ //else if in between flat and full radius, increase to max height with a gradient
                    var scalar = 1 - (_fullRad - radDist)/(_fullRad-_flatRad);
                    height = (float) scalar*(float)scalar*height;
                }

                output[x,y] = height;
            }
        }

        return output;

    }

    private void CalculateColours(float[,] noiseMap){

        //print("Calculating colours...");
        colours = new Color[mapWidth*mapHeight];

        for (int i = 0, y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

                colours[i] = gradient.Evaluate(noiseMap[x,y]);
                //print("Colours i: " + colours[i] + ", noisemap is: " + noiseMap[x,y]);
                i++;
            }
        }
    }

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float[] heightScales) {
        int width = heightMap.GetLength (0);
        int height = heightMap.GetLength (1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData (width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, heightMap [x, y] * heightScales[x], topLeftZ - y);
                meshData.uvs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1) {
                    meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }

    public void drift(){

        //string s = "";

        for(int i = 0; i < (mapWidth-frozen-1); i++){
            lacunarityList[i] = lacunarityList[i+1];
            persistanceList[i] = persistanceList[i+1];
            heightList[i] = heightList[i+1];
            //s += ", i: " + lacunarityList[i];
        }

        heightList[mapWidth-frozen-1] = heightList[mapWidth-frozen];

        int blockSize = (int)((mapWidth-frozen)/numBlocks);
        if((offset % blockSize) == 0){
            lacunarityList[mapWidth-frozen-1] = lacunarityList[mapWidth-frozen];
            persistanceList[mapWidth-frozen-1] = persistanceList[mapWidth-frozen];
        }

        //print(s);

        offset += 1;
        //print("offset: " + offset);
    }

    IEnumerator Grow(){
        while(true){
            drift();
            yield return new WaitForSeconds(period);
        }
    }
}

