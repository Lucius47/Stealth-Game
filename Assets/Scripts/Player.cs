using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public event System.Action OnReachedFinish;

	public float moveSpeed = 7;
	public float smoothMoveTime = 0.1f;
	public float turnSpeed = 8;

	float angle;
	float smoothInputMagnitude;
	float smoothMoveVelocity;
	bool disabled;

	Rigidbody rigidbody;
	Vector3 velocity;



	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		Guard.OnGuardHasSpottedPlayer += Disable;


	}

	void Update () {
		Vector3 inputDirection = Vector3.zero;
		if (!disabled) {
			inputDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
		}
		float inputMagnitude = inputDirection.magnitude;
		smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
		float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
		angle = Mathf.LerpAngle (angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);
		velocity = transform.forward * moveSpeed * smoothInputMagnitude;
	}

	void OnTriggerEnter (Collider hitCollider) {
		if (hitCollider.tag == "Finish") {
			Disable ();
			if (OnReachedFinish != null) {
				OnReachedFinish ();
			}
		}
	}

	void Disable () {
		disabled = true;
	}

	void FixedUpdate () {
		rigidbody.MoveRotation (Quaternion.Euler (Vector3.up * angle));
		rigidbody.MovePosition (rigidbody.position + velocity * Time.deltaTime);
	}


	void OnDestroy () {
		Guard.OnGuardHasSpottedPlayer -= Disable;
	}



}
