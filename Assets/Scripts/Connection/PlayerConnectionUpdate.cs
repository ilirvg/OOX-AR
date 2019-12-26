using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConnectionUpdate : MonoBehaviour {
    public ModifiedCloudAnchorController cloudAnchorController;
    public GameControllerPhoton gameControllerPhoton;
    public GameMainCanvas gameMainCanvas;
    private bool isGameOn = false;
    private float time;

    private void Update() {
        if (!DBManager.isSinglePlayer) {
            if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
                PhotonNetwork.ReconnectAndRejoin();
        }
        if (DBManager.pcTesting)
            return;
        EnterGame();
        LeaveGame();
    }

    private void EnterGame() {
        if (!DBManager.isSinglePlayer) {
            if (PhotonNetwork.room.PlayerCount >= 2 && !isGameOn) {
                isGameOn = true;
                gameControllerPhoton.photonView.RPC("EnterGame", PhotonTargets.All);
            }
        }
        else {
            if (!isGameOn) {
                isGameOn = true;
                gameControllerPhoton.EnterGame();
            }
        }
    }

    private void LeaveGame() {
        if (!DBManager.isSinglePlayer) {
            if (isGameOn && PhotonNetwork.room.PlayerCount == 1 && !DBManager.isGuest)
                SceneManager.LoadScene("Connection");
            if (PhotonNetwork.room.PlayerCount == 1 && !isGameOn && !DBManager.hostClicked && !DBManager.isGuest && !DBManager.inDistance) {
                time += Time.deltaTime;
                if (time >= 6.0f)
                    SceneManager.LoadScene("Connection");
            }
        }
    }
}
