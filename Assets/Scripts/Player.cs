using UnityEngine;

public class Player : MonoBehaviour {

	public event System.Action OnReachedFinish;

	[SerializeField] float moveSpeed = 7;
	[SerializeField] float smoothMoveTime = 0.1f;
	[SerializeField] float turnSpeed = 8;

	float angle;
	float smoothInputMagnitude;
	float smoothMoveVelocity;
	bool disabled;

	Rigidbody rb;
	Vector3 velocity;



	void Start () {
		rb = GetComponent<Rigidbody> ();
		Guard.OnGuardHasSpottedPlayer += DisablePlayer;


	}

	void Update () {
		Vector3 inputDirection = Vector3.zero;
		if (!disabled)
		{
			inputDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
		}
		float inputMagnitude = inputDirection.magnitude;
		
		
		//Mathf.SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
		velocity = transform.forward * moveSpeed * smoothInputMagnitude;

		float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
		angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Finish")
		{
			DisablePlayer ();
			OnReachedFinish?.Invoke();
		}
	}

	void DisablePlayer ()
	{
		disabled = true;
	}

	void FixedUpdate ()
	{
		rb.MoveRotation (Quaternion.Euler (Vector3.up * angle));
		rb.MovePosition (rb.position + velocity * Time.deltaTime);
	}


	void OnDestroy ()
	{
		Guard.OnGuardHasSpottedPlayer -= DisablePlayer;
	}



}
