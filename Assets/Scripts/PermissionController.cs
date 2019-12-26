using GoogleARCore;
using UnityEngine;


public class PermissionController : MonoBehaviour {
    private void Start() {
        const string cameraPermissionName = "android.permission.CAMERA";
        AndroidPermissionsManager.RequestPermission(cameraPermissionName);
    }
}
