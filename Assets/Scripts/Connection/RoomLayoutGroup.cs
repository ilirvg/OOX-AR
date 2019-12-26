using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour {
    public GameObject roomListingPrefab;
    private List<RoomListing> roomListingsButtons = new List<RoomListing>();

    void OnEnable() {
        RoomList();
    }

    private void OnReceivedRoomListUpdate() {
        RoomList();
    }

    private void RoomList() {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms) {
            Debug.Log("Rooms " + room);
            RoomReceived(room);
        }
        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room) {
        int index = roomListingsButtons.FindIndex(x => x.roomName == room.Name);
        if (index == -1) {
            //GameObject parent = GameObject.FindGameObjectWithTag("RoomLayoutGroup");
            if (room.IsVisible && room.PlayerCount<room.MaxPlayers) {
                GameObject roomListingObj = Instantiate(roomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);
                //roomListingObj.transform.SetParent(parent.transform, false);
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                roomListingsButtons.Add(roomListing);

                index = (roomListingsButtons.Count - 1);
            }
        }
        if (index != -1) {
            RoomListing roomListing = roomListingsButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.updated = true;
        }
    }

    private void RemoveOldRooms() {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in roomListingsButtons) {
            if (!roomListing.updated) {
                removeRooms.Add(roomListing);
            }
            else {
                roomListing.updated = false;
            }
        }

        foreach (RoomListing roomListing in removeRooms) {
            GameObject roomListingObj = roomListing.gameObject;
            roomListingsButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
