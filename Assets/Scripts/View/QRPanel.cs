using UnityEngine;
using UnityEngine.UI;

public class QRPanel : MonoBehaviour {
    //public MainCanvas mainCanvas;
    public QrCodeController qrCodeController;
    public PhotonController photonController;
    public RawImage readRaw;
    public RawImage generateRaw;

    private void Start() {
        if (qrCodeController.CamTexture.isPlaying)
            qrCodeController.CamTexture.Stop();
        DBManager.hostClicked = false;
        DBManager.joinClicked = false;
        //readRaw.gameObject.SetActive(false);
        //generateRaw.gameObject.SetActive(false);
    }

    public void Generate() {
        if (DBManager.hostClicked) 
            return;
        DBManager.hostClicked = true;
        DBManager.joinClicked = false;

        if (qrCodeController.CamTexture.isPlaying) 
            qrCodeController.CamTexture.Stop();
        readRaw.gameObject.SetActive(false);
        generateRaw.gameObject.SetActive(true);
        qrCodeController.RandomStringGenerator();
    }

    public void Read() {
        if (DBManager.joinClicked) 
            return;
        if (PhotonNetwork.room != null) 
            PhotonNetwork.LeaveRoom();

        DBManager.hostClicked = false;
        DBManager.joinClicked = true;
        generateRaw.gameObject.SetActive(false);
        readRaw.gameObject.SetActive(true);
        OpenCamera(readRaw);
    }

    public void OpenCamera(RawImage img) {
        readRaw.texture = qrCodeController.CamTexture;
        readRaw.material.mainTexture = qrCodeController.CamTexture;
        qrCodeController.CamTexture.Play();
    }

    public void Back() {
        
        if (qrCodeController.CamTexture.isPlaying) 
            qrCodeController.CamTexture.Stop();

        if (PhotonNetwork.inRoom) 
            PhotonNetwork.LeaveRoom();

        DBManager.hostClicked = false;
        DBManager.joinClicked = false;

        gameObject.SetActive(false);
        readRaw.gameObject.SetActive(false);
        generateRaw.gameObject.SetActive(false);
        //mainCanvas.nfcConnectonPanel.SetActive(true);
    }
}
