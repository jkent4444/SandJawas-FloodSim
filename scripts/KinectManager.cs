using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Windows.Kinect;
using UnityEngine.UI;
using System;

public class KinectManager : MonoBehaviour {

	public bool Initalised = false;

	private KinectSensor sensor;
	private MultiSourceFrameReader reader;

	public int DepthHeight { get; private set; }
	public int DepthWidth { get; private set; }

    //the amount the mesh is being reduced
	private const int downsample = 4;
	private const double depthScale = 0.1;

	private ushort[] depthData;
    private CircularData circularDataStorage;
    public bool firstLoad;
	private ushort[] rawFrameData;
	private ushort[] zS;
    private ushort[] topoGraphyMap;

    private MapFileManager MapFileManager;

    public Vector2 cullCenter;
	public GameObject meshThing;

	private Meshable meshable;

	//default number of frames to smooth
	public int numFramesToSmooth;

	//initial frame to point at is 0
	private int frameCounter = 0;

    //smoothing the frames in millimeters
    public int accuracy;

    //if cull depth is reached this will be the default value
    public uint cullDefaultHeight = 1200;

    //the max amount of depth before being culled
    public float cullDistance = 200;

	// Use this for initialization
	void Start () {
        MapFileManager = this.GetComponent<MapFileManager>();
        firstLoad = true;
        
		//runText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Initalised) {
			if (reader != null)
			{
				MultiSourceFrame multiframe = reader.AcquireLatestFrame();

				if (multiframe != null)
				{
					DepthFrame frame = multiframe.DepthFrameReference.AcquireFrame();

					if (frame != null) 
                    {
                            frame.CopyFrameDataToArray(rawFrameData);
						    frame.Dispose();
					}
				}

				multiframe = null;
                
				remesh();
			}
		}

	}
    public int getDownSample()
    {
        return downsample;
    }
	public void StartKinect()
	{
		if (Initalised == false) {
			Initalised = true;

			sensor = KinectSensor.GetDefault();

			if (sensor == null)
			{
				Initalised = false;
				return;
			}


			reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth);
			DepthWidth = sensor.DepthFrameSource.FrameDescription.Width;
			DepthHeight = sensor.DepthFrameSource.FrameDescription.Height;
            cullCenter = new Vector2(DepthWidth, 300);
            depthData = new ushort[DepthWidth * DepthHeight];
            rawFrameData = new ushort[DepthWidth * DepthHeight];
            circularDataStorage = new CircularData(numFramesToSmooth, (DepthHeight / downsample) * (DepthWidth / downsample));


			if (!sensor.IsOpen){
				sensor.Open();
			}

			zS = new ushort[(DepthHeight / downsample) * (DepthWidth / downsample)];
            Debug.Log(DepthWidth / downsample + ":" + DepthHeight / downsample);
			meshable = meshThing.GetComponent<Meshable>();
			meshable.CreateMesh(DepthWidth/downsample, DepthHeight/downsample);
		}
	}

	private void remesh()
	{
        uint sum = 0;
        ushort[] temp = new ushort[(DepthHeight / downsample) * (DepthWidth / downsample)];
		for (int y = 0; y < DepthHeight; y+= downsample) {
			for (int x = 0; x < DepthWidth; x+= downsample) {
				int indexX = x/downsample;
				int indexY = y/downsample;
				int smallIndex = (indexY * (DepthWidth / downsample)) + indexX;
                ushort avg = new ushort();

                if (checkWithinCull(x,y))
                {
                   avg = averageDepth(x, y);
                    
                }
                else
                {
                   avg = Convert.ToUInt16(cullDefaultHeight);

                }
                zS[smallIndex] = avg;
			}
		}


        circularDataStorage.addData(zS);
        ushort[] v = circularDataStorage.averageData;
        meshable.setZs(ref v);
       /** if (firstLoad)
        {
            //save the frame to depth data
            ushort[] copy = new ushort[zS.Length];
            Array.Copy(v, copy, v.Length);
            depthData = v;
            firstLoad = false;
        }
        else
        {
            for (int y = 0; y < DepthHeight / downsample ; y++) {
			    for (int x = 0; x < DepthWidth / downsample; x++) {
                    int index = (y * (DepthWidth / downsample)) + x;
                    if (v[index] == 4500)
                    {
                        continue;
                    }
                    if (Mathf.Abs(depthData[index] - v[index]) > accuracy)
                    {

                        depthData[index] = v[index];
            
                    }
			    }
		    }
            meshable.setZs(ref depthData);
        }**/

		
	}

    //checks whether the point is within the cull point
    private bool checkWithinCull(int x, int y)
    {

        int centerX = (int)cullCenter.x / 2;
        int centerY =  (int)cullCenter.y/ 2;
        int newX = x - centerX;
        int neyY = y - centerY;


        if (Mathf.Sqrt(newX * newX + neyY * neyY) <= cullDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	private ushort averageDepth(int x, int y)
	{
		uint sum = 0;

		uint ignore = 0;

        int downSquared = downsample * downsample;

		for (int y1 = y; y1 < y + downsample; y1++) {
			for (int x1 = x; x1 < x + downsample; x1++) {
				int fullIndex = (y1 * DepthWidth) + x1;

                if (rawFrameData[fullIndex] == 0 || rawFrameData[fullIndex] > 4500)
				{
                    sum += 4500;
					ignore++;
				} else {
                    sum += rawFrameData[fullIndex];
				}
			}
		}

        if (ignore > 0 && ignore != downSquared)
        {
            sum = sum - (ignore * 4500);
            return Convert.ToUInt16(sum / (downSquared - ignore));
        }
        else if (ignore == downSquared)
        {
            return Convert.ToUInt16(4500);
		} else {
            return Convert.ToUInt16(sum / downSquared);
		}
	}

    public void takeMeshSnapShot()
    {
        for (int y = 0; y < DepthHeight/downsample; y++)
        {
            for (int x = 0; x < DepthWidth / downsample; x++)
            {
                int index = (y * (DepthWidth / downsample)) + x;
                MapFileManager.addStringToSave(zS[index].ToString());            
            }           
        }
        MapFileManager.saveString();
    }

    public void LoadMeshSnapShot()
    {
        ushort[] temp = MapFileManager.loadFile();
        meshable = meshThing.GetComponent<Meshable>();
		DepthWidth = 128;
		DepthHeight = 106;
        meshable.CreateMesh(128, 106);
        zS = temp;
        meshable.setZs(ref temp);
    }

    public void GenerateTopoGraphyMap()
    {
        ushort[] temp = zS;
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 106; x++)
            {
                int index = (y * (106)) + x;
              
                temp[index] = Convert.ToUInt16(zS[index] - (zS[index] % 20));
               
                
            }
        }
        meshable = meshThing.GetComponent<Meshable>();
        meshable.CreateMesh(128, 106);
        meshable.setZs(ref temp);
    }

	public void initFloodSimulation(){
		this.GetComponent<WaterController> ().initWaterSim (ref zS);
        //this.GetComponent<CameraController>().setInGame(true);
	}
}
