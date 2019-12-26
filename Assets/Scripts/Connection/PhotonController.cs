using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotonController : MonoBehaviour {
    //public static Action disconectPopupAction;

    public string versionName = "0.1";
    public NewRoomController newRoomController;

    private int i = 0;
    private float time = 0.0f;
    private bool inLobby = false;
    public bool InLobby {
        get { return inLobby; }
    }

    private void Awake() {
        if (PhotonNetwork.connectionState == ConnectionState.Connected) {
            PhotonNetwork.Disconnect();
        }
    }

    private void Start() {
        DBManager.inDistance = false;
        DBManager.isGuest = false;
        DBManager.isSinglePlayer = false;
    }

    private void Update() {
        
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected) {
            //if (disconectPopupAction != null && i == 0)
            //    disconectPopupAction();
            PhotonNetwork.ConnectUsingSettings(versionName);
        }

        //TODO Uncoment
        if (newRoomController.OnePlayerConnected) {
            time += Time.deltaTime;
            if (time >= 12.0f) {
                time = 0.0f;
                newRoomController.OnePlayerConnected = false;
                PhotonNetwork.Disconnect();
            }
        }
        if (PhotonNetwork.playerList.Length >= 2 && PhotonNetwork.isMasterClient) {
            GameScene();
        }
    }

    private void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        i = 1;
    }

    private void OnJoinedLobby() {
        inLobby = true;
    }

    private void OnJoinedRoom() {
        if (newRoomController.EnterRoom) {
            if (PhotonNetwork.room.PlayerCount > 2) { // added on 10.7.2019
                DBManager.isGuest = true;
            }
            GameScene();
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        Debug.Log("Player Disconnected " );
    }

    private void GameScene() {
        SceneManager.LoadScene("TicTacToe");
    }
}
