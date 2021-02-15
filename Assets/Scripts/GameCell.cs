using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCell : MonoBehaviour
{
    GameStatus gameStatus;
    GameBoard gameBoard;
    int index;

    private void Start()
    {
      gameStatus = FindObjectOfType<GameStatus>();
      gameBoard = FindObjectOfType<GameBoard>();
      index = gameBoard.GetCellIndex(this);
    }

    public void OnCellClick()
    {
      gameStatus.RegisterClickedCell(this);
    }

    public void MarkCell(Player player)
    {
      GetComponent<Button>().enabled = false;
      UpdateDisplay(player.getPlayerColor());
    }

    public void ResetCell()
    {
      GetComponent<Button>().enabled = true;
      UpdateDisplay(Color.white);
    }

    private void UpdateDisplay(Color color)
    {
      GetComponent<Image>().color = color;
    }
}
