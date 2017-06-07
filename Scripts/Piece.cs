using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public bool isWhite;
    public bool isKing;

    public bool IsForceToMove(Piece[,]  board, int x, int y)
    {
        if (isWhite || isKing)    //If its white team or its king
        {
            //Top Left
            if (x > 1 && y < 6)
            {
                Piece p = board[x - 1, y + 1];
                //If there is a piece having different color with this
                if (p != null && p.isWhite != isWhite)
                {
                    //Check if its possible to jump
                    if (board[x - 2, y + 2] != null)
                        return true;
                }
            }

            //Top Right
            if (x < 6 && y < 6)
            {
                Piece p = board[x + 1, y + 1];
                //If there is a piece having different color with this
                if (p != null && p.isWhite != isWhite)
                {
                    //Check if its possible to jump
                    if (board[x + 2, y + 2] != null)
                        return true;
                }
            }
        }

        if(!isWhite || isKing)  //If its black team or its King
        {

            //Bot Left
            if (x > 1 && y > 1)
            {
                Piece p = board[x - 1, y - 1];
                //If there is a piece having different color with this
                if (p != null && p.isWhite != isWhite)
                {
                    //Check if its possible to jump
                    if (board[x - 2, y - 2] != null)
                        return true;
                }
            }
            //Bot Right
            if (x < 6 && y > 1)
            {
                Piece p = board[x + 1, y - 1];
                //If there is a piece having different color with this
                if (p != null && p.isWhite != isWhite)
                {
                    //Check if its possible to jump
                    if (board[x + 2, y - 2] != null)
                        return true;
                }
            }
            /*
            
            */

        }
        return false;
    }

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        //If moving on top of other piece
        if(board[x2, y2] != null)
        {
            return false;
        }

        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;
        //Debug.Log("deltaMoev = " + deltaMove + "|deltaMoveY = " + deltaMoveY);
        if(isWhite || isKing)
        {
            if(deltaMove == 1)
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if(deltaMove == 2)
            {
                if(deltaMoveY == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    //If color of the piece were jumping is not the same with
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        if (!isWhite || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    //If color of the piece were jumping is not the same with
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        return false;
    }
}
