using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    [SerializeField] Player[] players;
    Player currentPlayer;
    GameBoard gameBoard;

    private void Start()
    {
      gameBoard = FindObjectOfType<GameBoard>();
      currentPlayer = players[0];
    }

    public Player[] GetPlayers()
    {
      return players;
    }

    public void RegisterClickedCell(GameCell cell)
    {
      cell.MarkCell(currentPlayer);
      SwapPlayers();
    }

    private void SwapPlayers()
    {
      currentPlayer = currentPlayer == players[0] ? players[1] : players[0];
    }
}
