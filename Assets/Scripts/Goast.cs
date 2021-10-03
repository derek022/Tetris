using UnityEngine;
using UnityEngine.Tilemaps;

public class Goast : MonoBehaviour
{
    public Board mainBoard;

    public Piece trackingPiece;

    public Tile goastTile;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }

    public Vector3Int position { get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }


    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();

        Set();
    }


    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }


    private void Copy()
    {
        for(int i = 0;i<this.trackingPiece.cells.Length;i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }


    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;

        int currentY = position.y;
        int bottom = -this.mainBoard.boundSize.y / 2 - 1;

        this.mainBoard.Clear(this.trackingPiece);

        for (int row = currentY; row >= bottom; row--)
        {
            position.y = row;

            if (this.mainBoard.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        this.mainBoard.Set(this.trackingPiece);

    }


    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, goastTile);
        }
    }


}
