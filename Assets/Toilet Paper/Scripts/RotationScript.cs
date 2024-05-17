using UnityEngine;
using System.Collections;

public class RotationScript : MonoBehaviour {

    public float x;
    public float y;
    public float z;

    public bool randomStartX;
    public bool randomStartY;
    public bool randomStartZ;

	// Use this for initialization
	void Start () {

        if (randomStartX) {
            transform.Rotate(Random.Range(0.1f, 360), 0, 0);
        }

        if (randomStartY) {
            transform.Rotate(0, Random.Range(0.1f, 360), 0);
        }

        if (randomStartZ) {
            transform.Rotate(0, 0, Random.Range(0.1f, 360));
        }

	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
	}
}
