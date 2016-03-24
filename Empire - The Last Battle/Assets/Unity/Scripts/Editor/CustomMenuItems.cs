using UnityEditor;

public class CustomMenuItems {

    // Monster

    [MenuItem("Assets/Create/ELB/Monster/MonsterBaseData")]
    public static void CreateMonsterBaseData() {
        DataEditor.CreateScriptableObject("MonsterBaseData", "Monsters/");
    }

    // Player

    [MenuItem("Assets/Create/ELB/Player/Player")]
    public static void CreatePlayer() {
        DataEditor.CreateScriptableObject("Player", "Players/");
    }

    [MenuItem("Assets/Create/ELB/Player/Army")]
    public static void CreateArmy() {
        DataEditor.CreateScriptableObject("Army", "Players/");
    }

    // Battle

    [MenuItem("Assets/Create/ELB/Battle/BattleData")]
    public static void CreateBattleData() {
        DataEditor.CreateScriptableObject("BattleData", "Battle/");
    }

    // Board

    [MenuItem("Assets/Create/ELB/Board/BoardData")]
    public static void CreateBoardData() {
        DataEditor.CreateScriptableObject("BoardData", "Board/");
    }

    [MenuItem("Assets/Create/ELB/Board/BoardColumn")]
    public static void CreateBoardColumn() {
        DataEditor.CreateScriptableObject("BoardColumn", "Board/Columns/");
    }

    [MenuItem("Assets/Create/ELB/Board/Building/BuildingTypeData")]
    public static void CreateBuildingTypeData() {
        DataEditor.CreateScriptableObject("BuildingTypeData", "Board/Buildings/");
    }

    [MenuItem("Assets/Create/ELB/Board/Terrain/TerrainTypeData")]
    public static void CreateTerrainTypeData() {
        DataEditor.CreateScriptableObject("TerrainTypeData", "Board/Terrains/");
    }


    [MenuItem("Assets/Create/ELB/Board/Tile/TileData")]
    public static void CreateTileData() {
        DataEditor.CreateScriptableObject("TileData", "Board/Tiles/");
    }

    // Card

    [MenuItem("Assets/Create/ELB/Card/CardData")]
    public static void CreateCardData() {
        DataEditor.CreateScriptableObject("CardData", "Cards/");
    }

    // Game State

    [MenuItem("Assets/Create/ELB/GameState/GameStateHolder")]
    public static void CreateGameStateHolder() {
        DataEditor.CreateScriptableObject("GameStateHolder", "GameState/");
    }

    // Unit

    [MenuItem("Assets/Create/ELB/Unit/UnitBaseData")]
    public static void CreateUnitBaseData() {
        DataEditor.CreateScriptableObject("UnitBaseData", "Units/");
    }

	[MenuItem("Assets/Create/ELB/Armoury/PurchasableUnit")]
	public static void CreatePurchasableUnitData()
	{
		DataEditor.CreateScriptableObject(typeof(PurchasableUnit).Name, "Armoury/PurchasableUnits/");
	}

	[MenuItem("Assets/Create/ELB/Armoury/PurchasableCard")]
	public static void CreatePurchasableCardData()
	{
		DataEditor.CreateScriptableObject(typeof(PurchasableCard).Name, "Armoury/PurchasableCards/");
	}

	[MenuItem("Assets/Create/ELB/Armoury/PurchasableCastlePiece")]
	public static void CreatePurchasableCastlePieceData()
	{
		DataEditor.CreateScriptableObject(typeof(PurchasableCastlePiece).Name, "Armoury/PurchasableCastlePiece/");
	}
}

