using UnityEngine;
using System.Collections;

/// <summary>
/// Entry point for network initialisation: started at the beginning of the game.
/// Join a random room, if no room is created, creates one.
/// This script must be attached to the scene to start. For the moment.
/// Warning! Checkbox "AutoJoin Lobby" must be checked
/// </summary>
public class RandomConnectionMaker : Photon.PunBehaviour{
    public string typePrefabInstanciate = "LeMale";

    /// <summary>
    /// Launch the connection to Photon Server (must be PhotonCloud on PhotonSettings)
    /// </summary>
	void Start () {
        PhotonNetwork.ConnectUsingSettings("v0.1");
	}

    /// <summary>
    /// Lobby == entrance hall
    /// When joined try to join random room. If no one exists => OnPhotonRandomJoinFailed
    /// </summary>
    public override void OnJoinedLobby() {
        Debug.Log("Photon: OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Construct new random room if fail to join random room
    /// </summary>
    /// <param name="codeAndMsg"></param>
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        short errorCode = (short)codeAndMsg[0];
        string errorMsg = codeAndMsg[1].ToString();
        Debug.Log(errorMsg);

        if (errorCode != ExitGames.Client.Photon.ErrorCode.NoRandomMatchFound)
            throw new System.Exception(errorMsg);

        PhotonNetwork.CreateRoom(null);
    }

    /// <summary>
    /// Print to screen Photon states
    /// </summary>
    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    /// <summary>
    /// Create new body instance
    /// </summary>
    public override void OnJoinedRoom() {
        GameObject prefab;

        try {
            prefab = PhotonNetwork.Instantiate(typePrefabInstanciate, Vector3.zero, Quaternion.identity, 0);
        }
        catch (System.Exception e) {
            Debug.Log(e);
            return;
        }
        prefab.GetComponent<PlayerController>().m_isControllable = true;
        prefab.GetComponentInChildren<Camera>().enabled = true;
    }
}
