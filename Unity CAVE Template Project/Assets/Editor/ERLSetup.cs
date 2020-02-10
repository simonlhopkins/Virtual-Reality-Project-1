using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class ERLSetup {

	private static string FILEPATH = "/erldata.json";

	[System.Serializable]
	public class Wall {
		public Vector3[] wall;
		public Wall() {
			this.wall = new Vector3[3];
		}
	}

	[System.Serializable]
	public class ERLData {
		public List<Wall> walls;

		public ERLData() {
			this.walls = new List<Wall>();
		}
	}

	private static ERLData data;

	public static ERLData GetData() {
		if (data == null)
			LoadData();
		return data;
	}

	[MenuItem("ERL/Edit Walls")]
	public static void SetupWalls() {
		if (data == null)
			LoadData();
		
		EditWallsPopup window = EditorWindow.GetWindow<EditWallsPopup>();

		window.titleContent.text = "Edit Walls";
		window.position = new Rect(Screen.width/2, Screen.height/2, 250, 400);

		window.Focus();
		window.ShowPopup();
	}

	private static void LoadData() {
		string path = Application.dataPath + FILEPATH;

		if (File.Exists(path)) {
			string text = File.ReadAllText(path);
			data = JsonUtility.FromJson<ERLData>(text);
		} else {
			data = new ERLData();
		}
	}

	private static void SaveData() {
		string text = JsonUtility.ToJson(data);
		string path = Application.dataPath + FILEPATH;
		File.WriteAllText(path, text);
	}

	class EditWallsPopup : EditorWindow {
		private Vector2 scrollPos = new Vector2();

		void OnGUI() {
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			int numWalls = EditorGUILayout.IntField("Number of Walls", data.walls.Count);
			numWalls = Mathf.Clamp(numWalls, 1, 10);
			while (data.walls.Count < numWalls)
				data.walls.Add(new Wall());
			while (data.walls.Count > numWalls)
				data.walls.RemoveAt(data.walls.Count-1);

			GUILayout.Space(20);

			for (int i = 0; i < numWalls; i++) {
				EditorGUILayout.LabelField("Wall " + i + ": ");
				data.walls[i].wall[0] = EditorGUILayout.Vector3Field("bottom left", data.walls[i].wall[0]);
				data.walls[i].wall[1] = EditorGUILayout.Vector3Field("bottom right", data.walls[i].wall[1]);
				data.walls[i].wall[2] = EditorGUILayout.Vector3Field("top left", data.walls[i].wall[2]);
				GUILayout.Space(10);
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Close")) {
				SaveData();
				this.Close();
			}

			EditorGUILayout.EndScrollView();
		}
	}

}
