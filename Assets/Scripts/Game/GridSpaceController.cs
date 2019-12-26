using System.Collections.Generic;
using UnityEngine;
using System;

public class GridSpaceController : MonoBehaviour {
    public static Action populateListAction;
    public List<GameObject> gridSpaceList;

    private void Start() {
        if (populateListAction != null)
            populateListAction();
    }
}
