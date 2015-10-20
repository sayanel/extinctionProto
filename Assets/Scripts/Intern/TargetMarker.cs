using UnityEngine;
using System.Collections;

public class TargetMarker : MonoBehaviour {

    public Texture2D accurateMarker;
    public Texture2D normalMarker;
    public PlayerController playerController;

    private Rect position;

	// Use this for initialization
	void Start () {
        position = new Rect( Screen.width / 2 - 16, Screen.height / 2 - 16, 32, 32 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.DrawTexture( position, playerController.isAiming ? accurateMarker : normalMarker );
    }
}
