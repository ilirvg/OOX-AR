using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ProfileUI : MonoBehaviour {
    public List<Sprite> icons = new List<Sprite>();
    public TextMeshProUGUI username;
    public TextMeshProUGUI points;
    public Image icon;
    
    private void OnEnable() {
        LoginController.profileDetailAction += UpdateProfileDetail;
        IconButtons.updateIconAction += SelectedIcon;
    }

    private void OnDisable() {
        LoginController.profileDetailAction += UpdateProfileDetail;
        IconButtons.updateIconAction -= SelectedIcon;
    }

    public void CallSaveIcon() {
        StartCoroutine(SaveIconData());
    }

    IEnumerator SaveIconData() {
        WWWForm form = new WWWForm();
        form.AddField("name", DBManager.username);
        form.AddField("icon", DBManager.icon);

        //WWW www = new WWW("http://localhost/tictactoe/saveicon.php", form);
        WWW www = new WWW("http://epaytech.atwebpages.com/saveicon.php", form);
        //WWW www = new WWW("http://ar.epaytech.net/ar/saveicon.php", form);
        yield return www;
        if (www.text == "0") {
            Debug.Log("Icon saved");
        }
        else {
            Debug.Log("Save failed. Error #" + www.text);
        }

    }

    private void Start() {
        //icon.GetComponent<Image>().sprite = icons[PlayerPrefsManager.GetIcon()];
        icon.GetComponent<Image>().sprite = icons[DBManager.icon];
        UpdateProfileDetail();
    }

    private void UpdateProfileDetail() {
        username.text = DBManager.username;
        points.text = "points: " + DBManager.score;
    }

    public void SelectedIcon(Image img) {
        icon.GetComponent<Image>().sprite = img.sprite;
        foreach (var icon in icons) {
            if (icon == img.sprite) {
                //PlayerPrefsManager.SetIcon(icons.IndexOf(icon));
                DBManager.icon = icons.IndexOf(icon);
                CallSaveIcon();
            }
        }
    }
}
