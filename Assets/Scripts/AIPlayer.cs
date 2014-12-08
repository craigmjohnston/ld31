using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class AIPlayer : MonoBehaviour {
    public HomeBase homeBase;
    public PlayerShip playerShip;
    public HomeBase enemyBase;

    public enum State { Neutral, Building, Defensive, Aggressive }
    protected State state = State.Neutral;

	void Start () {
	
	}
	
	void Update () {
	    switch (state) {
	        case State.Neutral: StateNeutral(); break;
            case State.Building: StateBuilding(); break;
            case State.Defensive: StateDefensive(); break;
            case State.Aggressive: StateAggressive(); break;
	        default: throw new ArgumentOutOfRangeException();
	    }
	}

    protected void StateNeutral() {
        // build priority: mines, generators, launchers
        if (homeBase.NumberOfMines < 5) {
            if (homeBase.CanAffordBuilding(Building.Mine)) {
                homeBase.StartBuildMine();
            }
        } else if (homeBase.NumberOfGenerators < 5) {
            if (homeBase.CanAffordBuilding(Building.Generator)) {
                homeBase.StartBuildGenerator();
            }
        } else if (homeBase.CanAffordBuilding(Building.Launcher)) {
            homeBase.StartBuildLauncher();
        }
        // launch any missiles we have ready
        if (homeBase.NumberOfLaunchers > 0) {
            int readyMissiles = homeBase.MissilesReady;
            for (int i = 0; i < readyMissiles; i++) {
                homeBase.LaunchMissile();
            }
        }
        // enable the shield if the enemy player has launched missiles
        if (enemyBase.launchedMissiles.Count > 0 && !homeBase.shield.activated) {
            homeBase.shield.Enable();
        }
    }

    protected void StateBuilding() {
        // prioritise building buildings, normal resource gathering
    }

    protected void StateDefensive() {
        // prioritise fueling the shield, conservative resource gathering, defensive ship behaviour
    }

    protected void StateAggressive() {
        // harass enemy harvesters, liberal resource gathering, prioritise building missiles
    }
}