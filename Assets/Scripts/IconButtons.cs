using UnityEngine;
using UnityEngine.UI;
using System;

public class IconButtons : MonoBehaviour {
    public static Action<Image> updateIconAction;
    public static Action hidePopupAction;

    public void ButtonClicked(Image icon) {
        if (updateIconAction != null)
            updateIconAction(icon);
        if (hidePopupAction != null)
            hidePopupAction();
    }
}
