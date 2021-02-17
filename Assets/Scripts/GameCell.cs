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

    public int GetCellIndex()
    {
      return index;
    }

    public void OnCellClick()
    {
      gameStatus.RegisterClickedCell(this);
    }

    public void MarkCell(Player player)
    {
      DisableCell();
      UpdateDisplay(player.GetPlayerColor());
    }

    public void ResetCell()
    {
      GetComponent<Button>().enabled = true;
      UpdateDisplay(Color.white);
    }

    public void DisableCell()
    {
      GetComponent<Button>().enabled = false;
    }

    private void UpdateDisplay(Color color)
    {
      GetComponent<Image>().color = color;
    }
}
