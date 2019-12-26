using UnityEngine;
using TMPro;
using System;

public class DistanceCanvas : MonoBehaviour {
    public static Action<String> createNewRoomAction;
    public static Action<String> joinRoomAction;

    public NewRoomController newRoomController;
    public TMP_InputField roomName;

    public void CreateDistanceRoom() {
        //newRoomController.CreateRoom(roomName.text);
        DBManager.inDistance = true;//Not sure if I need added on 30.08.2019
        if (createNewRoomAction != null)
            createNewRoomAction(roomName.text);
    }

    public void OnClickJoinRoom(string roomName) {
        //newRoomController.JoinRoom(roomName);
        DBManager.inDistance = true;
        if (joinRoomAction != null)
            joinRoomAction(roomName);
    }

    public void OnClickJoinRoomAsGuest(string roomName) {
        DBManager.isGuest = true;
        DBManager.inDistance = true;
        if (joinRoomAction != null)
            joinRoomAction(roomName);
        //newRoomController.JoinRoom(roomName);
    }
}
