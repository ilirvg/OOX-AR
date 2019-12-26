using GoogleARCore.CrossPlatform;
using UnityEngine;

public class ArPrefabController : MonoBehaviour {
    public ModifiedCloudAnchorController cloudAnchorController;
    public GameController gameController;
    public GameControllerPhoton gameControllerPhoton;

    public GameObject[] BoardPrefabS;
    public GameObject[] BoardPrefabM;
    public GameObject[] BoardPrefabL;
    public int RandomNum { get; set; } = -1;

    public GameObject ArPrefab(string size, int type) {
        if (size == "S") 
            return BoardPrefabS[type];
        else if (size == "L") 
            return BoardPrefabL[type];
        else 
            return BoardPrefabM[type];
    }

    public void RandomArPrefab() {
        RandomNum = Random.Range(0, gameControllerPhoton.boardSkins.Count);
        if (!DBManager.isSinglePlayer) {
            cloudAnchorController.costumProperties.Add("BoardColor", RandomNum);
            PhotonNetwork.room.SetCustomProperties(cloudAnchorController.costumProperties);
        }
        ChangeArPrefab();
    }

    private void ChangeArPrefab() {
        if (RandomNum == -1 && !DBManager.isSinglePlayer) 
            gameControllerPhoton.ChangeBoardColor((int)PhotonNetwork.room.CustomProperties["BoardColor"]);
        else {
            if (!DBManager.isSinglePlayer) 
                gameControllerPhoton.photonView.RPC("ChangeBoardColor", PhotonTargets.All, RandomNum);
            else 
                gameControllerPhoton.ChangeBoardColor(RandomNum);
        }
    }

    public void SpawnArPrefabs(Component c = null, XPAnchor x = null) {
        ChangeArPrefab();
        GameObject andyObject = null;
        if (c != null) {
            andyObject = Instantiate(
            ArPrefab("M", DBManager.planeType),
            c.transform.position,
            c.transform.rotation
            ) as GameObject;
        }
        if (x != null) {
            andyObject = Instantiate(
            ArPrefab("M", DBManager.planeType),
            x.transform.position,
            x.transform.rotation
            ) as GameObject;
        }
        gameController.BoardInstantiateModification(andyObject);
    }
}
