using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino 
{
    I,J,L,O,S,T,Z
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino; // 形状
    public Tile tile;  // 格子

    // 和图形中心相对位置坐标
    public Vector2Int[] cells { get; private set; } 

    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }

}
