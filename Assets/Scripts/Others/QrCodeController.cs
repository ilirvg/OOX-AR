using UnityEngine;
using ZXing;
using ZXing.Common;
using System;

public class QrCodeController : MonoBehaviour {
    const string randomCharacers = "abcdefghijklmnopqrstuvwxyz0123456789/.,\';][=-!@#$%^&*()_+{}|?><"; //add the characters you want

    public NewRoomController newRoomController;
    public PhotonController photonController;
    public QRPanel qRPanel;
    //public GameObject qrCodePanel;

    private string generatedRoomName;
    private string roomName;
    private WebCamTexture webcamTexture;
    public WebCamTexture CamTexture {
        get { return webcamTexture; }
        set { webcamTexture = value; }
    }

    private void Start() {
        webcamTexture = new WebCamTexture();
    }

    private void Update() {
        if (webcamTexture.isPlaying) {
            ReadBarcode();
        }
    }

    public void RandomStringGenerator() {
        int charAmount = UnityEngine.Random.Range(3, 64);
        for (int i = 0; i < charAmount; i++) {
            generatedRoomName += randomCharacers[UnityEngine.Random.Range(0, randomCharacers.Length)];
        }
        GenerateBarcode(generatedRoomName, 1000, 1000);
        newRoomController.QrCreateRoom(generatedRoomName);
    }

    public void GenerateBarcode(string data, int width, int height) {
        BitMatrix bitMatrix = new MultiFormatWriter().encode(data, BarcodeFormat.QR_CODE, width, height);
        // Generate the pixel array
        Color[] pixels = new Color[bitMatrix.Width * bitMatrix.Height];
        int pos = 0;
        for (int y = 0; y < bitMatrix.Height; y++) {
            for (int x = 0; x < bitMatrix.Width; x++) {
                pixels[pos++] = bitMatrix[x, y] ? Color.black : Color.white;
            }
        }
        // Setup the texture
        Texture2D tex = new Texture2D(bitMatrix.Width, bitMatrix.Height);
        tex.SetPixels(pixels);
        tex.Apply();
        qRPanel.generateRaw.texture = tex;
    }

    private void ReadBarcode() {
        try {
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(webcamTexture.GetPixels32(), webcamTexture.width, webcamTexture.height);
            if (result != null) {
                roomName = result.Text;
                webcamTexture.Stop();
                newRoomController.JoinRoom(roomName);
            }
        }
        catch (Exception ex) { Debug.LogWarning(ex.Message); }
    }
}