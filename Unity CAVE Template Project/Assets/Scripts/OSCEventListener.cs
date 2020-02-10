using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OSCEventListener : MonoBehaviour {
	
	//PointerControl pointerRoot;
	public Transform projectionRoot;

	private static OSCEventListener osc;
	public static OSCEventListener OSC
	{
		get
		{
			if(osc == null)
				osc = GameObject.FindObjectOfType<OSCEventListener>();
			return osc;
		}
	}

	// Use this for initialization
	void Start () 
	{
		OSCHandler.Instance.Init(); //init OSC
		//projectionRoot = GameObject.Find("ProjectionRoot").transform;
		//pointerRoot = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
		DontDestroyOnLoad(gameObject);
	}

	// Update is called once per frame
	void Update () {
 
		/*
		OSCHandler.Instance.UpdateLogs();

		List<UnityOSC.OSCPacket> packets=OSCHandler.Instance.Servers["HeadTracker"].packets;
			
		if (packets.Count>0)
		Debug.Log (packets[0].Address);
			
		*/
		OSCHandler.Instance.UpdateLogs();
		List<string> server_messages = OSCHandler.Instance.Servers["HeadTracker"].log;
		foreach (string msg in server_messages){
			//Debug.Log (msg);
			//parse message and update tracker position
			string[] words = msg.Split(' ');
				

			//convert Vicon coordinates to Unity coordinates
			projectionRoot.localPosition = new Vector3(float.Parse(words[5]),float.Parse(words[7]), float.Parse(words[6]));

			Vector3 pos = projectionRoot.localPosition;
			pos *= 3.28084f;
			//GameObject.FindObjectOfType<Text>().text = pos.ToString();
			//14, 15, 16 = H, P, R
			// rotate X = pitch
			// rotate Y = heading
			// rotate Z = roll
			//if (pointerRoot)
			//	pointerRoot.UpdatePointer (float.Parse(words[11]), float.Parse(words[13]), float.Parse(words[12]), float.Parse(words[15]), float.Parse(words[14]), float.Parse(words[16]));
		}
	}

}
 