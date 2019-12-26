using System;
using TMPro;
using UnityEngine;


public class NFCPanel : MonoBehaviour {
    //public static Action checkNfcAction;
    public GameObject inactivePanel;
    public TextMeshProUGUI inactivePanelText;
    //private int i = 0;

    //private void OnEnable() {
    //    if (i > 0) {
    //        Debug.Log("SS");
    //        checkNfcAction?.Invoke();
    //    }

    //    else {
    //        Debug.Log("LL");
    //        i++;
    //    }
            
    //}

    private void Start() {
        if (DBManager.nfcStatus == 1)
            inactivePanel.SetActive(false);
        else if (DBManager.nfcStatus == 0) {
            inactivePanelText.text = "NFC is not Supported in this Device";
            inactivePanel.SetActive(true);
        }
        else {
            inactivePanelText.text = "Enable NFC via Settings!";
            inactivePanel.SetActive(true);
        }
    }
}
