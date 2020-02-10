using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

	public static string direction = "center";
	public static bool stereo = true;
	public static float interpupillaryDistance = 62 / 1000f;//0.06985f;

    private GameObject instance;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        // Only spawn one player for each client and have it sync with the server
        if (instance == null) {
            instance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, instance, playerControllerId);
        }
    }

}