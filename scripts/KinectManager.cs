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

	private const int downsample = 4;
	private const double depthScale = 0.1;

	private ushort[][] depthData;
	private ushort[] smoothedData;
	private ushort[] zS;


	public GameObject meshThing;

	private Meshable meshable;

	//default number of frames to smooth
	public int numFramesToSmooth = 3;

	//initial frame to point at is 0
	private int framePointer = 0;


	// Use this for initialization
	void Start () {
		depthData = new ushort[numFramesToSmooth][];
		//runText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Initalised) {
			bool depthProcessed = false;

			if (reader != null)
			{
				MultiSourceFrame multiframe = reader.AcquireLatestFrame();
				if (multiframe != null)
				{
					DepthFrame frame = multiframe.DepthFrameReference.AcquireFrame();
					if (frame != null) {
						if(framePointer >= numFramesToSmooth) {
							//run the reset and check script here

							framePointer = 0;
						} else {

							frame.CopyFrameDataToArray(depthData[framePointer]);

							//point to the next frame
							framePointer++;
						}

						//save the next three frames

						frame.Dispose();
						depthProcessed = true;

					}
				}

				multiframe = null;
				remesh();
			}
		}

	}

	/***
	 * Returns a short array that is smoothed to a certain accuracy. Accuracy is in millimeters.
	 * Assumes inputs width and height are consistent across data set. 
	 ***/
	public ushort[] smoothRawInput(ushort[][] input, int accuracy) {
		ushort[] temp;
		ushort shortest;
		temp = new ushort[input [0].Length];

		//i is ushort co-ordinate
		for(int i = 0; i < input[0].Length; i++) {
			//u is frame to check
			shortest = new ushort();
			for(int u = 0; u < numFramesToSmooth; u++) {
				//k is a parrallel frame to compare to
				for(int k = 0; k < numFramesToSmooth; k++) {

					if(k != u && input[u][i] <= input[k][i]+accuracy && input[u][i] >= input[k][i]-accuracy){
						//take the shortest of the accurate ones (might not work diserably 
						if(shortest <= 0 || input[u][i] <= shortest) {

							shortest = input[u][i];
						}
						break;
					}
				}
			}
			Debug.Log(shortest);
			temp[i] = shortest;
		}
		return temp;
	}

	//just a test to check the smoothing
	public void runText(){
		ushort[][] test = new ushort[3][];
		ushort[] temp = new ushort[50];
		int accuracy = 7;
		for (int j = 0; j < 3; j ++) {
			test[j] = new ushort[50];
			for (int i = 0; i < 50; i ++) {
				test[j][i] = (ushort)UnityEngine.Random.Range(0,10);
			}
		}

		test [0] [49] = 0;
		test [1] [49] = 100;
		temp = smoothRawInput(test, accuracy);
		//for (int i = 0; i < 50; i ++) {
		//	Debug.Log(temp[i]);
		//}
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
			for(int i = 0; i < numFramesToSmooth; i++) {
				depthData[i] = new ushort[DepthWidth * DepthHeight];
			}
			smoothedData = new ushort[DepthWidth * DepthHeight];



			if (!sensor.IsOpen){
				sensor.Open();
			}

			zS = new ushort[(DepthHeight / downsample) * (DepthWidth / downsample)];

			meshable = meshThing.GetComponent<Meshable>();
			meshable.CreateMesh(DepthWidth/downsample, DepthHeight/downsample);
		}
	}

	private void remesh()
	{
		for (int y = 0; y < DepthHeight; y+= downsample) {
			for (int x = 0; x < DepthWidth; x+= downsample) {
				int indexX = x/downsample;
				int indexY = y/downsample;
				int smallIndex = (indexY * (DepthWidth / downsample)) + indexX;

				ushort avg = averageDepth(x,y);

				zS[smallIndex] = avg;
			}
		}

		/*for (int y = 1; y < DepthHeight / downsample -1; y++) {
			for (int x = 1; x < DepthWidth / downsample - 1; x++) {

			}
		}*/

		meshable.setZs (ref zS);
	}

	private ushort averageDepth(int x, int y)
	{
		uint sum = 0;

		uint ignore = 0;

		for (int y1 = y; y1 < y + 4; y1++) {
			for (int x1 = x; x1 < x + 4; x1++) {
				int fullIndex = (y1 * DepthWidth) + x1;

				if (smoothedData[fullIndex] == 0 || smoothedData[fullIndex] > 4500)
				{
					sum += 4500;
					ignore++;
				} else {
					sum += smoothedData[fullIndex];
				}
			}
		}

		if (ignore > 0 && ignore != 16) {
			sum = sum - (ignore * 4500);
			return Convert.ToUInt16 (sum / (16 - ignore));
		} else if (ignore == 16) {
			return 4500;
		} else {
			return Convert.ToUInt16(sum /16);
		}
	}
}
