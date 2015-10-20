﻿using UnityEngine;
using System.Collections;


/// <summary>
/// Used for infos to send and to receive to others players: PhotonView observes on this script
/// Attached on a player(prefab)
/// </summary>
public class NetworkCharacter : Photon.MonoBehaviour {

    Vector3 m_correctPlayerPos; //< smooth
    Quaternion m_correctPlayerRot; //< smooth
    PlayerController m_playerController;
    CharacterState m_previousState;

    void Start() {
        m_playerController = GetComponent<PlayerController>();
        m_previousState = m_playerController.state;
    }

    /// <summary>
    /// Called each frame: do smooth stuff for network player
    /// !isMine: do only correction for remote players
    /// </summary>
    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, m_correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);
        }
    }

    /// <summary>
    /// While a script is observed, PhotonView calls regularly this method:
    /// Aim: create the info we want to pass to others and to handle incoming info (depending on who created PhotonView)
    /// </summary>
    /// <param name="stream">
    /// stream.isWriting tells us if we need to write or read info.
    /// First let's send the rotation and position info
    /// </param>
    /// <param name="info"></param>
    /// 
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // we handle this character: send to others the transform data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            // send to network only if state has changed
            if(!m_playerController.state.Equals(m_previousState)) {
                m_previousState = m_playerController.state;
                stream.SendNext((int)m_previousState);
            }
        }
        else {
            // Network player, receive data (this object viewed into other windows game over network)
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_playerController.state = (CharacterState)stream.ReceiveNext();
        }
    }
}
