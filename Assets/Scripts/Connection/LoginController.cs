using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class LoginController : MonoBehaviour {
    public static Action sucessfullAction;
    public static Action logoutAction;
    public static Action popupLoginAction;
    public static Action popupRegisterAction;
    public static Action profileDetailAction;

    public TMP_InputField loginUsernameField;
    public TMP_InputField loginPasswordField;

    public TMP_InputField registerUsernameField;
    public TMP_InputField registerPasswordField;

    public PhotonController photonController;

    private bool entered = false;

    private void Start() {
        if (PlayerPrefsManager.GetUsername() != "" && PlayerPrefsManager.GetPassword() != "") 
            StartCoroutine(LoginPlayer(PlayerPrefsManager.GetUsername(), PlayerPrefsManager.GetPassword()));
    }

    private void Update() {
        if (DBManager.LoggedIn && !entered && photonController.InLobby) {
            entered = true;
        }
    }

    public IEnumerator LoginPlayer(string username = null, string password = null) {
        bool enterPlayView = false;
        WWWForm form = new WWWForm();
        if (username == null && password == null) {
            enterPlayView = true;
            username = loginUsernameField.text;
            password = loginPasswordField.text;
        }
        form.AddField("name", username);
        form.AddField("password", password);
        //WWW www = new WWW("http://localhost/tictactoe/login.php", form);
        WWW www = new WWW("http://epaytech.atwebpages.com/login.php", form);
        //WWW www = new WWW("http://ar.epaytech.net/ar/login.php", form);
        yield return www;
        LoginOrRegister(www, username, password, popupLoginAction, enterPlayView, true);
    }

    public IEnumerator RegisterPlayer() {
        WWWForm form = new WWWForm();
        form.AddField("name", registerUsernameField.text);
        form.AddField("password", registerPasswordField.text);

        //WWW www = new WWW("http://localhost/tictactoe/register.php", form);
        WWW www = new WWW("http://epaytech.atwebpages.com/register.php", form);
        //WWW www = new WWW("http://ar.epaytech.net/ar/register.php", form);
        yield return www;
        LoginOrRegister(www, registerUsernameField.text, registerPasswordField.text, popupRegisterAction);
    }

    private void LoginOrRegister(WWW www, string username, string password, Action failedAction, bool enterPlayView = true, bool score = false) {
        if (www.text[0] == '0') {
            DBManager.username = username;
            if (score) {
                DBManager.score = int.Parse(www.text.Split('\t')[1]);
                DBManager.icon = int.Parse(www.text.Split('\t')[2]);
            }
            else {
                DBManager.score = 0;
                DBManager.icon = 0;
            }
            PlayerPrefsManager.SetUsername(username);
            PlayerPrefsManager.SetPassword(password);
            if (profileDetailAction != null)
                profileDetailAction();
            if (sucessfullAction != null && enterPlayView)
                sucessfullAction();
        }
        else {
            Debug.Log("User login failed. Error #" + www.text);
            if (failedAction != null)
                failedAction();
        }
    }

    public void Login() {
        StartCoroutine(LoginPlayer());
    }

    public void Logout() {
        DBManager.LogOut();
        PlayerPrefsManager.SetUsername("");
        PlayerPrefsManager.SetPassword("");
        if (logoutAction != null) {
            logoutAction();
        }
    }

    public void Register() {
        StartCoroutine(RegisterPlayer());
    }
}
