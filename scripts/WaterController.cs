using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//zones that could be placed on the map
public enum zoneType
{
    Land,
    Forest,
    City,
    Beach,
    Object
}

public struct vertexInfo {
	public int[] verticesLower;
	public int countLower;
	public Vector3 currentPosition;
	public int waterHeight;
    public int objectHeight;
	public ushort groundHeight;

    //0-100 the percentage of water that it will absorb before run offs
    public int solubility;
    public zoneType zone;

}

public class WaterController : MonoBehaviour {
    public vertexInfo[] matPoints;
	private int mapHeight = 126;
	private int mapWidth = 106;
	public Meshable groundMesh;
	public GameObject waterMeshObject;
	public Meshable waterMesh;
	private Vector3[] heightMap;
	public float waterSolubility;

	//Rain simulation information
	public GameObject rainEmitter;
	private Vector2 rainPosition;
	private float rainRadius;
	private float rainEmissionRate;
	public bool Simulating;
	private float timeDelta;
	private bool Live;
	private bool debug;
	public float timeCounter;
	public uint defaultWaterHeight;
	private bool waterMapInit;
	private bool mapInitiliased;

	void Start() {
		Simulating = false;
		Live = false;
		debug = true;
		timeCounter = 0;
		waterMapInit = false;
		mapInitiliased = true;
	}

	void Update() {
		timeCounter += Time.deltaTime;
		rainPosition.x = (rainEmitter.transform.position.x) - groundMesh.transform.position.x;
		rainPosition.y = groundMesh.transform.position.z - (rainEmitter.transform.position.z);
		if (Simulating && !(rainPosition.x < 0 || rainPosition.x > mapWidth) && !(rainPosition.y < 0 || rainPosition.y > mapHeight)) {
			if(Time.deltaTime != 0){
				float currentEmission = (rainEmissionRate/2) / (Time.deltaTime*1000);
				if(debug == true){
					createRain(currentEmission);
					if(waterMapInit) {
						initWaterMesh();
						waterMapInit = false;
						mapInitiliased = true;
					}
					if(mapInitiliased){
						generateWaterZs();
					}
				}

			}
		}
	}
	public void initWaterSim(ref ushort[] zS) {
		waterMapInit = true;
		debug = true;
        mapWidth = this.GetComponent<KinectManager>().DepthWidth / this.GetComponent<KinectManager>().getDownSample();
        mapHeight = this.GetComponent<KinectManager>().DepthHeight / this.GetComponent<KinectManager>().getDownSample(); ;
		heightMap = new  Vector3[mapWidth*mapHeight];
		heightMap = groundMesh.getVerts ();
        matPoints = new vertexInfo[heightMap.Length];
        Debug.Log(mapWidth + ":" + mapHeight);
		for (int x = 0; x < mapWidth; x++) {
			for(int y = 0; y < mapHeight; y++) {
				int fullIndex = (y * mapWidth) + x;
                matPoints[fullIndex].currentPosition = heightMap[fullIndex];
                matPoints[fullIndex].groundHeight = Convert.ToUInt16(1500 - zS[fullIndex]);
                matPoints[fullIndex].objectHeight = 0; 
                matPoints[fullIndex].waterHeight = 0;
                //default everything is land
                matPoints[fullIndex].zone = zoneType.Land;
                matPoints[fullIndex].countLower = 0;
                matPoints[fullIndex].verticesLower = new int[8];
			}
		}
		startWaterSim();
	}

	public void startWaterSim(){

		ParticleSystem rainParticleSystem = rainEmitter.GetComponent<ParticleSystem> ();
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(rainParticleSystem);
		rainRadius = so.FindProperty("ShapeModule.radius").floatValue;
		rainEmissionRate = rainParticleSystem.emissionRate;
		Simulating = true;
	}

	public void createRain(float currentEmission){
		//how much rain is falling on a point every frame
		float rainGauge = currentEmission / (Mathf.PI * rainRadius * rainRadius);
		Vector2 pointWithinRain = UnityEngine.Random.insideUnitCircle * rainRadius;
		pointWithinRain.x = rainPosition.x + pointWithinRain.x;
		pointWithinRain.y = rainPosition.y + pointWithinRain.y;
		if ((pointWithinRain.x < 0 || pointWithinRain.x > mapWidth) && (pointWithinRain.y < 0 || pointWithinRain.y > mapHeight)) {
			pointWithinRain = rainPosition;
		}
		rainAtPoint (pointWithinRain, rainGauge);
	}

	public void rainAtPoint(Vector2 point, float rainGauge) {
		if ((point.x < 0 || point.x > mapWidth) && (point.y < 0 || point.y > mapHeight)) {
			Debug.Log ("something went wrong point out side of map");
			return;
		}

		int fullIndex = ((int)point.y * mapWidth) + (int)point.x;
		int currentPoint = findLowestIndexFromPoint (point);
		int lastPoint = -1;
		while (true) {
			if(lastPoint == -1){
                lastPoint = currentPoint;
                currentPoint = findLowestIndexFromPoint(new Vector2(matPoints[currentPoint].currentPosition.x, -matPoints[currentPoint].currentPosition.y));
			} else {
                lastPoint = currentPoint;
                currentPoint = findLowestIndexFromPoint(new Vector2(matPoints[currentPoint].currentPosition.x, -matPoints[currentPoint].currentPosition.y));
                matPoints[(lastPoint.y * mapWidth) +lastPoint.x].groundHeight;
			}

            if (currentPoint == lastPoint)
            {
				break;
			}
		}
        matPoints[currentPoint].waterHeight += 1;
	}

	public int findLowestIndexFromPoint (Vector2 point) {

		int pointIndex = ((int)point.y * mapWidth) + (int)point.x;
		//Debug.Log (pointIndex);
		uint heightToMatch;
        vertexInfo waterPoint = matPoints[pointIndex];
        uint pointHeight = Convert.ToUInt32((waterPoint.waterHeight) + waterPoint.groundHeight + waterPoint.objectHeight);
		uint lowestHeight = pointHeight; 
		waterPoint.countLower = 0;
		waterPoint.verticesLower[0] = pointIndex;
		//Debug.Log ("top left height:" + lowestHeight);
		for (int i = 0; i < 8; i++) {
			int index = pointIndex;
			switch (i) {
			case 0:
				//top left
				if (index < mapWidth || point.x == 0) {
					break;
				} else {
					index += -mapWidth - 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.verticesLower[0] = index;
					}
				}
				break;
			case 1:
				//top mid
				if (index < mapWidth) {
					break;
				} else {
					index += -mapWidth;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 2:
				//top right
				if (index < mapWidth || point.x == mapWidth) {
					break;
				} else {
					index += -mapWidth + 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 3:
				//left
				if (point.x == 0) {
					break;
				} else {
					index += - 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 4:
				//right
				if (point.x == mapWidth) {
					break;
				} else {
					index += 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 5:
				//bottom left
				if (index > mapWidth * mapHeight || point.x == 0) {
					break;
				} else {
					index += mapWidth - 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 6:
				//bottom mid
				if (index > mapWidth * mapHeight || point.x == 0) {
					break;
				} else {
					index += mapWidth;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			case 7:
				//bottom right
				if (index > mapWidth * mapHeight || point.x == 0) {
					break;
				} else {
					index += mapWidth + 1;
                    heightToMatch = Convert.ToUInt32((matPoints[index].waterHeight) + matPoints[index].groundHeight + matPoints[index].objectHeight);
					if (heightToMatch < lowestHeight) {
						waterPoint.verticesLower = new int[8];
						waterPoint.countLower = 0;
						waterPoint.verticesLower[0] = index;
						lowestHeight =  heightToMatch;
					} else if(heightToMatch == lowestHeight && heightToMatch != pointHeight) {
						waterPoint.countLower ++;
						waterPoint.verticesLower[waterPoint.countLower] = index;
					}
				}
				break;
			default:
				Debug.Log ("something went horribly wrong");
				break;
			}
		}
		if (waterPoint.countLower == 0) {
			return waterPoint.verticesLower[0];
		} else {
			return waterPoint.verticesLower[UnityEngine.Random.Range(0,waterPoint.countLower)];
		}
	}

	public uint getLowestHeight(uint lowestHeight, uint heightToMatch, int heightToMatchIndex, int pointIndex) {
		if (heightToMatch < lowestHeight) {
			return heightToMatch;
		} 
		return lowestHeight;
	}

	public void initWaterMesh(){
		waterMesh = waterMeshObject.GetComponent<Meshable>();
		waterMesh.CreateMesh(mapWidth, mapHeight);
	}

	public void generateWaterZs(){
		ushort[] temp = new ushort[mapWidth*mapHeight];

		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				int fullIndex = (y* mapWidth) +x;
				temp[fullIndex] = Convert.ToUInt16((matPoints[fullIndex].waterHeight + matPoints[fullIndex].groundHeight + matPoints[fullIndex].objectHeight));
			}
		}
		waterMesh.setWaterZs(ref temp);
	}

    public void updateObjectHeightAtPoint(int fullIndexToPosition, int height)
    {
        matPoints[fullIndexToPosition].objectHeight = height;
    }

    public int getDeltaOfWaterHeight(int initialPos, int finalPos)
    {
        int initHeight = matPoints[initialPos].waterHeight;
        int finalHeight = matPoints[finalPos].waterHeight;
        if (initialPos == finalPos || initHeight == finalHeight || finalHeight > initHeight)
        {
            return 0;
        }
        else
        {
            return (int)(finalHeight - initHeight) / 2;
            
        }
    }

    public void addZone(Zone zoneToUpdate)
    {
        //change all values from mat to be of type zone from zones position
    }

    //removes zone with radius
    public void removeZone(zoneType type, Vector3 position, int radius)
    {
        //change all the values from mat to be of type default
        //iterate from point - radius from width and height to have the start point to look at then filter radius from there
    }

    

}
