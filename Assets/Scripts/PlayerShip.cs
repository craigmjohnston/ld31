using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {
    public float acceleration;
    public float maxSpeed;
    public float stopTime;
    public float turnSpeed;

    public Bullet bulletPrefab;
    public float firingInterval;
    public float overheatTime;
    public float cooldownTime;

    public int health;

    public AudioClip shootSfx;

    public Vector2 velocity;
    protected Vector2 currentVelocity;
    protected float firingTimer;
    protected float direction;

    protected float overheatTimer;
    protected bool overheated;

    public HomeBase homeBase;
	
	void Update () {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
	        velocity += (Vector2)transform.up*acceleration*Time.deltaTime;
	    }
	    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
	        direction += turnSpeed*Time.deltaTime;
	    }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            velocity += -(Vector2)transform.up * acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            direction -= turnSpeed*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q)) {
            velocity += -Vector2.right * acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E)) {
            velocity += Vector2.right * acceleration * Time.deltaTime;
        }
	    velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
	    velocity = Vector2.SmoothDamp(velocity, Vector2.zero, ref currentVelocity, stopTime);
	    transform.position += (Vector3)velocity*Time.deltaTime;
	    transform.rotation = Quaternion.Euler(0, 0, direction);

	    if (overheated) {
	        overheatTimer -= Time.deltaTime / (cooldownTime / overheatTime);
	        if (overheatTimer <= 0) {
	            overheatTimer = 0;
	            overheated = false;
	        }
	    } else if (Input.GetKey(KeyCode.Space)) {
	        overheatTimer += Time.deltaTime;
	        if (overheatTimer >= overheatTime) {
	            overheated = true;
	        }
	        firingTimer -= Time.deltaTime;
	        if (firingTimer <= 0) {
	            firingTimer += firingInterval;
	            var bullet = (Bullet) Instantiate(bulletPrefab, transform.position + transform.up * 0.25f + transform.right * Random.Range(-0.1f, 0.1f), Quaternion.identity);
	            bullet.direction = velocity.normalized;
	            bullet.mothership = transform;
	            bullet.homeBase = homeBase;
                audio.PlayOneShot(shootSfx, 0.4f);
	        }
	    } else if (overheatTimer > 0) {
            overheatTimer -= Time.deltaTime * (cooldownTime / overheatTime);
            if (overheatTimer <= 0) {
                overheatTimer = 0;
            }
        }
	}

    public void Damage(int amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
