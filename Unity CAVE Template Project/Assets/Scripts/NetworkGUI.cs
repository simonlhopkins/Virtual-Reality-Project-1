using UnityEngine;
using UnityEngine.Networking;

public class NetworkGUI : MonoBehaviour {

    public NetworkManager manager;
	public LANNetworkDiscovery discovery;

	public static string ip = "localhost";

    private bool started = false;

    public void OnGUI() {
		int dy = 0;
        if (!started) {
            if (GUI.Button(new Rect(10, 10+dy, 150, 20), "Start server as center")) {
                CustomNetworkManager.direction = "center";
                manager.networkAddress = ip;
                manager.StartHost();
                started = true;
            }
            if (GUI.Button(new Rect(10, 30+dy, 150, 20), "Listen as left")) {
				CustomNetworkManager.direction = "left";
				manager.networkAddress = ip;
                manager.StartClient();
                started = true;
            }
            if (GUI.Button(new Rect(10, 50+dy, 150, 20), "Listen as right")) {
				CustomNetworkManager.direction = "right";
				manager.networkAddress = ip;
                manager.StartClient();
                started = true;
			}
			if (GUI.Button(new Rect(10, 110+dy, 150, 20), "Begin LAN Broadcast")) {
				discovery.StartBroadcast();
			}
			if (GUI.Button(new Rect(10, 130+dy, 150, 20), "Begin LAN Search")) {
				discovery.StartLooking();
			}

            ip = GUI.TextField(new Rect(10, 70+dy, 150, 20), ip, 20);
        }
    }

}
