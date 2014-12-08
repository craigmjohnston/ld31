using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Building {
    Generator, Mine, Launcher
}

[Serializable]
public class BuildingCost {
    public Building building;
    public int mass;
    public int energy;
    public float time;
}

public class HomeBase : MonoBehaviour {
    public int health { get; protected set; }

    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    public Text energyText;
    public Text massText;

    public Text launcherText;
    public Text generatorText;
    public Text mineText;
    public Text shieldText;

    public Harvester harvesterPrefab;

    public static List<HomeBase> Instances = new List<HomeBase>();
    protected HomeBase enemyBase;
    public Shield shield { get; protected set; }
    protected List<Building> buildings = new List<Building>();
    protected Dictionary<Building, BuildingCost> buildingCostsLookup = new Dictionary<Building, BuildingCost>();
    protected List<Harvester> harvesters = new List<Harvester>();
    public List<float> launcherTimers = new List<float>(); 
    public List<Image> nukeImages = new List<Image>();
    public List<Missile> launchedMissiles = new List<Missile>();

    public Image launcherProgress;
    public Image mineProgress;
    public Image generatorProgress;
    public Image shieldProgress;

    public Button launchButton;
    public Image nukeImagePrefab;
    public float nukeTopRowY;
    public float nukeBottomRowY;
    public float nukeXMin;
    public float nukeXSpacing;

    public Dictionary<Building, float> buildingProgress = new Dictionary<Building, float>(); 

    protected float harvesterBuildTimer;

    protected int launchers {
        get { return buildings.Count(b => b == Building.Launcher); }
    }

    public int NumberOfMines {
        get { return buildings.Count(b => b == Building.Mine); }
    }

    public int NumberOfGenerators {
        get { return buildings.Count(b => b == Building.Generator); }
    }

    public int NumberOfLaunchers {
        get { return buildings.Count(b => b == Building.Launcher); }
    }

    public int NumberOfHarvesters {
        get { return harvesters.Count; }
    }

    public int MissilesReady {
        get { return launcherTimers.Count(t => t <= 0); }
    }

    void Awake() {
        Instances.Add(this);
        shield = GetComponentInChildren<Shield>();
    }

	void Start () {
	    health = GameValues.PlanetStartHealth;
	    foreach (BuildingCost buildingCost in GameValues.BuildingCosts) {
	        buildingCostsLookup.Add(buildingCost.building, buildingCost);
	    }
	    enemyBase = Instances.FirstOrDefault(i => i != this);
	    for (int i = 0; i < Enum.GetValues(typeof(ResourceType)).Length; i++) {
	        resources.Add((ResourceType)i, 0);
	    }
        BuildGenerator();
        BuildMine();
	}
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.H)) {
	        BuildHarvester();
	    }
	    if (Input.GetKeyDown(KeyCode.M)) {
	        LaunchMissile();
	    }
	    if (Input.GetKeyDown(KeyCode.N)) {
	        ToggleShield();
	    }
	    foreach (Building building in buildings) {
	        switch (building) {
	            case Building.Generator:
                    resources[ResourceType.Energy] += GameValues.GeneratorOutput * Time.deltaTime;
	                break;
	            case Building.Mine:
                    resources[ResourceType.Mass] += GameValues.MineOutput * Time.deltaTime;
	                break;
	            default:
	                break;
	        }
	    }
	    if (massText != null) {
	        massText.text = ((int)resources[ResourceType.Mass]).ToString();
            energyText.text = ((int)resources[ResourceType.Energy]).ToString();
	    }
	    if (launcherText != null) {
	        launcherText.text = NumberOfLaunchers.ToString();
            generatorText.text = NumberOfGenerators.ToString();
            mineText.text = NumberOfMines.ToString();
	    }
	    if (shield.activated) {
            resources[ResourceType.Energy] -= GameValues.ShieldDrain * Time.deltaTime;
	        if (resources[ResourceType.Energy] <= 0) {
	            shield.Disable();
	            resources[ResourceType.Energy] = 0;
	        }
	    }
	    for (int i = 0; i < launcherTimers.Count; i++) {
	        launcherTimers[i] -= Time.deltaTime;
	        if (nukeImagePrefab != null) {
	            nukeImages[i].color = new Color(1, 1, 1, launcherTimers[i] > 0 ? 0.2f : 1f);
	        }
	    }

	    harvesterBuildTimer -= Time.deltaTime;
	    if (harvesterBuildTimer <= 0 && NumberOfHarvesters < GameValues.MaxHarvesters) {
	        harvesterBuildTimer += GameValues.HarvesterBuildInterval;
            BuildHarvester();
	    }
	    harvesters.RemoveAll(h => h == null);
	    Building[] keys = buildingProgress.Keys.ToArray();
        foreach (Building building in keys) {
            buildingProgress[building] += Time.deltaTime / buildingCostsLookup[building].time;
	        if (generatorProgress != null) {
                switch (building) {
	                case Building.Generator:
                        generatorProgress.fillAmount = buildingProgress[building];
	                    break;
	                case Building.Mine:
                        mineProgress.fillAmount = buildingProgress[building];
	                    break;
	                case Building.Launcher:
                        launcherProgress.fillAmount = buildingProgress[building];
	                    break;
	                default:
	                    throw new ArgumentOutOfRangeException();
	            }
	        }
            if (buildingProgress[building] >= 1) {
                buildingProgress.Remove(building);
                switch (building) {
                    case Building.Generator:
                        if (generatorProgress != null) {
                            generatorProgress.fillAmount = 0;
                        }
                        BuildGenerator();
                        break;
                    case Building.Mine:
                        if (mineProgress != null) {
                            mineProgress.fillAmount = 0;
                        }
                        BuildMine();
                        break;
                    case Building.Launcher:
                        if (launcherProgress != null) {
                            launcherProgress.fillAmount = 0;
                        }
                        BuildLauncher();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
	        }
	    }
	}

    public void LaunchMissile() {
        if (!launcherTimers.Any(t => t <= 0)) return;
        var missile = (Missile)Instantiate(GameValues.MissilePrefab, transform.position, Quaternion.identity);
        missile.target = enemyBase;
        launcherTimers[launcherTimers.FindIndex(t => t <= 0)] = GameValues.MissileBuildTime;
        launchedMissiles.Add(missile);
        missile.Destroyed += MissileDestroyed;
    }

    protected void MissileDestroyed(Missile missile) {
        launchedMissiles.Remove(missile);
        missile.Destroyed -= MissileDestroyed;
    }

    public void BuildHarvester() {
        //if (!CanAffordHarvester()) return;
        //resources[ResourceType.Mass] -= GameValues.HarvesterMass;
        //resources[ResourceType.Energy] -= GameValues.HarvesterEnergy;
        var harvester = (Harvester)Instantiate(harvesterPrefab, transform.position, Quaternion.identity);
        harvester.homeBase = this;
        harvesters.Add(harvester);
    }

    public void ToggleShield() {
        shield.Toggle();
    }

    public void MissileHit() {
        health -= 1;
        resources[ResourceType.Mass] /= 2f;
        if (health <= 0) {
            GameValues.PlanetDestroyed(this);
            Destroy(gameObject);
        }
    }

    public void StartBuildMine() {
        if (!CanAffordBuilding(Building.Mine)) return;
        if (buildingProgress.ContainsKey(Building.Mine)) return;
        buildingProgress.Add(Building.Mine, 0);
        PayForBuilding(Building.Mine);
    }

    public void StartBuildGenerator() {
        if (!CanAffordBuilding(Building.Generator)) return;
        if (buildingProgress.ContainsKey(Building.Generator)) return;
        buildingProgress.Add(Building.Generator, 0);
        PayForBuilding(Building.Generator);
    }

    public void StartBuildLauncher() {
        if (!CanAffordBuilding(Building.Launcher)) return;
        if (buildingProgress.ContainsKey(Building.Launcher)) return;
        buildingProgress.Add(Building.Launcher, 0);
        PayForBuilding(Building.Launcher);
    }

    public void BuildMine() {
        buildings.Add(Building.Mine);
    }

    public void BuildGenerator() {
        buildings.Add(Building.Generator);
    }

    public void BuildLauncher() {
        buildings.Add(Building.Launcher);
        launcherTimers.Add(GameValues.MissileBuildTime);
        if (nukeImagePrefab != null) {
            var nukeIcon = (Image) Instantiate(nukeImagePrefab);
            nukeIcon.transform.parent = launchButton.transform;
            nukeIcon.rectTransform.anchoredPosition = new Vector3(
                nukeXMin + ((launcherTimers.Count - 1)/2*nukeXSpacing),
                launcherTimers.Count%2 == 1 ? nukeTopRowY : nukeBottomRowY);
            nukeImages.Add(nukeIcon);
        }
    }

    public bool CanAffordBuilding(Building building) {
        BuildingCost cost = buildingCostsLookup[building];
        return resources[ResourceType.Mass] >= cost.mass && resources[ResourceType.Energy] >= cost.energy;
    }

    public bool CanAffordHarvester() {
        return resources[ResourceType.Mass] >= GameValues.HarvesterMass && resources[ResourceType.Energy] >= GameValues.HarvesterEnergy;
    }

    protected void PayForBuilding(Building building) {
        BuildingCost cost = buildingCostsLookup[building];
        resources[ResourceType.Mass] -= cost.mass;
        resources[ResourceType.Energy] -= cost.energy;
    }
}
