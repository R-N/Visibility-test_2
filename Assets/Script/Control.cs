using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Control : MonoBehaviour {
	public CharacterController cc = null;
	public Transform trans = null;
	float mvSpd = 7;
	Vector3 grav = Vector3.zero;
	public Camera cam = null;
	Vector3 inputMove = Vector3.zero;
	// Use this for initialization
	void Start () {
		trans = transform;
	}
	
	// Update is called once per frame
	void Update () {
		inputMove = new Vector3 (CrossPlatformInputManager.GetAxis ("Horizontal"), 0, CrossPlatformInputManager.GetAxis ("Vertical"));
		if (cc.isGrounded) {
			grav = Vector3.zero;
			cc.Move (inputMove * mvSpd * Time.deltaTime);
		} else {
			grav += Physics.gravity * Time.deltaTime;
			cc.Move (grav);
		}
		if (inputMove != Vector3.zero) {
			trans.rotation = Quaternion.Slerp (trans.rotation, Quaternion.LookRotation (inputMove, Vector3.up), 8 * Time.deltaTime);
		}
	}
}
