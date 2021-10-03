using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }

    public int rotationIndex { get; private set; }


    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;


    public void Initialize(Board board,Vector3Int position ,TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;

        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if(this.cells == null)
        {
            this.cells = new Vector3Int[this.data.cells.Length];
        }

        for(int i =0;i<data.cells.Length;i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }


    private void Update()
    {
        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            HardDrop();
        }

        if(Time.time >= this.stepTime)
        {
            Step();
        }

        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    private void HardDrop()
    {
        while(Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }


    private bool Move(Vector2Int translate)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translate.x;
        newPosition.y += translate.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if(valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalIndex = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        Debug.Log("rotationIndex " + rotationIndex);

        ApplyRotationMatrix(direction);

        if(!TestWallKicks(this.rotationIndex,direction))
        {
            this.rotationIndex = originalIndex;
            ApplyRotationMatrix(-direction);
        }
    }


    private void ApplyRotationMatrix(int direction)
    {

        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cellPos = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cellPos.x -= 0.5f;
                    cellPos.y -= 0.5f;
                    x = Mathf.CeilToInt((cellPos.x * matrix[0] * direction) + (cellPos.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cellPos.x * matrix[2] * direction) + (cellPos.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cellPos.x * matrix[0] * direction) + (cellPos.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cellPos.x * matrix[2] * direction) + (cellPos.y * matrix[3] * direction));

                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    /// <summary>
    /// 测试旋转结果是否超过了边界
    /// </summary>
    /// <param name="rotationIndex"></param>
    /// <param name="rotationDirection"></param>
    /// <returns></returns>
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="rotationIndex"></param>
    /// <param name="rotationDirection"></param>
    /// <returns></returns>
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }


    /// <summary>
    /// 变量循环   0，1，2，3  
    ///  0-- =》  -1 => 3 
    ///  3++ =》 4 => 0
    /// </summary>
    /// <param name="value">当前值</param>
    /// <param name="min">最小值（包含）</param>
    /// <param name="exmax">最大值（不包含）</param>
    /// <returns></returns>
    private int Wrap(int value,int min,int exmax)
    {
        return (value + exmax) % (exmax - min) + min;
    }




}
