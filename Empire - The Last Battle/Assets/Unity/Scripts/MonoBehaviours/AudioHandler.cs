using UnityEngine;

public class AudioHandler
{
	Audio _audio;
	TurnManager _turnManager;
	Armoury _armoury;
	Army _army;
	CardSystem _cardSystem;
	CommanderUI _battlebeardUI;
	CommanderUI _stormshaperUI;
	OverworldUI _overWorldUI;

	// Use this for initialization
	public void Init(Audio au, TurnManager tm, Armoury armoury, Army army, CardSystem cs, CommanderUI ssUi, CommanderUI bbUI, OverworldUI oUI)
	{
		_audio = au;
		_turnManager = tm;
		_armoury = armoury;
		_army = army;
		_cardSystem = cs;
		_battlebeardUI = bbUI;
		_stormshaperUI = ssUi;
		_overWorldUI = oUI;

	//	_turnManager.OnSwitchTurn += SwitchTurnListener;
		//_armoury.OnPurchasedItem += PurchasedItemListener;
		//_army.OnUpdateUnit += UpdateUnitListener;
		//_army.OnAddUnit += AddUnitListener;
		//_army.OnRemoveUnit += RemoveUnitListener;
		_cardSystem.OnEffectApplied += EffectAppliedListener;
		_cardSystem.OnCardUseFailed += EffectFailedListener;
		_battlebeardUI.OnStartDrag += BattlebeardStartDragListener;
		//_battlebeardUI.OnDropCommander += BattlebeardDropListener;
		_stormshaperUI.OnStartDrag += StormshaperStartDragListener;
	//	_stormshaperUI.OnDropCommander += StormshaperDropListener;
		_overWorldUI.OnPause += PauseListener;
		_overWorldUI.OnUnPause += UnPauseListener;	}

	void SwitchTurnListener()
	{
		_audio.Play(_turnManager, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void PurchasedItemListener(PurchasableItem purchasedItem)
	{
		_audio.Play(_armoury, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void UpdateUnitListener(Unit u)
	{
		_audio.Play(_army, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void AddUnitListener(Unit u)
	{
		_audio.Play(_army, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void RemoveUnitListener(Unit u, int unitIndex)
	{
		//Maybe want to play a different sound depending on unit index?
		//switch (unitIndex) {
		//default:
		//	break;
		//}

		_audio.Play(_army, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void EffectAppliedListener(CardData cd, Player p)
	{
		_audio.Play(_cardSystem, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void EffectFailedListener(CardData cd, Player p)
	{
		_audio.Play(_cardSystem, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void BattlebeardStartDragListener()
	{
		_audio.Play(_battlebeardUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void BattlebeardDropListener(TileData td)
	{
		_audio.Play(_battlebeardUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void StormshaperStartDragListener()
	{
		_audio.Play(_stormshaperUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void StormshaperDropListener(TileData td)
	{
		_audio.Play(_stormshaperUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}
	
	void PauseListener()
	{
		_audio.Play(_overWorldUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}

	void UnPauseListener()
	{
		_audio.Play(_overWorldUI, new AudioEventArgs { ClipName = "cartoon001", Priority = 256 });
	}
}