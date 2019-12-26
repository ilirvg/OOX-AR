//-----------------------------------------------------------------------
// <copyright file="CloudAnchorUIController.cs" company="Google">
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
using UnityEngine;
using TMPro;

public class CloudAnchorMessageController : MonoBehaviour{
    public GameObject messageSnack;
    public GameObject messageSnackBottom;
    public GameObject loading;
    public TextMeshProUGUI snackText;
    public TextMeshProUGUI snackTextBottom;


    private void OnEnable() {
        ModifiedCloudAnchorController.MessageToCallAction += MessageToShow;
        GoogleARCore.Examples.Common.PlaneDiscoveryGuide.MessageToCallAction += MessageToShow;
        ScoreController.ScoreMessageToCallAction += MessageToShow;
        TicTacToeController.TicMessageToCallAction += MessageToShow;
    }

    private void OnDisable() {
        ModifiedCloudAnchorController.MessageToCallAction -= MessageToShow;
        GoogleARCore.Examples.Common.PlaneDiscoveryGuide.MessageToCallAction -= MessageToShow;
        ScoreController.ScoreMessageToCallAction -= MessageToShow;
        TicTacToeController.TicMessageToCallAction -= MessageToShow;
    }

    private void ShowHostingModeBegin(string snackbarText = null) {
        SnackBarComponents(true, false, snackbarText);
    }

    private void ShowHostingModeAttemptingHost(){
        SnackBarComponents(true, true, "Attempting to host the board...");
    }

    private void ShowResolvingModeBegin(string snackbarText = null){
        SnackBarComponents(true, false, snackbarText);
    }

    private void ShowResolvingModeAttemptingResolve(){
        SnackBarComponents(true, true, "Attempting to resolve the board...");
    }

    private void SearchSurface() {
        SnackBarComponents(true, true, "Scan the correct surface...");
    }

    private void CreateBoard() {
        messageSnackBottom.SetActive(true);
        snackTextBottom.text = "You're the Host. Create The Board!";
    }

    private void WaitForBoard() {
        messageSnackBottom.SetActive(true);
        snackTextBottom.text = "Your Friend Needs To Create The Board!";
    }

    private void CreateBoardDistance() {
        messageSnackBottom.SetActive(true);
        snackTextBottom.text = "Scan than Touch the Surface to Create the Board!";
    }

    private void HideSnackBottom() {
        messageSnackBottom.SetActive(false);
    }

    private void NotEnoughPoints() {
        SnackBarComponents(true, false, "Not enough points to use the bomb!!!");
        Invoke("HideMsgSnackBar", 2);
    }

    private void TimeToRest() {
        SnackBarComponents(true, false, "Time to rest, too many loses!!!");
        Invoke("HideMsgSnackBar", 2);
    }

    private void SnackBarComponents(bool msgBool, bool loadingBool, string msg) {
        messageSnack.SetActive(msgBool);
        loading.SetActive(loadingBool);
        snackText.text = msg;
        messageSnackBottom.SetActive(false);
    }

    public void HideMsgSnackBar() {
        messageSnack.SetActive(false);
    }

    public void MessageToShow(int i) {
        switch (i) {
            case 0:
                CreateBoard();
                break;
            case 1:
                CreateBoardDistance();
                break;
            case 2:
                ShowHostingModeBegin();
                break;
            case 3:
                ShowResolvingModeAttemptingResolve();
                break;
            case 4:
                ShowHostingModeAttemptingHost();
                break;
            case 5:
                ShowHostingModeBegin("Failed to host cloud anchor");
                break;
            case 6:
                HideMsgSnackBar();
                break;
            case 7:
                ShowResolvingModeBegin("Resolving Error");
                break;
            case 8:
                HideSnackBottom();
                break;
            case 9:
                SearchSurface();
                break;
            case 10:
                TimeToRest();
                break;
            case 11:
                NotEnoughPoints();
                break;
            case 12:
                WaitForBoard();
                break;
            default:
                Debug.Log("Wrong selection in swithch statment!!!");
                break;
        }
    }
}
