using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    /// <summary>
    /// 0: white|1: black
    /// </summary>
    public GameObject[] piecePrefabs;

    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private Piece selectedPiece;

    private bool isWhite;
    private bool isWhiteTurn;

    private void Start()
    {
        isWhiteTurn = true;
        GenerateBoard();
    }

    private void Update()
    {
        UpdateMouseOver();
        //Debug.Log(mouseOver);

        //If is my turn
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
        if(x < 0 || x >= pieces.Length || y < 0 || y > pieces.Length)
        {
            return;
        }

        Piece p = pieces[x, y];
        if(p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        //Multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //Check out of bounds
        if(x2  <0 || x2 > pieces.Length || y2 < 0 || y2 > pieces.Length)
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
                        Destroy(p);
                    }
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
        }
    }

    private void EndTurn()
    {
        selectedPiece = null;
        startDrag = Vector2.zero;
        
        isWhiteTurn = !isWhiteTurn;
        CheckVictory(); 
    }

    private void CheckVictory()
    {
            
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
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}
