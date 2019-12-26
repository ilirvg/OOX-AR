using UnityEngine;

public class Bomb : MonoBehaviour {
    public GameObject[] explosionEffect;
    public string scale;

    private AudioSource audioSource;
    private GameObject choosenExplosionEffect;
    private float radius;
    private float delay = 1f;
    private float countdown;
    private bool hasExploded = false;
    private int[] emptyArray = new int[] { 0, 0, 0 };

    void Start () {
        audioSource = GetComponent<AudioSource>();
        radius = FindObjectOfType<GridSpaceController>().transform.localScale.x * 357.2f;
        countdown = delay;
        if (radius <= 0.2) {
            choosenExplosionEffect = explosionEffect[0];
        }
        else if (radius > 0.25 && radius <= 35) {
            choosenExplosionEffect = explosionEffect[1];
        }
        else {
            choosenExplosionEffect = explosionEffect[2];
        }
    }
	
	void Update () {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded) {
            hasExploded = true;
            if (audioSource)
                audioSource.Play();

            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer item in meshRenderers) {
                item.enabled = false;
            }
            Explode();
            
        }
	}

    private void Explode() {
        //Show effect
        Instantiate(choosenExplosionEffect, transform.position, Quaternion.identity);
        //Get nearby Objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        //DebugExtension.DebugWireSphere(transform.position, Color.red, radius);
        

        foreach (Collider nearbyObjects in colliders) {
            if (nearbyObjects.tag == "Player") {
                nearbyObjects.transform.parent.transform.parent.name = "GridSpace";

                int i = nearbyObjects.transform.parent.transform.parent.transform.parent.GetComponent<GridSpaceController>().gridSpaceList.IndexOf(nearbyObjects.transform.parent.transform.parent.gameObject);
                int j = FindObjectOfType<GridSpaceController>().gridSpaceList.IndexOf(gameObject.transform.parent.transform.parent.gameObject);

                if (!DBManager.isSinglePlayer) {
                    FindObjectOfType<ModifiedCloudAnchorController>().costumProperties.Clear();
                    FindObjectOfType<ModifiedCloudAnchorController>().costumProperties.Add("Instantiate" + i, emptyArray);
                    FindObjectOfType<ModifiedCloudAnchorController>().costumProperties.Add("Instantiate" + j, emptyArray);
                    PhotonNetwork.room.SetCustomProperties(FindObjectOfType<ModifiedCloudAnchorController>().costumProperties);
                }

                Destroy(nearbyObjects.transform.parent.gameObject);
            }
        }

        gameObject.transform.parent.transform.parent.name = "GridSpace";
        Destroy(gameObject.transform.parent.gameObject, audioSource.clip.length);
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
