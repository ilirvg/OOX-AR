using UnityEngine;
using ZXing;
using ZXing.Common;
using UnityEngine.UI;

public class QR : MonoBehaviour {

    public RawImage generateRaw;

    private string generatedRoomName;

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
        generateRaw.texture = tex;
    }
}
