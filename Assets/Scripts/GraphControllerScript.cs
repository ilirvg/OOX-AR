using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using System.Collections.Generic;
using UnityEngine;

public class GraphControllerScript : MonoBehaviour {
    public GraphController MyController;

    public UIView nfc;
    public UIView qr;
    public UIView list;

    private UIView activeView;
    private int i = 0;

    private void OnEnable() {
        LoginController.sucessfullAction += LoginOrRegister;
        LoginController.logoutAction += Logout;
        //PlayPopup.nfcViewAction += Nfc;
        //PlayPopup.qrViewAction += Qr;
        //PlayPopup.distanceViewAction += Distance;
    }

    private void OnDisable() {
        LoginController.sucessfullAction -= LoginOrRegister;
        LoginController.logoutAction -= Logout;
        //PlayPopup.nfcViewAction -= Nfc;
        //PlayPopup.qrViewAction -= Qr;
        //PlayPopup.distanceViewAction -= Distance;
    }

    private void Update() {
        if(UIView.IsViewVisible("Connection", "Nfc") && !DBManager.LoggedIn ||
            UIView.IsViewVisible("Connection", "Qr") && !DBManager.LoggedIn ||
            UIView.IsViewVisible("Connection", "Distance") && !DBManager.LoggedIn)
                MyController.GoToNodeByName("Login");
    }

    public void ShowProfileView()
    {
        if (DBManager.LoggedIn) 
            MyController.GoToNodeByName("Profile");
        else 
            MyController.GoToNodeByName("Login");
    }

    private void LoginOrRegister() {
        MyController.GoToNodeByName("Play");
    }

    private void Logout() {
        MyController.GoToNodeByName("Login");
    }

    //private void Nfc() {
    //    MyController.GoToNodeByName("NFC");
    //}

    //private void Qr() {
    //    MyController.GoToNodeByName("Qr");
    //}

    //private void Distance() {
    //    MyController.GoToNodeByName("Distance");
    //}
}
