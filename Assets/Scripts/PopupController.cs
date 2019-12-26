using Doozy.Engine.UI;
using UnityEngine;
using System;

public class PopupController : MonoBehaviour {
    public static Action checkNfcAfterHideAction;

    private string popupLogin = "Popup 1";
    private string loginMessage = "username or password is incorrect!";
    private string registerMessage = "username is already taken!";

    private string popupIcon = "Popup 2";
    private string iconMessage = "Choose your icon";
    private string popupInfo = "Popup 4";

    private void OnEnable() {
        LoginController.popupLoginAction += ShowLoginPopup;
        LoginController.popupRegisterAction += ShowRegisterPopup;
        IconButtons.hidePopupAction += HidePopup;
    }

    private void OnDisable() {
        LoginController.popupLoginAction -= ShowLoginPopup;
        LoginController.popupRegisterAction -= ShowRegisterPopup;
        IconButtons.hidePopupAction -= HidePopup;
    }

    public void ShowLoginPopup() {
        UIPopup popup = UIPopup.GetPopup(popupLogin);
        if (popup == null)
            return;
        popup.Data.SetLabelsTexts(loginMessage);
        popup.Show();
    }

    public void ShowRegisterPopup() {
        UIPopup popup = UIPopup.GetPopup(popupLogin);
        if (popup == null)
            return;
        popup.Data.SetLabelsTexts(registerMessage);
        popup.Show();
    }

    public void ShowDisconectPopup() {
        UIPopup popup = UIPopup.GetPopup(popupLogin);
        if (popup == null)
            return;
        popup.Data.SetLabelsTexts("Please Check Internet Connection!");
        popup.Show();
    }

    public void ShowIconsPopup() {
        UIPopup popup = UIPopup.GetPopup(popupIcon);
        if (popup == null)
            return;
        popup.Data.SetLabelsTexts(iconMessage);
        popup.Show();
    }

    private void HidePopup() {
        UIPopup.HidePopup(popupIcon);
    }

    public void ShowInfoPopup() {
        UIPopup popup = UIPopup.GetPopup(popupInfo);
        if (popup == null)
            return;
        popup.Show();
    }
}
