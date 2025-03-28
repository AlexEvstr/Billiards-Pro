using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour {

	[SerializeField] protected int ballNumber;
	[SerializeField] protected float stopThreshold;
	[SerializeField] protected float stopThresholdAngular;
	[SerializeField] protected float railDamping;
	[SerializeField] protected float maxAngularVelocity;

	[SerializeField] protected Texture ballTexture;

	[SerializeField] protected AudioClip ballCollisionSound;
	[SerializeField] protected AudioClip railCollisionSound;
	[SerializeField] protected AudioClip pocketSound;
	[SerializeField] protected AudioClip ballCollectorSound;

	private MusicAndVibro _musicAndVibro;

	protected PoolManager poolManager;
	protected Rigidbody rb;
	protected MeshRenderer meshRenderer;
	protected AudioSource audioSrc;

	protected BallCollector ballCollector;

	public float Radius {
		get;
		private set;
	}

	public int BallNumber {
		get {
			return ballNumber;
		}
	}

	public bool IsAtRest {
		get {
			return rb.velocity.magnitude <= stopThreshold &&
			rb.angularVelocity.magnitude <= stopThresholdAngular;
		}
	}

	public bool IsPocketed {
		get {
			return poolManager.IsBallPocketed (this);
		}
	}

	public BallType Type {
		get {
			if (BallNumber == 0) {
				return BallType.CUE;
			}

			if (BallNumber == 8) {
				return BallType.EIGHT;
			}

			if (BallNumber >= 1 && BallNumber <= 7) {
				return BallType.SOLID;
			}

			if (BallNumber >= 9 && BallNumber <= 15) {
				return BallType.STRIPED;
			}

			return BallType.UNKNOWN;
		}
	}

	protected virtual void Awake() {
		poolManager = GameObject.FindObjectOfType<PoolManager> ();

		rb = GetComponent<Rigidbody> ();
		rb.maxAngularVelocity = maxAngularVelocity;

		meshRenderer = GetComponent<MeshRenderer> ();
		meshRenderer.material.mainTexture = ballTexture;

		audioSrc = GetComponent<AudioSource> ();

		Radius = GetComponent<Collider> ().bounds.size.x / 2.0f;

		ballCollector = GameObject.FindObjectOfType<BallCollector> ();
	}

    private void Start()
    {
		_musicAndVibro = FindAnyObjectByType<MusicAndVibro>();
	}

    protected virtual void Update() {
		if (!rb.isKinematic)
		{
			if (rb.velocity.y > 0)
			{
				Vector3 velocity = rb.velocity;
				velocity.y = 0;
				rb.velocity = velocity;
			}

			if (rb.velocity.magnitude <= stopThreshold)
			{
				rb.velocity = Vector3.zero;
			}

			if (rb.angularVelocity.magnitude <= stopThresholdAngular)
			{
				rb.angularVelocity = Vector3.zero;
			}
		}

	}

	protected void HandleCollisionEnter(Collision col, bool isServer) {
		if(col.collider.CompareTag("Rail")) {
			PlayRailCollisionSound ();

			if (isServer) {
				rb.velocity *= railDamping;

				poolManager.CurrentTurn.AddToCushionedBalls (this);
				if (poolManager.CurrentTurn.BallsHitByCueBall.Count > 0) {
					poolManager.CurrentTurn.BallsHitRailsAfterContact = true;
				}
			}
		}

		if (col.collider.CompareTag ("Ball")) {
			PlayBallCollisionSound ();

			if (isServer) {
				if (BallNumber == 0) {
					Ball otherBall = col.collider.GetComponent<Ball> ();
					if (otherBall != null) {
						if (poolManager.CurrentTurn != null) {
							poolManager.CurrentTurn.AddToBallsHitByCueBall (otherBall);
						}
					}
				}
			}
		}
	}

	protected void HandleTriggerEnter(Collider col, bool isServer) {
		if (col.CompareTag ("Pocket")) {
			Pocket pocket = col.GetComponent<Pocket> ();
			PlayPocketSound ();
			_musicAndVibro.PeekVibration();
			if (isServer) {
				poolManager.CurrentTurn.AddToPocketedBalls (this);
				StartCoroutine (PocketCo (pocket));
			}
		}
	}

	public override string ToString () {
		return BallNumber.ToString ();
	}

	public void Stop() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	protected IEnumerator PocketCo(Pocket pocket) {
		SetLayer (0);
		SetGravity (false);

		for (int i = 0; i < pocket.path.nodes.Count; i++) {
			Transform currentNode = pocket.path.nodes [i];

			while (Vector3.Distance (currentNode.position, transform.position) > 0.01f) {
				Vector3 direction = (currentNode.position - transform.position).normalized;
				rb.velocity = direction * pocket.ballVelInPocket;
				rb.angularVelocity = rb.angularVelocity.normalized * pocket.ballAngVelInPocket;

				yield return new WaitForEndOfFrame ();
			}
		}

		if (BallNumber == 0) {
			transform.position = new Vector3 (1000, 1000, 1000);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			yield break;
		}

		StartCollectorSound ();

		transform.position = poolManager.BallCollectorNodes [0].position;
		transform.up = Vector3.left;

		for (int i = 1; i < poolManager.BallCollectorNodes.Count; i++) {
			Transform currentNode = poolManager.BallCollectorNodes [i];

			while (Vector3.Distance (currentNode.position, transform.position) > 0.01f) {
				Vector3 direction = (currentNode.position - transform.position).normalized;
				rb.velocity = direction * pocket.ballVelInCollector;
				rb.angularVelocity = Vector3.left * pocket.ballAngVelInCollector;

				yield return new WaitForEndOfFrame ();
			}

			transform.position = currentNode.position;
		}

		Stop();

		Sleep ();

		PlayBallReachedSound ();

		Transform lastNode = poolManager.BallCollectorNodes [poolManager.BallCollectorNodes.Count - 1];
		Vector3 lastNodePos = lastNode.position;

		float deltaZ = Radius * 2;
		deltaZ = deltaZ + (deltaZ * 0.05f);

		lastNodePos.z += deltaZ;
		lastNode.position = lastNodePos;

		StopCollectorSound ();
	}

	protected void PlayBallCollisionSound() {
		if (PlayerPrefs.GetFloat("SoundsVolume", 1) == 1)
        {
			audioSrc.volume = 1.0f;
			audioSrc.clip = ballCollisionSound;
			audioSrc.Play();
		}
			
	}

	protected void PlayRailCollisionSound() {
		if (PlayerPrefs.GetFloat("SoundsVolume", 1) == 1)
        {
			audioSrc.volume = 0.5f;
			audioSrc.clip = railCollisionSound;
			audioSrc.Play();
		}
			
	}

	protected void PlayPocketSound() {
		if (PlayerPrefs.GetFloat("SoundsVolume", 1) == 1)
        {
			audioSrc.volume = 1.0f;
			audioSrc.clip = pocketSound;
			audioSrc.Play();
		}
			
	}

	protected virtual void PlayBallReachedSound() {
		if (PlayerPrefs.GetFloat("SoundsVolume", 1) == 1)
			PlayBallCollisionSound ();
	}

	protected virtual void SetLayer(int layerNumber) {
		this.gameObject.layer = layerNumber;
	}

	protected virtual void SetGravity(bool gravityState) {
		rb.useGravity = gravityState;
	}

	protected virtual void Sleep() {
		rb.isKinematic = true;
		rb.Sleep ();
	}

	protected virtual void StartCollectorSound() {
		if (PlayerPrefs.GetFloat("SoundsVolume", 1) == 1)
			ballCollector.StartCollectorSound ();
	}

	protected virtual void StopCollectorSound() {
		ballCollector.StopCollectorSound ();
	}

}
