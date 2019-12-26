//-----------------------------------------------------------------------
// <copyright file="CloudAnchorController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using GoogleARCore.Examples.CloudAnchors;
using GoogleARCore.Examples.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ModifiedCloudAnchorController : Photon.MonoBehaviour {
    public ExitGames.Client.Photon.Hashtable costumProperties = new ExitGames.Client.Photon.Hashtable();

    public static Action<int> MessageToCallAction;

    public PointcloudVisualizer pointcloudVisualizer;
    public GuestController guestController;
    public GameController gameController;
    public ArPrefabController arPrefabController;
    public CloudAnchorPhoton cloudAnchorPhoton;
    public GameControllerPhoton gameControllerPhoton;
    public GameMainCanvas gameMainCanvas;

    public GameObject ARCoreRoot;
    //public GameObject ARKitRoot;
    //public Camera ARKitFirstPersonCamera;

    public Component SinglePlayerAnchor { get; set; }
    public Component LastPlacedAnchor { get; set; }
    public Component LastDistancePlacedAnchor { get; set; }
    public XPAnchor LastResolvedAnchor { get; set; }

    //private ARKitHelper m_ARKit = new ARKitHelper();
    private bool m_IsQuitting = false;

    private ApplicationMode m_CurrentMode = ApplicationMode.Ready;
    public enum ApplicationMode {
        Ready,
        Hosting,
        Resolving,
    }

    public void Start() {
        pointcloudVisualizer.gameObject.SetActive(true);
        if (Application.platform != RuntimePlatform.IPhonePlayer) {
            ARCoreRoot.SetActive(true);
            //ARKitRoot.SetActive(false);
        }
        else {
            ARCoreRoot.SetActive(false);
            //ARKitRoot.SetActive(true);
        }
        if (PhotonNetwork.isMasterClient || DBManager.isSinglePlayer) {
            if (!DBManager.inDistance) {
                m_CurrentMode = ApplicationMode.Hosting;
                MessageToCallAction?.Invoke(0);
            }
            arPrefabController.RandomArPrefab();
        }
        else if (!PhotonNetwork.isMasterClient || DBManager.isSinglePlayer) {
            if (!DBManager.inDistance) {
                MessageToCallAction?.Invoke(12);
            }
        }
        if (DBManager.inDistance || DBManager.isSinglePlayer) {
            MessageToCallAction?.Invoke(1);
        }
        if (DBManager.pcTesting) {
            m_CurrentMode = ApplicationMode.Hosting;
            ShowPopup();
        }
        if (DBManager.isGuest) {
            gameControllerPhoton.ChangeBoardColor((int)PhotonNetwork.room.CustomProperties["BoardColor"]);
            DBManager.planeType = (int)PhotonNetwork.room.CustomProperties["PlaneType"];
            if (!DBManager.inDistance) {
                OnEnterResolvingModeClick();
                StartCoroutine("DelayResolve");
            }
            else {
                MessageToCallAction?.Invoke(1);
            }
        }
    }

    public void Update() {
        _UpdateApplicationLifecycle();
        if (LastPlacedAnchor != null || LastResolvedAnchor != null || LastDistancePlacedAnchor != null || SinglePlayerAnchor != null) {
            PlaneVisualiserOff();
            return;
        }
        if (m_CurrentMode != ApplicationMode.Hosting && !DBManager.inDistance && !DBManager.isSinglePlayer)
            return;

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            return;

        if (Application.platform != RuntimePlatform.IPhonePlayer) {
            TrackableHit hit;
            if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit)) {
                if (!DBManager.inDistance && !DBManager.isSinglePlayer)
                    LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                else if (DBManager.isSinglePlayer)
                    SinglePlayerAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                else
                    LastDistancePlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                DetectedPlaneGenerator.instance.CheckPlane(hit);
            }
        }
        else {
            //Pose hitPose;
            //if (m_ARKit.RaycastPlane(ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose)) 
            //    LastPlacedAnchor = m_ARKit.CreateAnchor(hitPose);
        }
        if (LastPlacedAnchor != null) {
            OnEnterHostingModeClick();
            gameControllerPhoton.photonView.RPC("PlaneType", PhotonTargets.All, DBManager.planeType);
            costumProperties.Add("PlaneType", DBManager.planeType);
            PhotonNetwork.room.SetCustomProperties(costumProperties);
            arPrefabController.SpawnArPrefabs(LastPlacedAnchor);
            _HostLastPlacedAnchor();
        }
        if (LastDistancePlacedAnchor != null)
            InDistanceInstantiateBoard();
        if (SinglePlayerAnchor != null)
            SingleInstantiateBoard();
    }

    public void OnEnterHostingModeClick() {
        m_CurrentMode = ApplicationMode.Hosting;
        cloudAnchorPhoton.photonView.RPC("EnterResolving", PhotonTargets.Others);
        MessageToCallAction?.Invoke(2);
    }

    public void OnEnterResolvingModeClick() {
        m_CurrentMode = ApplicationMode.Resolving;
        if (MessageToCallAction != null)
            MessageToCallAction(3);
    }

    private void _HostLastPlacedAnchor() {
#if !UNITY_IOS || ARCORE_IOS_SUPPORT

#if !UNITY_IOS
        var anchor = (Anchor)LastPlacedAnchor;
#else
        var anchor = (UnityEngine.XR.iOS.UnityARUserAnchorComponent)LastPlacedAnchor;
#endif
        MessageToCallAction?.Invoke(4);
        XPSession.CreateCloudAnchor(anchor).ThenAction(result => {
            if (result.Response != CloudServiceResponse.Success) {
                MessageToCallAction?.Invoke(5);
                return;
            }
            costumProperties.Add("CloudID", result.Anchor.CloudId);
            PhotonNetwork.room.SetCustomProperties(costumProperties);
            cloudAnchorPhoton.photonView.RPC("Resolve", PhotonTargets.Others, result.Anchor.CloudId);
            MessageToCallAction?.Invoke(6);
            ShowPopup();
        });
#endif
    }

    public void _ResolveAnchorFromId(string cloudAnchorId) {
        XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction((System.Action<CloudAnchorResult>)(result => {
            if (result.Response != CloudServiceResponse.Success) {
                MessageToCallAction?.Invoke(7);
                RecallResolve(cloudAnchorId);
                return;
            }
            LastResolvedAnchor = result.Anchor;
            arPrefabController.SpawnArPrefabs(LastResolvedAnchor);
            cloudAnchorPhoton.photonView.RPC("BothObjSpawn", PhotonTargets.AllBuffered);
            MessageToCallAction?.Invoke(6);
            if (!DBManager.isGuest)
                ShowPopup();
            else
                guestController.PopulateGuestBoard();
        }));
    }

    private void InDistanceInstantiateBoard() {
        if (!DBManager.isGuest) {
            cloudAnchorPhoton.photonView.RPC("BothObjSpawnDistance", PhotonTargets.All);
            ShowPopup();
        }
        arPrefabController.SpawnArPrefabs(LastDistancePlacedAnchor);
        MessageToCallAction?.Invoke(8);
        if (DBManager.isGuest) {
            guestController.PopulateGuestBoard();
        }
    }

    private void SingleInstantiateBoard() {
        arPrefabController.SpawnArPrefabs(SinglePlayerAnchor);
        ShowPopup();
        MessageToCallAction?.Invoke(8);
    }

    private void _UpdateApplicationLifecycle() {
        if (Input.GetKey(KeyCode.Escape)) {
            //Application.Quit();
            SceneManager.LoadScene("Connection");
        }

        var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking) {
            const int lostTrackingSleepTimeout = 15;
            sleepTimeout = lostTrackingSleepTimeout;
        }
#endif
        Screen.sleepTimeout = sleepTimeout;
        if (m_IsQuitting) {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError()) {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    private void _DoQuit() {
        Application.Quit();
    }

    private void _ShowAndroidToastMessage(string message) {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null) {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    private void PlaneVisualiserOff() {
        pointcloudVisualizer.gameObject.SetActive(false);
        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("plane")) {
            Renderer r = plane.GetComponent<Renderer>();
            DetectedPlaneVisualizer t = plane.GetComponent<DetectedPlaneVisualizer>();
            r.enabled = false;
            t.enabled = false;
        }
    }

    private void RecallResolve(string value) {
        MessageToCallAction?.Invoke(9);
        _ResolveAnchorFromId(value);
    }

    IEnumerator DelayResolve() {
        yield return new WaitForSeconds(3f);
        _ResolveAnchorFromId(PhotonNetwork.room.CustomProperties["CloudID"].ToString());
    }

    private void ShowPopup() {
        gameMainCanvas.moveBoardPopup.SetActive(true);
        DBManager.inSettings = true;
        Invoke("HidePopup", 3);
    }

    private void HidePopup() {
        gameMainCanvas.moveBoardPopup.SetActive(false);
        gameMainCanvas.scalePopup.SetActive(true);
    }
}