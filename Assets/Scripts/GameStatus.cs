using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    [SerializeField] Player[] players;
    Player currentPlayer;
    GameBoard gameBoard;
    string[] gameState;

    readonly int[][][] winningCombosByCell = new int[9][][]
    {
      new int[][]
      {
        new int[] { 1, 2 },
        new int[] { 3, 6 },
        new int[] { 4, 8 }
      },
      new int[][]
      {
        new int[] { 0, 2 },
        new int[] { 4, 7 }
      },
      new int[][]
      {
        new int[] { 0, 1 },
        new int[] { 4, 6 },
        new int[] { 5, 8 }
      },
      new int[][]
      {
        new int[] { 0, 6 },
        new int[] { 4, 5 }
      },
      new int[][]
      {
        new int[] { 0, 8 },
        new int[] { 1, 7 },
        new int[] { 2, 6 },
        new int[] { 3, 5 }
      },
      new int[][]
      {
        new int[] { 2, 8 },
        new int[] { 3, 4 }
      },
      new int[][]
      {
        new int[] { 0, 3 },
        new int[] { 2, 4 },
        new int[] { 7, 8 }
      },
      new int[][]
      {
        new int[] { 1, 4 },
        new int[] { 6, 8 }
      },
      new int[][]
      {
        new int[] { 0, 4 },
        new int[] { 2, 5 },
        new int[] { 6, 7 }
      }
    };

    private void Start()
    {
      gameBoard = FindObjectOfType<GameBoard>();
      ResetGame();
    }

    // Getters
    public Player[] GetPlayers()
    {
      return players;
    }

    // Set game to initial state
    public void ResetGame()
    {
      currentPlayer = players[0];
      gameState = new string[9];
      gameBoard.ResetCells();
    }

    public void RegisterClickedCell(GameCell cell)
    {
      UpdateGameState(cell);
      cell.MarkCell(currentPlayer);

      SwapPlayers();
    }

    public void UpdateGameState(GameCell cell)
    {
      int cellIndex = cell.GetCellIndex();
      gameState[cellIndex] = currentPlayer.GetPlayerName();

      if (CheckForWin(cellIndex))
      {
        Debug.Log(currentPlayer.GetPlayerName() + " wins!");
      }
      else if (CheckForTie())
      {
        Debug.Log("It's a tie!");
      }
    }

    private void SwapPlayers()
    {
      currentPlayer = currentPlayer == players[0] ? players[1] : players[0];
    }

    private bool CheckForWin(int cellIndex)
    {
      int[][] combosToCheck = winningCombosByCell[cellIndex];
      string currentPlayerName = currentPlayer.GetPlayerName();
      return combosToCheck.Any(combo => combo.All(cell => gameState[cell] == currentPlayerName));
    }

    private bool CheckForTie()
    {
      return !gameState.Any(cell => cell == null);
    }
}
