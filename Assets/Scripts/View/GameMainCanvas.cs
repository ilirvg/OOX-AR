using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainCanvas : MonoBehaviour {
    public ArPrefabController arPrefabController;
    public GameControllerPhoton gameControllerPhoton;
    public TicTacToeController ticTacToeController;
    public TextMeshProUGUI totalPointsText;
    public TextMeshProUGUI winsText;
    public TextMeshProUGUI losesText;
    public TextMeshProUGUI usernameText;
    public GameObject scaleItems;
    public GameObject topLeftPanel;
    public GameObject scalePopup;
    public GameObject moveBoardPopup;
    public GameObject chooseSkin;
    public Image sButton;
    public Image mButton;
    public Image lButton;

    private List<int> skinsForAI = new List<int>();
    private CloudAnchorMessageController cloudAnchorMessageController;

    private void Start() {
        cloudAnchorMessageController = GetComponent<CloudAnchorMessageController>();

        if (DBManager.isGuest) {
            topLeftPanel.SetActive(false);
        }
        UpdateScore();
        for (int i = 0; i < 6; i++) {
            skinsForAI.Add(i);
        }
    }

    public void UpdateScore() {
        winsText.text = DBManager.wins.ToString();
        losesText.text = DBManager.loses.ToString();
        totalPointsText.text = DBManager.score.ToString();
    }

    public void SkinItemsClicked(int i) {
        chooseSkin.SetActive(false);
        DBManager.isSkinChoosen = true;
        int y = Random.Range(0, 6);
        if (DBManager.planeType != 0) 
            i = i + 7;

        if (!DBManager.isSinglePlayer)
            gameControllerPhoton.photonView.RPC("ChoosePlayerSkin", PhotonTargets.All, i, DBManager.username);
        else {
            gameControllerPhoton.ChoosePlayerSkin(i, DBManager.username);
            skinsForAI.Remove(i);
            int a = Random.Range(0, skinsForAI.Count);
            gameControllerPhoton.ChoosePlayerSkin(skinsForAI[a], gameControllerPhoton.SecondPlayerName);
            if (ticTacToeController.PlayerSide == gameControllerPhoton.SecondPlayerName) {
                StartCoroutine(ticTacToeController.InstantiateAI(2));
            }
        }
    }

    public void ForceItemsClicked() {
        ticTacToeController.OnBombClicked(DBManager.username);
    }

    public void ScaleItems(int i) {
        if (i == 0) {
            DBManager.scale = 0;
            BoardChange(arPrefabController.ArPrefab("S", DBManager.planeType));
            ResetScaleButtons();
            sButton.rectTransform.sizeDelta = new Vector2(310, 160);
        }
        if (i == 1) {
            DBManager.scale = 1;
            BoardChange(arPrefabController.ArPrefab("M", DBManager.planeType));
            ResetScaleButtons();
            mButton.rectTransform.sizeDelta = new Vector2(310, 160);
        }
        if (i == 2) {
            DBManager.scale = 2;
            BoardChange(arPrefabController.ArPrefab("L", DBManager.planeType));
            ResetScaleButtons();
            lButton.rectTransform.sizeDelta = new Vector2(310, 160);
        }
        if (i == 3) {
            scalePopup.SetActive(false);
            SkinToBe();
            DBManager.inSettings = false;
        }
    }

    private void ResetScaleButtons() {
        sButton.rectTransform.sizeDelta = new Vector2(280, 140);
        mButton.rectTransform.sizeDelta = new Vector2(280, 140);
        lButton.rectTransform.sizeDelta = new Vector2(280, 140);
    }

    private void BoardChange(GameObject boardToBe) {
        if (FindObjectOfType<GridSpaceController>().name != boardToBe.name) {
            Vector3 boardPos = FindObjectOfType<GridSpaceController>().transform.position;
            Quaternion boardRot = FindObjectOfType<GridSpaceController>().transform.rotation;
            Destroy(FindObjectOfType<GridSpaceController>().gameObject);
            Instantiate(boardToBe, boardPos, boardRot);
        }
    }

    public void SkinToBe() {
        cloudAnchorMessageController.HideMsgSnackBar();
        chooseSkin.SetActive(true);
        DBManager.isSkinChoosen = false;
    }

    public void OnRestartClicked() {
        if (!DBManager.isSinglePlayer)
            gameControllerPhoton.photonView.RPC("RestartRPC", PhotonTargets.AllBuffered);
        else
            gameControllerPhoton.RestartRPC();
    }
}

