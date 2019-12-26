using UnityEngine;
using Doozy.Engine.Nody;
using UnityEngine.SceneManagement;
public class SinglePlayerView : MonoBehaviour{
    public GraphController MyController;
    public void StartSinglePlayerGame() {
        if (!DBManager.LoggedIn) {
            MyController.GoToNodeByName("Login");
        }
        else {
            DBManager.isSinglePlayer = true;
            SceneManager.LoadScene("TicTacToe");
        }
    }
}
