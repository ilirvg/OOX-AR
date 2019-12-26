using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameControllerPhoton gameControllerPhoton;
    public ModifiedCloudAnchorController cloudAnchorController;
    public TicTacToeController ticTacToeController;
    public GameMainCanvas settingsUIController;
    public List<GameObject> playerSkinsM;
    public List<GameObject> playerSkinsS;
    public List<GameObject> playerSkinsL;

    private void Start() {
        if (DBManager.isGuest) {
            gameControllerPhoton.Player1 = PlayerGO()[(int)PhotonNetwork.room.CustomProperties["FirstPlayerSkin"]];
            gameControllerPhoton.Player2 = PlayerGO()[(int)PhotonNetwork.room.CustomProperties["SecondPlayerSkin"]];
        }
        if (DBManager.pcTesting)
            gameControllerPhoton.EnterGame();
    }

    private void Update() {
        if (cloudAnchorController.LastPlacedAnchor || cloudAnchorController.LastResolvedAnchor || DBManager.pcTesting || cloudAnchorController.LastDistancePlacedAnchor || cloudAnchorController.SinglePlayerAnchor) {
            for (int i = 0; i < FindObjectOfType<GridSpaceController>().gridSpaceList.Count; i++) {
                if (FindObjectOfType<GridSpaceController>().gridSpaceList[i].name != "GridSpace")
                    FindAndApplyAtributes(i, DBManager.planeType);
            }
        }        
    }

    private static void FindAndApplyAtributes(int i, int planeType) {
        Transform current = FindObjectOfType<GridSpaceController>().gridSpaceList[i].transform.GetChild(0);
        Transform toBe = (planeType == 1) ? FindObjectOfType<VerticalSpaceController>().verticalSpaceList[i].transform : FindObjectOfType<GridSpaceController>().gridSpaceList[i].transform;

        current.localScale = toBe.localScale / 20;
        current.transform.position = toBe.position;
        current.transform.rotation = toBe.rotation;
    }

    public void BoardInstantiateModification(GameObject andyObject) {
        Debug.Log("BoardInstantiateModification1");
        if (!DBManager.pcTesting) {
            Debug.Log("BoardInstantiateModification2");
            if (PhotonNetwork.isMasterClient || DBManager.isSinglePlayer)
                gameControllerPhoton.SetBoardInteractable(true);
            else
                gameControllerPhoton.SetBoardInteractable(false);
        }
        if (DBManager.planeType == 0) {
            Debug.Log("BoardInstantiateModification3");
            andyObject.transform.rotation = Quaternion.Euler(
                GameObject.FindGameObjectWithTag("plane").transform.rotation.x,
                andyObject.transform.rotation.y,
                GameObject.FindGameObjectWithTag("plane").transform.rotation.z
                );
            andyObject.transform.Rotate(0, 180, 0, Space.Self);
            Debug.Log("BoardInstantiateModification4");
        }
        else {
            andyObject.transform.Rotate(0, -45, 0, Space.Self);
        }
    }

    public List<GameObject> PlayerGO() {
        if (DBManager.scale == 0)
            return playerSkinsS;
        else if (DBManager.scale == 1)
            return playerSkinsM;
        else
            return playerSkinsL;
    }

    public IEnumerator OtherPlayer() {
        if (!PhotonNetwork.isMasterClient) {
            yield return PhotonNetwork.player.GetNext().NickName;
            gameControllerPhoton.FirstPlayerName = PhotonNetwork.player.GetNext().NickName;
            gameControllerPhoton.playerTurnText.text = gameControllerPhoton.FirstPlayerName;
            ticTacToeController.PlayerSide = gameControllerPhoton.FirstPlayerName;
            settingsUIController.usernameText.text = DBManager.username;
            cloudAnchorController.costumProperties.Clear();
            cloudAnchorController.costumProperties.Add("PlayerSide", ticTacToeController.PlayerSide);
            PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);
        }
        if (PhotonNetwork.isMasterClient) {
            yield return PhotonNetwork.player.GetNext().NickName;
            gameControllerPhoton.SecondPlayerName = !DBManager.pcTesting ? PhotonNetwork.player.GetNext().NickName : "P2";
        }
    }
}
