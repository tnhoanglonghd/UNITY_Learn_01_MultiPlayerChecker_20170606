using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    /// <summary>
    /// 0: white|1: black
    /// </summary>
    public GameObject[] piecePrefabs;
    public bool isWhite;

    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private Piece selectedPiece;

    
    private bool isWhiteTurn;
    private bool hasKill;

    private List<Piece> forcedPieces;

    private void Start()
    {
        isWhiteTurn = true;
        //hasKill = false;
        GenerateBoard();
    }

    private void Update()
    {
        UpdateMouseOver();
        //Debug.Log(mouseOver);

        //If is my turn
        if(isWhite ? isWhiteTurn : !isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if(selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }

    private void UpdateMouseOver()
    {
        //If it is my turn
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int) (hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);

        }
        else
        {
            mouseOver.x = mouseOver.y = -1;
        }
    }

    public void UpdatePieceDrag(Piece p)
    {
        //If it is my turn
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void SelectPiece(int x, int y)
    {
        //If out of bounds
        if(x < 0 || x > pieces.GetLength(0) || y < 0 || y > pieces.GetLength(1))
        {
            return;
        }
        //Debug.Log("SELECT " + forcedPieces.Count);
        Piece p = pieces[x, y];
        
        if (p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
        }
        
        /*
        if(p != null && p.isWhite == isWhite)
        {
            //If there is impossible to kill any piece
            if(forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                //Look for the piece under our of forcedPiece List
                if(forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }

                selectedPiece = p;
                startDrag = mouseOver;
            }
            
            //Debug.Log(selectedPiece.name);
        }*/
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanPossibleMove();
        Debug.Log("TRY MOVE " + forcedPieces.Count);
        //Multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //Check out of bounds
        if(x2  <0 || x2 > pieces.GetLength(0) || y2 < 0 || y2 > pieces.GetLength(1))
        {
            if(selectedPiece != null)
            {
                MovePiece(selectedPiece, x1, y1);
            }

            startDrag = Vector2.zero;
            selectedPiece = null;

            return;
        }

        if(selectedPiece != null)
        {
            //If piece not move
            if(endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
            //If its a valid move
            if(selectedPiece.ValidMove(pieces, x1,y1,x2,y2))
            {
                //Kill anything?
                //If its a jump
                if(Mathf.Abs(x1 - x2) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        hasKill = true;
                    }
                }

                //Have killed any thing?
                if(forcedPieces.Count != 0 && !hasKill)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }
                

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }

    private void EndTurn()
    {
        selectedPiece = null;
        startDrag = Vector2.zero;
        hasKill = false;
        isWhiteTurn = !isWhiteTurn;
        CheckVictory();
        Debug.Log("END TURN " + forcedPieces.Count);
    }

    private void CheckVictory()
    {
            
    }

    private List<Piece> ScanPossibleMove()
    {
        forcedPieces = new List<Piece>();

        //Check all pieces
        //Debug.Log(pieces.Length.ToString());
        Debug.Log(pieces.GetLength(0) + "|" + pieces.GetLength(1));
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //Check if a single piece has the same color with current turn
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);
            }
        }
        return forcedPieces;
            
    }

    private void GenerateBoard()
    {
        //Generate white team
        for (int y = 0; y < 3; y++)
        {
            bool ood = y % 2 == 0;
            for (int x = 0; x < 8; x+= 2)
            {
                //Generate out Piece
                GeneratePiece(ood ? x : (x + 1),y);
            }
        }

        //Generate black team
        for (int y = 7; y > 4; y--)
        {
            bool ood = y % 2 == 0;
            for (int x = 0; x < 8; x += 2)
            {
                //Generate out Piece
                GeneratePiece(ood ? x : (x + 1), y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        bool isWhitePiece = y < 3;
        GameObject goj = Instantiate(piecePrefabs[isWhitePiece ? 0 : 1]) as GameObject;
        goj.transform.SetParent(transform);
        Piece p = goj.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        if (p == null)
            return;
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}
