using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameControllerPhoton : Photon.MonoBehaviour {

    [Header("Scripts")]
    public ArPrefabController arPrefabController;
    public GameController gameController;
    public GameMainCanvas gameMainCanvas;
    public ScoreController scoreConroller;
    public ModifiedCloudAnchorController cloudAnchorController;
    public TicTacToeController ticTacToeController;

    [Header("Others")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI gameOverText;
    public Button[] skinButtons;
    public List<Material> boardSkins;

    public GameObject Player1 { get; set; }
    public GameObject Player2 { get; set; }
    public string FirstPlayerName { get; set; }
    public string SecondPlayerName { get; set; }

    private bool isBombActivated = false;

    private int[] emptyArray = new int[] { 0, 0, 0 };

    [PunRPC]
    public void ChangeBoardColor(int i) {
        arPrefabController.RandomNum = i;
        arPrefabController.ArPrefab("S", 0).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
        arPrefabController.ArPrefab("M", 0).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
        arPrefabController.ArPrefab("L", 0).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
        arPrefabController.ArPrefab("S", 1).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
        arPrefabController.ArPrefab("M", 1).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
        arPrefabController.ArPrefab("L", 1).GetComponentInChildren<MeshRenderer>().material = boardSkins[i];
    }

    [PunRPC]
    public void EnterGame() {
        if (DBManager.isSinglePlayer) {
            FirstPlayerName = DBManager.username;
            SecondPlayerName = "AI";
            playerTurnText.text = "Your";
            ticTacToeController.PlayerSide = FirstPlayerName;
            gameMainCanvas.usernameText.text = DBManager.username;
            return;
        }
        if (DBManager.isGuest) {
            FirstPlayerName = PhotonNetwork.playerList[0].NickName;
            SecondPlayerName = PhotonNetwork.playerList[1].NickName;
            ticTacToeController.PlayerSide = PhotonNetwork.room.CustomProperties["PlayerSide"].ToString();
            playerTurnText.text = ticTacToeController.PlayerSide;
        }
        if (!PhotonNetwork.isMasterClient && !DBManager.isGuest) {
            PhotonNetwork.player.NickName = DBManager.username;
            SecondPlayerName = DBManager.username;
            StartCoroutine(gameController.OtherPlayer());
        }
        else if (PhotonNetwork.isMasterClient && !DBManager.isGuest) {
            PhotonNetwork.player.NickName = DBManager.username;
            FirstPlayerName = DBManager.username;
            StartCoroutine(gameController.OtherPlayer());
            playerTurnText.text = "Your";
            ticTacToeController.PlayerSide = FirstPlayerName;
            gameMainCanvas.usernameText.text = DBManager.username;
        }
        
    }

    [PunRPC]
    public void RestartRPC() {
        gameOverPanel.SetActive(false);
        DBManager.isSkinChoosen = false;
        ticTacToeController.Draw = false;
        ticTacToeController.BoombUsed = false;
        ticTacToeController.PopulateSinglePlayerGridList();
        ticTacToeController.BombImg.color = new Color32(250, 255, 255, 255);

        if (!DBManager.isSinglePlayer)
            photonView.RPC("ChangeSide", PhotonTargets.All);
        else
            ticTacToeController.ChangeSideAI();

        foreach (Button b in skinButtons) {
            b.interactable = true;
        }
        if (!DBManager.isGuest) {
            gameMainCanvas.chooseSkin.SetActive(true);
        }
        for (int i = 0; i < FindObjectOfType<GridSpaceController>().gridSpaceList.Count; i++) {
            GameObject gridSpaceObject = FindObjectOfType<GridSpaceController>().gridSpaceList[i];
            if (gridSpaceObject.name != "GridSpace") {
                Destroy(gridSpaceObject.transform.GetChild(0).transform.gameObject);
                gridSpaceObject.name = "GridSpace";
            }
        }

        if (ticTacToeController.PlayerSide == PhotonNetwork.player.NickName || ticTacToeController.PlayerSide == DBManager.username && DBManager.isSinglePlayer)
            SetBoardInteractable(true);
        if (!DBManager.isSinglePlayer) {
            cloudAnchorController.costumProperties.Clear();
            for (int i = 0; i < 9; i++)
                cloudAnchorController.costumProperties.Add("Instantiate" + i, emptyArray);
            PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);
        }
        ticTacToeController.GameEnd = false;
    }

    [PunRPC]
    public void ChoosePlayerSkin(int i, string username) {
        string propertie = null;
        if (username == FirstPlayerName) {
            Player1 = gameController.PlayerGO()[i];
            if(!DBManager.isSinglePlayer)
                propertie = "FirstPlayerSkin";
        }
        else if (username == SecondPlayerName) {
            Player2 = gameController.PlayerGO()[i];
            if (!DBManager.isSinglePlayer)
                propertie = "SecondPlayerSkin";
        }
        if (username != DBManager.username) {
            if (DBManager.planeType == 1)
                i = i - 7;
            skinButtons[i].interactable = false;
        }
        if (!DBManager.isSinglePlayer) {
            cloudAnchorController.costumProperties.Clear();
            cloudAnchorController.costumProperties.Add(propertie, i);
            PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);
        }
       
    }

    [PunRPC]
    public void GameOver(string winingPlayer) {
        SetBoardInteractable(false);
        gameOverPanel.SetActive(true);
        if (winingPlayer == "draw") {
            gameOverText.text = "It's a draw";
        }
        else {
            gameOverText.text = ticTacToeController.PlayerSide + " Won!";
            if (!DBManager.isSinglePlayer) {
                if (winingPlayer == PhotonNetwork.player.NickName)
                    scoreConroller.IncreaseScore();
                else
                    scoreConroller.DecreaseScore();
            }
            else {
                if (winingPlayer == DBManager.username)
                    scoreConroller.IncreaseScore();
                else
                    scoreConroller.DecreaseScore();
            }
            
        }
    }

    [PunRPC]
    public void SetBoardInteractable(bool toggle) {
        for (int i = 0; i < FindObjectOfType<GridSpaceController>().gridSpaceList.Count; i++) 
            FindObjectOfType<GridSpaceController>().gridSpaceList[i].GetComponent<BoxCollider>().enabled = toggle;
    }

    [PunRPC]
    public void PlaneType(int i) {
        DBManager.planeType = i;
    }

    [PunRPC]
    public void ChangeSide() {
        if (ticTacToeController.GameEnd)
            return;

        ticTacToeController.PlayerSide = (ticTacToeController.PlayerSide == FirstPlayerName) ? SecondPlayerName : FirstPlayerName;

        cloudAnchorController.costumProperties.Clear();
        cloudAnchorController.costumProperties.Add("PlayerSide", ticTacToeController.PlayerSide);
        PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);

        if (!DBManager.pcTesting) {
            bool x = (ticTacToeController.PlayerSide == PhotonNetwork.player.NickName) ? true : false;
            SetBoardInteractable(x);
        }
        playerTurnText.text = (ticTacToeController.PlayerSide == PhotonNetwork.player.NickName) ? "Your" : ticTacToeController.PlayerSide;
    }

    [PunRPC]
    public void SpawnPrefab(Vector3 pos, Quaternion quat, int index) {
        if (isBombActivated) {
            isBombActivated = false;
            ticTacToeController.InstantiateObject(pos, quat, index, gameController.PlayerGO()[6], "Bomb");
        }
        else {
            if (ticTacToeController.PlayerSide == FirstPlayerName)
                ticTacToeController.InstantiateObject(pos, quat, index, Player1, FirstPlayerName);
            else if (ticTacToeController.PlayerSide == SecondPlayerName)
                ticTacToeController.InstantiateObject(pos, quat, index, Player2, SecondPlayerName);
        }
    }

    [PunRPC]
    public void AddPlayerForce(string username) {
        if (username == ticTacToeController.PlayerSide) 
            isBombActivated = true;
    }
}
