using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class AIShip : MonoBehaviour {
    protected enum State {
        Attacking, Defending
    }

    protected State state = State.Attacking;

    public float acceleration;
    public float maxSpeed;
    public float drag;

    public float targetingInterval;

    public HomeBase homeBase;

    public Bullet bulletPrefab;
    public float firingInterval;
    public float overheatTime;
    public float cooldownTime;
    public AudioClip shootSfx;

    public int health;

    protected Transform target;

    protected float targetingTimer;
    protected Vector2 velocity;
    protected float overheatTimer;
    protected bool overheated;
    protected float firingTimer;

    protected float stateTimer;
    protected State lastState;

    protected int lastHarvesterCount;

    public int currentHealth;

	void Start () {
	    currentHealth = health;
        lastHarvesterCount = homeBase.NumberOfHarvesters;
	    lastState = state;
	}
	
	void Update () {
	    if (lastState != state) {
	        stateTimer = 0;
	    }
	    lastState = state;
	    stateTimer += Time.deltaTime;
        WeaponOverheating(); 
        if (lastHarvesterCount < homeBase.NumberOfHarvesters) {
            state = State.Defending;
            target = null;
        }
        lastHarvesterCount = homeBase.NumberOfHarvesters;
	    switch (state) {
	        case State.Attacking: StateAttacking(); break;
	        case State.Defending: StateDefending(); break;
	        default: throw new ArgumentOutOfRangeException();
	    }
	    velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;
	}

    protected void WeaponOverheating() {
        if (overheatTimer > 0) {
            overheatTimer -= Time.deltaTime * (cooldownTime / overheatTime);
            if (overheatTimer <= 0) {
                overheatTimer = 0;
            }
        }
    }

    protected void StateAttacking() {
        targetingTimer -= Time.deltaTime;
        if (targetingTimer <= 0) {
            targetingTimer += targetingInterval;
            FindTarget();
        }
        if (target != null) {
            if (Vector2.Distance(target.transform.position, transform.position) < 2f) {
                Fire();
            }
            if (Vector2.Distance(target.transform.position, transform.position) < 0.5f) {
                var pos = target.position + (target.position - transform.position);
                velocity += (Vector2)(target.transform.position - transform.position).normalized * acceleration *
                            Time.deltaTime;
            } else {
                velocity += (Vector2)(target.transform.position - transform.position).normalized * acceleration *
                            Time.deltaTime;
            }
        } else {
            velocity = Vector2.zero;
        }
    }

    protected void StateDefending() {
        if (stateTimer > 15) {
            state = State.Attacking;
            return;
        }
        if (target == null) {
            var playerShip = FindObjectOfType<PlayerShip>();
            if (playerShip != null) {
                target = playerShip.transform;
            }
        }
        if (target != null) {
            if (Vector2.Distance(target.transform.position, transform.position) < 2f) {
                Fire();
            }
            if (Vector2.Distance(target.transform.position, transform.position) < 0.5f) {
                var pos = target.position + (target.position - transform.position);
                velocity += (Vector2)(target.transform.position - transform.position).normalized * acceleration *
                            Time.deltaTime;
            } else {
                velocity += (Vector2)(target.transform.position - transform.position).normalized * acceleration *
                            Time.deltaTime;
            }
        } else {
            velocity = Vector2.zero;
        }
    }

    protected void Fire() {
        overheatTimer += Time.deltaTime;
        if (overheatTimer >= overheatTime) {
            overheated = true;
        }
        firingTimer -= Time.deltaTime;
        if (firingTimer <= 0) {
            firingTimer += firingInterval;
            var bullet = (Bullet)Instantiate(bulletPrefab, transform.position + transform.up * 0.25f + transform.right * Random.Range(-0.1f, 0.1f), Quaternion.identity);
            bullet.direction = velocity.normalized;
            bullet.mothership = transform;
            //audio.PlayOneShot(shootSfx, 0.4f);
        }
    }

    protected void FindTarget() {
        Harvester harvester = FindObjectsOfType<Harvester>()
                .Where(h => h.homeBase != homeBase)
                .OrderBy(h => Vector2.Distance(h.transform.position, transform.position))
                .FirstOrDefault();
        if (harvester == null) {
            target = FindObjectOfType<PlayerShip>().transform;
        } else {
            target = harvester.transform;
        }
    }

    public void Damage(int amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
