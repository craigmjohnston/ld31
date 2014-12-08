using System;
using System.Linq;
using UnityEngine;

public class Harvester : MonoBehaviour {
    public float targetingInterval;
    public float harvestingDistance;
    public float moveSpeed;
    public int maxResources;
    public int health;
    public float harvestingSpeed;
    public float dumpingDistance;
    public float dumpingSpeed;

    public ResourceType resourceType;
    public int resourceQuantity;

    public HomeBase homeBase;

    protected enum State {
        Idle, Chasing, Harvesting, Returning, Dumping
    }

    protected Asteroid target;
    protected State state = State.Idle;

    protected float targetingTimer;
    protected Vector3 targetPosition;

    protected float resourceFraction;

    protected ParticleSystem explosionParticles;

    protected bool destroyed = false;

	void Start () {
	    explosionParticles = GetComponentInChildren<ParticleSystem>();
	}
	
	void Update () {
	    if (destroyed) {
	        if (!explosionParticles.isPlaying) {
	            Destroy(gameObject);
	        }
	        return;
	    }
	    switch (state) {
	        case State.Idle: StateIdle(); break;
	        case State.Chasing: StateChasing(); break;
	        case State.Harvesting: StateHarvesting(); break;
	        case State.Returning: StateReturning(); break;
	        case State.Dumping: StateDumping(); break;
	        default:
	            throw new ArgumentOutOfRangeException();
	    }
	}

    public void Damage(int amount = 1) {
        health -= amount;
        if (health <= 0) {
            destroyed = true;
            renderer.enabled = false;
            explosionParticles.Play();
        }
    }

    protected void StateIdle() {
        targetingTimer -= Time.deltaTime;
        if (targetingTimer <= 0) {
            // search for a new asteroid
            targetingTimer += targetingInterval;
            target = FindTarget();
            var deltaPosition = target.transform.position - transform.position;
            targetPosition = transform.position + Vector3.ClampMagnitude(deltaPosition, deltaPosition.magnitude - harvestingDistance);
            if (target != null) {
                // begin moving towards the target
                resourceType = target.resourceType;
                state = State.Chasing;
            }
        }
    }

    protected void StateChasing() {
        // head towards the target
        MoveTowards(targetPosition);
        if (transform.position == targetPosition) {
            // begin harvesting
            state = State.Harvesting;
        }
    }

    protected void StateHarvesting() {
        // harvest
        resourceFraction += harvestingSpeed * Time.deltaTime;
        var wholeAmount = Mathf.FloorToInt(resourceFraction);
        resourceFraction -= wholeAmount;
        // clamp the value so cargo doesn't overfill and we don't take more than the asteroid has
        wholeAmount = Mathf.Min(wholeAmount, target.quantity, maxResources - resourceQuantity);
        target.quantity -= wholeAmount;
        resourceQuantity += wholeAmount;
        if (resourceQuantity == maxResources) {
            // head back to base
            state = State.Returning;
            var deltaPosition = homeBase.transform.position - transform.position;
            targetPosition = transform.position + Vector3.ClampMagnitude(deltaPosition, deltaPosition.magnitude - dumpingDistance);
            target = null;
        } else if (target == null || target.quantity == 0) {
            // look for a new asteroid
            state = State.Idle;
            target = null;
        }
    }

    protected void StateReturning() {
        // head towards the base
        MoveTowards(targetPosition);
        if (transform.position == targetPosition) {
            // begin dumping
            state = State.Dumping;
        }
    }

    protected void StateDumping() {
        resourceFraction += dumpingSpeed * Time.deltaTime;
        var wholeAmount = Mathf.FloorToInt(resourceFraction);
        resourceFraction -= wholeAmount;
        // clamp the value so we don't dump more than we have
        wholeAmount = Mathf.Min(wholeAmount, resourceQuantity);
        homeBase.resources[resourceType] += wholeAmount;
        resourceQuantity -= wholeAmount;
        if (resourceQuantity == 0) {
            state = State.Idle;
        }
    }

    protected void MoveTowards(Vector3 position) {
        transform.LookAt(position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    protected Asteroid FindTarget() {
        return Asteroid.Instances
            .Where(a => resourceQuantity == 0 || a.resourceType == resourceType)
            .OrderBy(a => Vector3.Distance(a.transform.position, transform.position))
            .FirstOrDefault();
    }
}