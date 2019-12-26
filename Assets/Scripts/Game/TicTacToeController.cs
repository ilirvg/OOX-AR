using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeController : MonoBehaviour {
    public static Action<int> TicMessageToCallAction;

    public ModifiedCloudAnchorController cloudAnchorController;
    public GameControllerPhoton gameControllerPhoton;
    public ScoreController scoreConroller;

    public Image BombImg;
    public bool GameEnd { get; set; }
    public bool Draw { get; set; }
    public bool BoombUsed { get; set; }
    public string PlayerSide { get; set; }

    public List<GameObject> SpawnList { get; set; } = new List<GameObject>();
    private int[] instantiateList = new int[3];
    private int gridSpaceListIndex;

    private void OnEnable() {
        GridSpaceController.populateListAction += PopulateSinglePlayerGridList;
    }

    private void OnDisable() {
        GridSpaceController.populateListAction -= PopulateSinglePlayerGridList;
    }

    private void Start() {
        if (DBManager.isGuest)
            BombImg.gameObject.SetActive(false);
    }

    public void TouchedGrid(GameObject hitedGrid) {
        
        if (!DBManager.pcTesting) {
            if (!DBManager.inDistance) {
                if (!DBManager.isSkinChoosen || !DBManager.bothObjSpawn && !DBManager.isSinglePlayer || DBManager.isGuest)
                    return;
            }
            else {
                if (!DBManager.isSkinChoosen || !DBManager.bothObjSpawnDistance || DBManager.isGuest)
                    return;
            }
        }
        foreach (GameObject g in FindObjectOfType<GridSpaceController>().gridSpaceList) {
            if (g == hitedGrid) {
                gridSpaceListIndex = FindObjectOfType<GridSpaceController>().gridSpaceList.IndexOf(g);
            }

        }
        if (!DBManager.isSinglePlayer)
            gameControllerPhoton.photonView.RPC("SpawnPrefab", PhotonTargets.All, hitedGrid.transform.position, hitedGrid.transform.rotation, gridSpaceListIndex);
        else
            gameControllerPhoton.SpawnPrefab(hitedGrid.transform.position, hitedGrid.transform.rotation, gridSpaceListIndex);

        AfterSpawnCheck();
    }

    public void InstantiateObject(Vector3 pos, Quaternion quat, int index, GameObject obj, string name) {
        GameObject playerObj = Instantiate(obj, pos, quat) as GameObject;
        playerObj.transform.parent = FindObjectOfType<GridSpaceController>().gridSpaceList[index].transform;
        FindObjectOfType<GridSpaceController>().gridSpaceList[index].transform.name = name;

        foreach (var item in SpawnList) {
            if (item == FindObjectOfType<GridSpaceController>().gridSpaceList[index] && name != "Bomb") {
                SpawnList.Remove(item);
                break;
            }
        }
        //Costum Properties
        if (!DBManager.isSinglePlayer) {
            instantiateList[0] = 1;
            instantiateList[1] = (name == gameControllerPhoton.FirstPlayerName) ? 0 : 1;
            instantiateList[2] = index;
            cloudAnchorController.costumProperties.Clear();
            cloudAnchorController.costumProperties.Add("Instantiate" + index, instantiateList);
            PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);
        }
    }

    private void AfterSpawnCheck() {
        MoveCounts();
        CheckForWin(0, 1, 2);
        CheckForWin(3, 4, 5);
        CheckForWin(6, 7, 8);
        CheckForWin(0, 3, 6);
        CheckForWin(1, 4, 7);
        CheckForWin(2, 5, 8);
        CheckForWin(0, 4, 8);
        CheckForWin(2, 4, 6);
        if (Draw && !GameEnd) {
            if (!DBManager.isSinglePlayer) {
                gameControllerPhoton.photonView.RPC("GameOver", PhotonTargets.All, "draw");
                GameEnd = true;
            }
            else {
                gameControllerPhoton.GameOver("draw");
                GameEnd = true;
            }
        }   
        else {
            if (!DBManager.isSinglePlayer)
                gameControllerPhoton.photonView.RPC("ChangeSide", PhotonTargets.All);
            else
                ChangeSideAI();
        }
    }

    public void ChangeSideAI() {
        if (GameEnd)
            return;

        PlayerSide = (PlayerSide == gameControllerPhoton.FirstPlayerName) ? gameControllerPhoton.SecondPlayerName : gameControllerPhoton.FirstPlayerName;
        if (!DBManager.pcTesting) {
            bool x = (PlayerSide == gameControllerPhoton.FirstPlayerName) ? true : false;
            gameControllerPhoton.SetBoardInteractable(x);
        }
        gameControllerPhoton.playerTurnText.text = (PlayerSide == gameControllerPhoton.FirstPlayerName) ? "Your" : PlayerSide;

        if (PlayerSide == gameControllerPhoton.SecondPlayerName) {
            StartCoroutine(InstantiateAI(2));
        }
    }

    public IEnumerator InstantiateAI(int i) {
        int a = UnityEngine.Random.Range(0, SpawnList.Count);
        foreach (var item in FindObjectOfType<GridSpaceController>().gridSpaceList) {
            if (item == SpawnList[a]) {
                gridSpaceListIndex = FindObjectOfType<GridSpaceController>().gridSpaceList.IndexOf(item);
                break; ;
            }
        }
        GameObject g = FindObjectOfType<GridSpaceController>().gridSpaceList[gridSpaceListIndex];
        yield return new WaitForSeconds(i);
        gameControllerPhoton.SpawnPrefab(g.transform.position, g.transform.rotation, gridSpaceListIndex);
        AfterSpawnCheck();
    }

    private void MoveCounts() {
        int moves = 0;
        foreach (var i in FindObjectOfType<GridSpaceController>().gridSpaceList) {
            if (i.name != "GridSpace" && i.name != "Bomb") {
                moves++;
                if (moves == 9)
                    Draw = true;
            }
        }
    }

    private void CheckForWin(int i, int j, int k) {
        if (FindObjectOfType<GridSpaceController>().gridSpaceList[i].transform.name == PlayerSide &&
            FindObjectOfType<GridSpaceController>().gridSpaceList[j].transform.name == PlayerSide &&
            FindObjectOfType<GridSpaceController>().gridSpaceList[k].transform.name == PlayerSide) {
            if (!DBManager.isSinglePlayer) {
                gameControllerPhoton.photonView.RPC("GameOver", PhotonTargets.All, PlayerSide);
                GameEnd = true;
            } 
            else{
                gameControllerPhoton.GameOver(PlayerSide);
                GameEnd = true;
            }
        }
    }

    public void OnBombClicked(string username) {
        if (BoombUsed || username != PlayerSide)
            return;
        if (DBManager.score > 0) {
            BoombUsed = true;
            DBManager.score--;
            scoreConroller.CallSaveData();
            BombImg.color = new Color32(255, 0, 0, 255);
            if (!DBManager.isSinglePlayer)
                gameControllerPhoton.photonView.RPC("AddPlayerForce", PhotonTargets.AllBuffered, username);
            else
                gameControllerPhoton.AddPlayerForce(username);
        }
        else {
            TicMessageToCallAction?.Invoke(11);
        }
    }

    public void PopulateSinglePlayerGridList() {
        if(SpawnList.Count > 0)
            SpawnList.Clear();

        foreach (var item in FindObjectOfType<GridSpaceController>().gridSpaceList) {
            SpawnList.Add(item);
        }
    }

}
