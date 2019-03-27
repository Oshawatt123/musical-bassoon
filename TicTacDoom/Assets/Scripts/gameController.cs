using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{

    public int turn;
    public Image turnImage;

    private enum gridValue
    {
        O_VAL = 0,
        X_VAL = 1,
        NO_VAL = -10000,
    };

    public GameObject[] buttons;
    public int[] markedTiles;
    public Sprite[] buttonSprites;

    private int humanPlayer;
    private int aiPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // initialise the array used for keeping track of the grid
        for (int i = 0; i < buttons.Length; i++)
        {
            markedTiles[i] = (int)gridValue.NO_VAL;
        }
        turn = (int)gridValue.X_VAL;
        turnImage.sprite = buttonSprites[turn];

        humanPlayer = turn;
        if(humanPlayer == (int)gridValue.X_VAL)
        {
            aiPlayer = (int)gridValue.O_VAL;
        }
        else
        {
            aiPlayer = (int)gridValue.X_VAL;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(turn == aiPlayer)
        {
            ButtonClick(getBestMove(markedTiles, aiPlayer, 0).movePosition);
            turn = humanPlayer;
        }
        else
        {
            //Debug.Log("Waiting for player to press a button");
        }
    }

    public void ButtonClick(int Number)
    {
        if (markedTiles[Number] == (int)gridValue.NO_VAL)
        {
            markedTiles[Number] = turn + 1;
            Debug.Log(markedTiles[Number]);
            buttons[Number].GetComponent<Image>().sprite = buttonSprites[turn];
            if (turn == (int)gridValue.O_VAL)
            {
                turn = (int)gridValue.X_VAL;
            }
            else
            {
                turn = (int)gridValue.O_VAL;
            }
            if (WinnerCheck(markedTiles) == 1)
            {
                Debug.Log("O wins!");
            } else if (WinnerCheck(markedTiles) == 2)
            {
                Debug.Log("X wins!");
            } else if (WinnerCheck(markedTiles) == 0)
            {
                Debug.Log("Its a draw mate!");
            }

        }
        turnImage.sprite = buttonSprites[turn];
    }

    // return 1 for O, 2 for X, 0 for draws, and -1 for no victory condition met
    private int WinnerCheck(int[] board)
    {
        // draw check
        int count = 0;
        for (int i = 0; i < board.Length; i++)
        {
            if(board[i] != (int)gridValue.NO_VAL)
            {
                count += 1;
            }
        }
        Debug.Log(count + " : " + board.Length);
        if(count == board.Length)
        {
            return 0;
        }


        // Horizontal
        int w1 = board[0] + board[1] + board[2]; // Top
        int w2 = board[3] + board[4] + board[5]; // Middle
        int w3 = board[6] + board[7] + board[8]; // Bottom

        // Vertical
        int w4 = board[0] + board[3] + board[6]; // Left
        int w5 = board[1] + board[4] + board[7]; // Middle
        int w6 = board[2] + board[5] + board[8]; // Right

        // Diagonal
        int w7 = board[0] + board[4] + board[8]; // TL to BR
        int w8 = board[2] + board[4] + board[6]; // TR to BL

        int[] solutions = new int[] { w1, w2, w3, w4, w5, w6, w7, w8 };
        for (int i = 0; i < solutions.Length; i++)
        {
            // could just == 3
            if (solutions[i] == 3 * ((int)gridValue.O_VAL + 1))
            {
                return 1;
                // could just == 6
            } else if (solutions[i] == 3 * ((int)gridValue.X_VAL + 1))
            {
                return 2;
            }
        }
        return -1;
    }

    private AIClass getBestMove(int[] tempBoard, int player, int depth)
    {
        // fail-safe (I thought)
        if(depth >= 8)
        {
            return new AIClass(0);
        }
        // base case, check for end state
        if(WinnerCheck(tempBoard) == aiPlayer + 1) // ai wins
        {
            return new AIClass(10);
        }
        else if (WinnerCheck(tempBoard) == humanPlayer + 1) // human wins
        {
            return new AIClass(-10);
        }
        else if (WinnerCheck(tempBoard) == 0) // draw
        {
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            return new AIClass(0);
        }

        List<AIClass> moveList = new List<AIClass>();
        // for every tile in the grid that isn't occupied, make a move
        for (int i = 0; i < tempBoard.Length; i++)
        {
            if(depth == 0)
            {
                Debug.Log("Board index: " + i);
            }
            if (tempBoard[i] == (int)gridValue.NO_VAL)
            {
                AIClass newMove = new AIClass();
                newMove.movePosition = i;

                tempBoard[i] = player + 1;
                
                // if we are the AI, we want to get the score of the next move the human would make
                if(player == aiPlayer)
                {
                    //Debug.Log("AI player places at index " + i);
                    newMove.score = getBestMove(tempBoard, humanPlayer, depth+1).score;
                    
                }
                else // get the score of the next move that the AI would make
                {
                    //Debug.Log("Human player palces at index " + i);
                    newMove.score = getBestMove(tempBoard, aiPlayer, depth+1).score;
                    
                }

                moveList.Add(newMove);
                if(depth == 0)
                {
                    Debug.Log("Adding move of score " + newMove.score + ". New length: " + moveList.Count);
                }
                tempBoard[i] = (int)gridValue.NO_VAL;
            }
        }

        // get the best move possible from the recursion
        int bestMove = 0;

        // ai will always take the biggest score
        if(player == aiPlayer)
        {
            int bestScore = -1000000; // we start with an extreme number so we ensure that we move in the correct direction
                                      //  in this condition we want to get a larger score
            for (int i = 0; i < moveList.Count; i++)
            {
                if(moveList[i].score > bestScore)
                {
                    bestMove = i;
                    bestScore = moveList[i].score;
                }
            }
        }
        else // the player will always take the lowest score
        {
            int bestScore = 1000000;
            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].score < bestScore)
                {
                    bestMove = i;
                    bestScore = moveList[i].score;
                }
            }
        }
        if(depth == 0)
        {
            Debug.Log("Move index: " + bestMove + " length of move array " + moveList.Count);
        }
        Debug.Log("Depth: " + depth);
        return moveList[bestMove];
    }
}
