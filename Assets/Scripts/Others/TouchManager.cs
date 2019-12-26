using UnityEngine;

public class TouchManager : MonoBehaviour {
    public TicTacToeController tacToeController;
    public Camera firstPersonCamera;

    private GameObject dragObj = null;
    private Plane objPlnae;
    private Vector3 mouseOffset;

    void Update() {
        if (Input.GetMouseButton(0)) {
            Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPersonCamera.farClipPlane);
            Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPersonCamera.nearClipPlane);
            Vector3 mousePosF = firstPersonCamera.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = firstPersonCamera.ScreenToWorldPoint(mousePosNear);

            RaycastHit hit;
            if (Physics.Raycast(mousePosN, mousePosF - mousePosN, out hit)) {
                if (hit.transform.tag == "GridSpace" && hit.transform.childCount == 0) {
                    GameObject hitedGrid = hit.transform.gameObject;
                    tacToeController.TouchedGrid(hitedGrid);
                }
            }
        }
        if (DBManager.inSettings){
            if (DBManager.planeType == 0) {
                Drag();
            }
            else {
                Rotate();
            }
        }
    }

    private void Drag() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = MouseRay();
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
                dragObj = FindObjectOfType<GridSpaceController>().gameObject;       //hit.transform.parent.gameObject;
                objPlnae = new Plane(firstPersonCamera.transform.forward * -1, dragObj.transform.position);

                //ofset
                Ray mRay = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
                float rayDistance;
                objPlnae.Raycast(mRay, out rayDistance);
                mouseOffset = dragObj.transform.position - mRay.GetPoint(rayDistance);
            }
        }
        else if (Input.GetMouseButton(0) && dragObj) {
            Ray mRay = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlnae.Raycast(mRay, out rayDistance)) {
                dragObj.transform.position = mRay.GetPoint(rayDistance) + mouseOffset;
            }
        }
        else if (Input.GetMouseButtonUp(0) && dragObj) {
            dragObj = null;
        }
    }

    private void Rotate() {
        if (Input.touchCount > 0) {
            Ray ray = MouseRay();
            RaycastHit hit;
            Touch firstTouch = Input.GetTouch(0);
            dragObj = FindObjectOfType<GridSpaceController>().gameObject;
            if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
                if (firstTouch.phase == TouchPhase.Began) {
                }
                if (firstTouch.phase == TouchPhase.Moved) {
                    dragObj.transform.Rotate(0, firstTouch.deltaPosition.x, 0, Space.Self); // -0.5f
                }
            }
        }
    }

    private Ray MouseRay() {
        Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPersonCamera.farClipPlane);
        Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPersonCamera.nearClipPlane);
        Vector3 mousePosF = firstPersonCamera.ScreenToWorldPoint(mousePosFar);
        Vector3 mousePosN = firstPersonCamera.ScreenToWorldPoint(mousePosNear);
        Ray mr = new Ray(mousePosN, mousePosF - mousePosN);
        return mr;
    
    }
}
