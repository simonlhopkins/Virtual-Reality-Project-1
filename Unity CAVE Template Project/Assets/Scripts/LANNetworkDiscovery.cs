using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LANNetworkDiscovery : NetworkDiscovery {

	public void StartLooking() {
		base.Initialize();
		base.StartAsClient();
	}

	public void StartBroadcast() {
		base.Initialize();
		base.StartAsServer();
	}

	public override void OnReceivedBroadcast(string fromAddress, string data) {
		base.OnReceivedBroadcast(fromAddress, data);

		int index = fromAddress.LastIndexOf(":")+1;
		string ipAddress = fromAddress.Substring(index, fromAddress.Length - index);

		NetworkGUI.ip = ipAddress;
	}

}
