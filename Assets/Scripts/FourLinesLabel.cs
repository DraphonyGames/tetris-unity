using UnityEngine;
using System.Collections;

public class FourLinesLabel : MonoBehaviour {
    public float LiftUpBy;
    public float LiftSpeed;
    public float FadeSpeed;
    /// <summary>Avoid creating a new font</summary>
    public Color FontColor;

    float Lifted;

	// Use this for initialization
	void Start () {
        Lifted = 0;
        renderer.material.color = FontColor;
	}
	
	// Update is called once per frame
	void Update () {
        if (Lifted < LiftUpBy) {
            float amtToMove = LiftSpeed * Time.deltaTime;
            float amtToFade = FadeSpeed * Time.deltaTime;

            transform.Translate(Vector3.up * amtToMove);
            this.renderer.material.color = new Color(this.renderer.material.color.r, this.renderer.material.color.g,
                                                     this.renderer.material.color.b, Mathf.Max(this.renderer.material.color.a - amtToFade, 0.01f));

            Lifted += (Vector3.up * amtToMove).y;
        }
        else
            Destroy(gameObject);
	}
}
