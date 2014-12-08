using UnityEngine;
using UnityEngine.UI;

public class GameValues : MonoBehaviour {
    protected static GameValues instance;
    public static GameValues Instance {
        get { return instance ?? (instance = FindObjectOfType<GameValues>()); }
    }

    public Harvester harvesterPrefab;
    public static Harvester HarvesterPrefab {
        get { return Instance.harvesterPrefab; }
    }


    public Missile missilePrefab;
    public static Missile MissilePrefab {
        get { return Instance.missilePrefab; }
    }

    public float mineOutput;
    public static float MineOutput {
        get { return Instance.mineOutput; }
    }

    public float generatorOutput;
    public static float GeneratorOutput {
        get { return Instance.generatorOutput; }
    }

    public float shieldDrain;
    public static float ShieldDrain {
        get { return Instance.shieldDrain; }
    }

    public BuildingCost[] buildingCosts;
    public static BuildingCost[] BuildingCosts {
        get { return Instance.buildingCosts; }
    }

    public HomeBase playerBase;
    public static HomeBase PlayerBase {
        get { return Instance.playerBase; }
    }

    public int planetStartHealth;
    public static int PlanetStartHealth {
        get { return Instance.planetStartHealth; }
    }

    public float missileBuildTime;
    public static float MissileBuildTime {
        get { return Instance.missileBuildTime; }
    }

    public int harvesterMass;
    public static int HarvesterMass {
        get { return Instance.harvesterMass; }
    }

    public int harvesterEnergy;
    public static int HarvesterEnergy {
        get { return Instance.harvesterEnergy; }
    }

    public float harvesterBuildInterval;
    public static float HarvesterBuildInterval {
        get { return Instance.harvesterBuildInterval; }
    }

    public int maxHarvesters;
    public static int MaxHarvesters {
        get { return Instance.maxHarvesters; }
    }

    public Button harvesterButton;
    public Button launcherButton;
    public Button generatorButton;
    public Button mineButton;
    public Button launchButton;

    public AudioClip clickSfx;

    protected Rect screenRect;
    public static Rect ScreenRect {
        get {
            if (Instance.screenRect.width == 0) {
                var bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
                var topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
                Instance.screenRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
            }
            return Instance.screenRect;
        }
    }

    public static void PlanetDestroyed(HomeBase homeBase) {
        if (homeBase == PlayerBase) {
            Application.LoadLevel(4);
        } else {
            Application.LoadLevel(3);
        }
    }

    public void UIClick() {
        audio.PlayOneShot(clickSfx, 2);
    }

    void Update() {
        harvesterButton.interactable = playerBase.CanAffordHarvester();
        launcherButton.interactable = playerBase.CanAffordBuilding(Building.Launcher) && !playerBase.buildingProgress.ContainsKey(Building.Launcher);
        generatorButton.interactable = playerBase.CanAffordBuilding(Building.Generator) && !playerBase.buildingProgress.ContainsKey(Building.Generator);
        mineButton.interactable = playerBase.CanAffordBuilding(Building.Mine) && !playerBase.buildingProgress.ContainsKey(Building.Mine);
        launchButton.interactable = playerBase.MissilesReady > 0;
    }
}