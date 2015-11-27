using UnityEngine;
using System.Collections.Generic;

public class BoardAnimator : MonoBehaviour {

    public Board BoardManager;
    public float Speed = 1f;
    public float Height = 50f;
    private bool _loaded;
    private int _totalLanded;
    private Vector2 _boardSize;

	// Use this for initialization
	void Start () {
	    BoardManager.OnGenerate +=BoardManager_OnGenerate;
        _boardSize = BoardManager.GetBoardSize();

	}
	
	// Update is called once per frame
	void Update () {
        if (_loaded && _totalLanded < _boardSize.x * _boardSize.y) {
            Vector2 boardSize = BoardManager.GetBoardSize();
            for(int i = 0; i < _boardSize.x; i++) {
                for (int j = 0; j < _boardSize.y; j++) {
                    Tile t = BoardManager.GetTileAt(i, j);
                    if (t.TileObject.transform.position.y < t.TileData.Height) {
                        Vector3 position = t.TileObject.transform.position;
                        //Debug.Log(t.TileData.Height);
                        t.TileObject.transform.position = new Vector3(position.x, t.TileData.Height, position.z);
                        _totalLanded++;
                    }
                    else {
                        t.TileObject.transform.Translate(0, -Speed*Time.deltaTime, 0);
                    }
                    
                }
            }
        }
	}

    private void BoardManager_OnGenerate() {
        _loaded = true;
        Vector2 boardSize = BoardManager.GetBoardSize();
        for (int i = 0; i < boardSize.x; i++) {
            for (int j = 0; j < boardSize.y; j++) {
                GameObject tile = BoardManager.GetTileAt(i, j).TileObject;
                tile.transform.position = new Vector3(tile.transform.position.x, Height, tile.transform.position.z);
            }
        }
    }
}
