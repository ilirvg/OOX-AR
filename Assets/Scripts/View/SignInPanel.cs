using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignInPanel : MonoBehaviour {
    public LoginController loginController;
    public SignUpPanel signUpPanel;
    public QRPanel qRPanel;
    public QrCodeController qrCodeController;

    public RawImage guestReadRaw;
    public GameObject qrGuestPanel;
    public GameObject incorectSignin;

    public TMP_InputField loginUsernameField;
    public TMP_InputField loginPasswordField;
    public Button signInButton;


    private void Start() {
        signInButton.transform.GetChild(0).gameObject.SetActive(true);//No
        signInButton.transform.GetChild(1).gameObject.SetActive(false);//No
        loginUsernameField.text = PlayerPrefsManager.GetUsername();//No
        loginPasswordField.text = PlayerPrefsManager.GetPassword();//No
    }

    public void CallSignin() {
        incorectSignin.SetActive(false);//No
        StartCoroutine(loginController.LoginPlayer());//No
        signInButton.GetComponent<Animator>().SetInteger("buttonClicked", 1);//No
        signInButton.transform.GetChild(0).gameObject.SetActive(false);//No
        signInButton.transform.GetChild(1).gameObject.SetActive(true);//No
    }

    public void GuestRead() {
        DBManager.isGuest = true;
        qrGuestPanel.SetActive(true);
        qRPanel.OpenCamera(guestReadRaw);
    }

    //public void LoginRememberMe(bool value) {//No
    //    loginController.RememberMeLogin = value;//No
    //    Debug.Log(value);//No
    //}

    public void GoToSignupPanel() {//No
        gameObject.SetActive(false);//No
        signUpPanel.gameObject.SetActive(true);//No
    }

    public void CloseGuestRead() {
        qrCodeController.CamTexture.Stop();
        DBManager.isGuest = false;
        qrGuestPanel.SetActive(false);
    }
}
