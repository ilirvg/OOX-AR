using UnityEngine;

public class NewRoomController : MonoBehaviour {
    private ExitGames.Client.Photon.Hashtable costumProperties = new ExitGames.Client.Photon.Hashtable();
    private int[] array = new int[] { 0, 0, 0 };
    public bool OnePlayerConnected { get; set; } = false;
    public bool EnterRoom { get; private set; } = true;

    private void OnEnable() {
        DistanceCanvas.createNewRoomAction += CreateRoom;
        DistanceCanvas.joinRoomAction += JoinRoom;
    }

    private void OnDisable() {
        DistanceCanvas.createNewRoomAction -= CreateRoom;
        DistanceCanvas.joinRoomAction -= JoinRoom;
    }

    private RoomOptions ModifiedRoomOptions() {
        RoomOptions myRoomOptions = new RoomOptions();
        myRoomOptions.MaxPlayers = 5;

        costumProperties.Clear();
        costumProperties.Add("CloudID", "");
        costumProperties.Add("BoardColor", 0);
        costumProperties.Add("PlaneType", 0);
        costumProperties.Add("PlayerSide", "");
        costumProperties.Add("FirstPlayerSkin", 0);
        costumProperties.Add("SecondPlayerSkin", 0);
        costumProperties.Add("Instantiate0", array);
        costumProperties.Add("Instantiate1", array);
        costumProperties.Add("Instantiate2", array);
        costumProperties.Add("Instantiate3", array);
        costumProperties.Add("Instantiate4", array);
        costumProperties.Add("Instantiate5", array);
        costumProperties.Add("Instantiate6", array);
        costumProperties.Add("Instantiate7", array);
        costumProperties.Add("Instantiate8", array);
        myRoomOptions.CustomRoomProperties = costumProperties;
        return myRoomOptions;
    }
    
    public void CreateRoom(string roomName) {
        RoomOptions myRoomOptions = ModifiedRoomOptions();
        PhotonNetwork.CreateRoom(roomName, myRoomOptions, TypedLobby.Default);
        DBManager.roomName = roomName;
        OnePlayerConnected = true;
    }

    public void QrCreateRoom(string roomName) {
        RoomOptions myRoomOptions = ModifiedRoomOptions();
        PhotonNetwork.CreateRoom(roomName, myRoomOptions, TypedLobby.Default);
        DBManager.roomName = roomName;
        EnterRoom = false;
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
        DBManager.roomName = roomName;
        OnePlayerConnected = true;
    }
}
