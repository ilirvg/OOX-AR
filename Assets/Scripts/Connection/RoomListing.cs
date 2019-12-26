using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomListing : MonoBehaviour {

    public TextMeshProUGUI roomNameText;
    public string roomName;
    public bool updated;
    public Button joinButton;
    public Button guestButton;

    private void Start() {
        DistanceCanvas distanceCanvas = FindObjectOfType<DistanceCanvas>();
        joinButton.onClick.AddListener(() => distanceCanvas.OnClickJoinRoom(roomNameText.text));
        guestButton.onClick.AddListener(() => distanceCanvas.OnClickJoinRoomAsGuest(roomNameText.text));
    }

    private void OnDestroy() {
        joinButton.onClick.RemoveAllListeners();
        guestButton.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text) {
        roomName = text;
        roomNameText.text = roomName;
    }
}
