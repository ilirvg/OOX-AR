using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour {
    public GameMainCanvas settingsUIController;
    //public CloudAnchorMessageController cloudAnchorUIController;

    public static Action<int> ScoreMessageToCallAction;

    private int count = 0;

    private void Awake() {
        if (!DBManager.LoggedIn && !DBManager.isGuest) {
           SceneManager.LoadScene(0);
        } 
    }

    public void CallSaveData() {
        StartCoroutine(SavePlayerData());
    }

    IEnumerator SavePlayerData() {
        WWWForm form = new WWWForm();
        form.AddField("name", DBManager.username);
        form.AddField("score", DBManager.score);

        //WWW www = new WWW("http://localhost/tictactoe/savedata.php", form);
        WWW www = new WWW("http://epaytech.atwebpages.com/savedata.php", form);
        //WWW www = new WWW("http://ar.epaytech.net/ar/savedata.php", form);
        yield return www;
        if (www.text == "0") {
            Debug.Log("Game saved");
        }
        else {
            Debug.Log("Save failed. Error #" + www.text);
        }

        settingsUIController.UpdateScore();
    }

    public void IncreaseScore() {
        DBManager.score = DBManager.score + 2;
        DBManager.inGameScore++;
        DBManager.wins++;
        CallSaveData();
    }

    public void DecreaseScore() {
        DBManager.loses++;
        if (DBManager.score > 0) {
            DBManager.score--;
            count = 0;
            if (DBManager.inGameScore > 0) {
                DBManager.inGameScore--;
            }
        }
        else {
            count++;
            if (count == 2) {
                //cloudAnchorUIController.TimeToRest();
                if (ScoreMessageToCallAction != null)
                    ScoreMessageToCallAction(10);
                settingsUIController.chooseSkin.SetActive(false);
                Invoke("GoBack", 2);
            }
        }
        CallSaveData();
    }

    public void GoBack() {
        SceneManager.LoadScene("Connection");
    }
}
