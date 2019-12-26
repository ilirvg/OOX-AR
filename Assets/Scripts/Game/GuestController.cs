using UnityEngine;

public class GuestController : MonoBehaviour {
    public GameController gameController;
    public GameControllerPhoton gameControllerPhoton;

    private int populateIndex;
    private string populateInstantName;

    public void PopulateGuestBoard() {
        for (int i = 0; i < 9; i++) {
            int j = 0;
            foreach (int value in ((int[])PhotonNetwork.room.CustomProperties["Instantiate" + i])) {
                if (j == 0) {
                    if (value == 0)
                        break;
                }
                if (j == 1)
                    populateInstantName = (value == 1) ? gameControllerPhoton.FirstPlayerName : gameControllerPhoton.SecondPlayerName;
                if (j == 2) {
                    populateIndex = value;
                    InstantiateForGuest(populateIndex, populateInstantName);
                }
                j++;
            }
        }
    }

    public void InstantiateForGuest(int index, string name) {
        GameObject obj = (name == gameControllerPhoton.FirstPlayerName) ?
            gameController.PlayerGO()[(int)PhotonNetwork.room.CustomProperties["SecondPlayerSkin"]]:
            gameController.PlayerGO()[(int)PhotonNetwork.room.CustomProperties["FirstPlayerSkin"]];

        GameObject playerObj = Instantiate(
                obj,
                FindObjectOfType<GridSpaceController>().gridSpaceList[index].transform.position,
                Quaternion.identity
                ) as GameObject;
        playerObj.transform.parent = FindObjectOfType<GridSpaceController>().gridSpaceList[index].transform;
        FindObjectOfType<GridSpaceController>().gridSpaceList[index].transform.name = name;
    }
}
