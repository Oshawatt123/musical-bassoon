using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{

    public int turn;

    private enum gridValue
    {
        O_VAL = 0,
        X_VAL = 1,
        NO_VAL = -10000,
    };

    public GameObject[] buttons;
    public int[] markedTiles;
    public Sprite[] buttonSprites; 

    // Start is called before the first frame update
    void Start()
    {
        // initialise the array used for keeping track of the grid
        for (int i = 0; i < buttons.Length; i++)
        {
            markedTiles[i] = (int)gridValue.NO_VAL;
        }
        turn = (int)gridValue.X_VAL;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClick(int Number)
    {
        if(markedTiles[Number] == (int)gridValue.NO_VAL)
        {
            markedTiles[Number] = turn + 1;
            buttons[Number].GetComponent<Image>().sprite = buttonSprites[turn];
            Debug.Log("Turn taken");
            if(turn == (int)gridValue.O_VAL)
            {
                turn = (int)gridValue.X_VAL;
            }
            else
            {
                turn = (int)gridValue.O_VAL;
            }
            WinnerCheck();
        }
    }

    private void WinnerCheck()
    {
        // Horizontal
        int w1 = markedTiles[0] + markedTiles[1] + markedTiles[2]; // Top
        int w2 = markedTiles[3] + markedTiles[4] + markedTiles[5]; // Middle
        int w3 = markedTiles[6] + markedTiles[7] + markedTiles[8]; // Bottom

        // Vertical
        int w4 = markedTiles[0] + markedTiles[3] + markedTiles[6]; // Left
        int w5 = markedTiles[1] + markedTiles[4] + markedTiles[7]; // Middle
        int w6 = markedTiles[2] + markedTiles[5] + markedTiles[8]; // Right

        // Diagonal
        int w7 = markedTiles[0] + markedTiles[4] + markedTiles[8]; // TL to BR
        int w8 = markedTiles[2] + markedTiles[4] + markedTiles[6]; // TR to BL

        int[] solutions = new int[] { w1, w2, w3, w4, w5, w6, w7, w8 };
        for (int i = 0; i < solutions.Length; i++)
        { 
            // could just == 3
            if(solutions[i] == 3*(int)gridValue.O_VAL)
            {
                Debug.Log("VICTORY FOR PLAYER O");
                return;
            // could just == 6
            }else if (solutions[i] == 6*(int)gridValue.X_VAL)
            {
                Debug.Log("VICTORY FOR PLAYER X");
                return;
            }
        }
    }
}
