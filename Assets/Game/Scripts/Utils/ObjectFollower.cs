using UnityEngine;

public class ObjectFollower : MonoBehaviour {

	public Transform target;
	public Vector3 offset;

	void OnEnable() {
		SetPosition ();
	}

	void Update() {
		SetPosition ();
	}

	private void SetPosition() {
		if (target == null) {
			return;
		}

		transform.position = target.position + offset;
	}

}
