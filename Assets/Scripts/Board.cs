using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;

    public Tilemap tilemap { get; private set; }

    public Piece activePiece { get; private set; }

    public Vector3Int spawnPosition;

    public Vector2Int boundSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int leftBottomCorner = new Vector2Int(-this.boundSize.x / 2, -this.boundSize.y / 2);
            return new RectInt(leftBottomCorner, boundSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }

    }


    private void Start()
    {
        SpawnPiece();
    }



    public void SpawnPiece()
    {
        int index = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[index];

        this.activePiece.Initialize(this, spawnPosition,data);


        Set(this.activePiece);
    }


    public void ClearLines()
    {
        RectInt bound = this.Bounds;
        int row = bound.yMin;

        while (row < bound.yMax)
        {
            if (IsRowFull(row))
            {
                ClearRow(row);
            }
            else
            {
                row++;
            }
        }

    }


    private bool IsRowFull(int row)
    {
        RectInt bound = this.Bounds;
        for(int col = bound.xMin;col<bound.xMax;col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if(!this.tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }


    private void ClearRow(int row)
    {
        RectInt bound = this.Bounds;
        while (row < bound.yMax)
        {
            for (int col = bound.xMin; col < bound.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);
                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }

    }

    public void Set(Piece piece)
    {
        for(int i =0;i<piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }


    public bool IsValidPosition(Piece piece,Vector3Int pos)
    {
        RectInt bound = this.Bounds;

        for(int i =0;i<piece.cells.Length;i++)
        {
            Vector3Int tilePos = piece.cells[i] + pos;
            // 计算每个图形的格子是否在网格范围内
            if(!bound.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            // 网格里面，改位置是否已经有图形了
            if(this.tilemap.HasTile(tilePos))
            {
                return false;
            }

        }

        return true;
    }
}
