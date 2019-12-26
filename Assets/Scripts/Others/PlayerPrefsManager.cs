using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    const string USERNAME_KEY = "username";

    public static void SetUsername(string value) {
        PlayerPrefs.SetString(USERNAME_KEY, value);
    }
    public static string GetUsername() {
        return PlayerPrefs.GetString(USERNAME_KEY, "");
    }

    const string PASSWORD_KEY = "password";

    public static void SetPassword(string value) {
        ZPlayerPrefs.SetString(PASSWORD_KEY, value);
        PlayerPrefs.SetString(PASSWORD_KEY, value);
    }
    public static string GetPassword() {
        return ZPlayerPrefs.GetString(PASSWORD_KEY, ""); //ZPlayerPrefs.GetRowString(PASSWORD_KEY, "");
    }

    const string ICON_KEY = "icon";

    public static void SetIcon(int value) {
        PlayerPrefs.SetInt(ICON_KEY, value);
        PlayerPrefs.SetInt(ICON_KEY, value);
    }
    public static int GetIcon() {
        return PlayerPrefs.GetInt(ICON_KEY, 0);     }


    const string VOLUME_KEY = "volume";

    public static void SetVolume(int value) {
        PlayerPrefs.SetInt(VOLUME_KEY, value);
        PlayerPrefs.SetInt(VOLUME_KEY, value);
    }
    public static int GetVolume() {
        return PlayerPrefs.GetInt(VOLUME_KEY, 1);
    }
}
