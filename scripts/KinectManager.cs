using UnityEngine;
using System.Collections.Generic;
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

	private ushort[] depthData;
	private ushort[] zS;

	public GameObject meshThing;

	private Meshable meshable;

	// Use this for initialization
	void Start () {
	
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
						frame.CopyFrameDataToArray(depthData);
						frame.Dispose();
						depthProcessed = true;

						Debug.Log (depthData[10000]);
					}
				}

				multiframe = null;
				remesh();
			}
		}

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

			depthData = new ushort[DepthWidth * DepthHeight];

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

				if (depthData[fullIndex] == 0 || depthData[fullIndex] > 4500)
				{
					sum += 4500;
					ignore++;
				} else {
					sum += depthData[fullIndex];
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
