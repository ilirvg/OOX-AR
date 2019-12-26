using UnityEngine;

public class CloudAnchorPhoton : Photon.MonoBehaviour {
    public ModifiedCloudAnchorController cloudAnchorController;
    public GameController gameController;
    public GameMainCanvas gameMainCanvas;   

    [PunRPC]
    private void EnterResolving() {
        cloudAnchorController.OnEnterResolvingModeClick();
    }

    [PunRPC]
    private void Resolve(string value) {
        cloudAnchorController._ResolveAnchorFromId(value);
    }

    [PunRPC]
    private void BothObjSpawn() {
        DBManager.bothObjSpawn = true;
    }

    [PunRPC]
    public void BothObjSpawnDistance() {
        DBManager.bothBoardsSpawn++;
        if(DBManager.bothBoardsSpawn == 2 || DBManager.isSinglePlayer)
            DBManager.bothObjSpawnDistance = true;
    }
}
