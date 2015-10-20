using UnityEngine;
using System.Collections;

public class TargetMarker : MonoBehaviour {

    public Texture2D m_accurateMarker;
    public Texture2D m_normalMarker;
    public PlayerController m_playerController;

    private Rect m_position;

	// Use this for initialization
	void Start () {
        m_position = new Rect( Screen.width / 2 - 16, Screen.height / 2 - 16, 32, 32 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.DrawTexture( m_position, m_playerController.isAiming ? m_accurateMarker : m_normalMarker );
    }
}
