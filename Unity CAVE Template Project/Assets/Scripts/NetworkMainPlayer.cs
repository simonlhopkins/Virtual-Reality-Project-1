using UnityEngine;
using UnityEngine.Networking;

public class NetworkMainPlayer : NetworkBehaviour {

    public void Start() {
        // Since only one player syncs with the clients and is controlled by the
        //   server - disabling control for non local players makes it so only the
        //   server can control the character.
        if (!isLocalPlayer)
            this.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

        // Enable camera at given direction
		GameObject parent = GameObject.FindWithTag("camera_" + CustomNetworkManager.direction);

		// Disable everything first
		for (int i = 0; i < parent.transform.parent.childCount; i++)
			parent.transform.parent.GetChild(i).gameObject.SetActive(false);

		parent.SetActive(true);

		Camera[] cameras = parent.transform.GetComponentsInChildren<Camera>();

		for (int i = 0; i < cameras.Length; i++) {
			cameras[i].enabled = true;
			if (i+1 < Display.displays.Length)
				Display.displays[i+1].Activate();
		}
    }

}
