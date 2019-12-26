using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.Soundy;

public class VolumeController : MonoBehaviour {
    public Image volumeON;
    public Image volumeOFF;
    private int volumeState;

	void Start () {
        volumeState = PlayerPrefsManager.GetVolume();
        if (volumeState == 1) {
            SetImg(true, false);
            SoundyManager.UnmuteAllSounds();
            AudioListener.volume = 1f;
        }
        else {
            SetImg(false, true);
            SoundyManager.MuteAllSounds();
            AudioListener.volume = 0f;
        }
    }
	
	public void SwitchVolume() {
        if (volumeState == 1) {
            volumeState = 0;
            SetImg(false, true);
            SoundyManager.MuteAllSounds();
            PlayerPrefsManager.SetVolume(0);
            AudioListener.volume = 0f;
        }
        else {
            volumeState = 1;
            SetImg(true, false);
            SoundyManager.UnmuteAllSounds();
            PlayerPrefsManager.SetVolume(1);
            AudioListener.volume = 1f;
        }
    }

    private void SetImg(bool ON, bool OFF) {
        volumeON.gameObject.SetActive(ON);
        volumeOFF.gameObject.SetActive(OFF);
    }
}
