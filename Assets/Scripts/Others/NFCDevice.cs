using UnityEngine;

public class NFCDevice : MonoBehaviour {
    const string randomCharacers = "abcdefghijklmnopqrstuvwxyz0123456789/.,\';][=-!@#$%^&*()_+{}|?><"; //add the characters you want

    public static System.Action showPopupNfcAction;

    public PhotonController photonController;
    public NewRoomController newRoomController;

    private AndroidJavaClass javaClass;
    private AndroidJavaObject activity;
    private string joinRoomName;
    private string generatedRoomName;
    private int i = 0;


    //private void OnEnable() {
    //    CheckNfc();
    //}
    private void OnEnable() {
        //NFCPanel.checkNfcAction += CheckNfc;
        if (i > 0) {
            CheckNfc();
        }

        else {
            i++;
        }
    }

    //private void OnDisable() {
    //    //NFCPanel.checkNfcAction -= CheckNfc;
    //}

    void Start() {
        photonController = FindObjectOfType<PhotonController>();
        javaClass = new AndroidJavaClass("com.epaytech.nfclibrary.Beam");
        activity = javaClass.GetStatic<AndroidJavaObject>("activity");
        RandomStringGenerator();
        CheckNfc();
    }

    public void CreateRoom(string value) {
        newRoomController.CreateRoom(value);
    }

    public void JoinRoom(string value) {
        joinRoomName = value;
        Invoke("InvokeJoin", 2);
    }

    private void RandomStringGenerator() {
        int charAmount = Random.Range(3, 64); 
        for (int i = 0; i < charAmount; i++) {
            generatedRoomName += randomCharacers[Random.Range(0, randomCharacers.Length)];
        }
        activity.Call("GetRoomNameFromUnity", generatedRoomName);
    }

    private void CheckNfc() {
        activity.Call("IsNfcEnabeled");
    }

    public void NfcStatus(string dataReceived) {
        if (dataReceived == "none") 
            DBManager.nfcStatus = 0;
        else if (dataReceived == "true")
            DBManager.nfcStatus = 1;
        else
            DBManager.nfcStatus = 2;
    }

    void InvokeJoin() {
        newRoomController.JoinRoom(joinRoomName);
    }
}