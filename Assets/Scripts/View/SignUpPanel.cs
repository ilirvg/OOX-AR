using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignUpPanel : MonoBehaviour {
    public LoginController loginController;
    public SignInPanel signInPanel;
    public GameObject incorectSignup;
    public TMP_InputField signupUsernameField;
    public TMP_InputField signupPasswordField;
    public Button signUpButton;

    //public void RegisterRememberMe(bool value) {
    //    loginController.RememberMeRegister = value;
    //}

    //public void GoToLoginPanel() {
    //    signInPanel.gameObject.SetActive(true);
    //    gameObject.SetActive(false);
    //}

    //public void CallSignup() {
    //    incorectSignup.gameObject.SetActive(false);
    //    StartCoroutine(loginController.Register());
    //    signUpButton.GetComponent<Animator>().SetInteger("buttonClicked", 1);
    //    signUpButton.transform.GetChild(0).gameObject.SetActive(false);
    //    signUpButton.transform.GetChild(1).gameObject.SetActive(true);
    //}
}
