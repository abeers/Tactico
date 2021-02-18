using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    [SerializeField] Text resultText;
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

    public enum GameMode { Normal, Tactico }
    private GameMode gameMode;
    private int numGameModes = System.Enum.GetValues(typeof(GameMode)).Length;

    private delegate void UpdateDelegate(GameCell cell);
    private UpdateDelegate[] UpdateDelegates;

    private delegate void SwapPlayersDelegate();
    private SwapPlayersDelegate[] SwapPlayersDelegates;

    private delegate void MarkCellDelegate(GameCell cell);
    private MarkCellDelegate[] MarkCellDelegates;

    private delegate bool CheckWinDelegate(int cellIndex);
    private CheckWinDelegate[] CheckWinDelegates;

    private delegate bool CheckTieDelegate();
    private CheckTieDelegate[] CheckTieDelegates;

    void Awake()
    {
        UpdateDelegates = new UpdateDelegate[(int)numGameModes];
        SwapPlayersDelegates = new SwapPlayersDelegate[(int)numGameModes];
        MarkCellDelegates = new MarkCellDelegate[(int)numGameModes];
        CheckWinDelegates = new CheckWinDelegate[(int)numGameModes];
        CheckTieDelegates = new CheckTieDelegate[(int)numGameModes];

        UpdateDelegates[(int)GameMode.Normal] = UpdateNormalState;
        UpdateDelegates[(int)GameMode.Tactico] = UpdateTacticoState;
        SwapPlayersDelegates[(int)GameMode.Normal] = SwapPlayersNormal;
        SwapPlayersDelegates[(int)GameMode.Tactico] = SwapPlayersTactico;
        MarkCellDelegates[(int)GameMode.Normal] = MarkCellNormal;
        MarkCellDelegates[(int)GameMode.Tactico] = MarkCellTactico;
        CheckWinDelegates[(int)GameMode.Normal] = CheckWinNormal;
        CheckWinDelegates[(int)GameMode.Tactico] = CheckWinTactico;
        CheckTieDelegates[(int)GameMode.Normal] = CheckTieNormal;
        CheckTieDelegates[(int)GameMode.Tactico] = CheckTieTactico;

        gameMode = (GameMode)PlayerPrefs.GetInt("gameMode");

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
      currentPlayer.SetName(PlayerPrefs.GetString("playerName"));
      currentPlayer.SetColor(PlayerPrefs.GetInt("playerColor"));
      currentPlayer.BecomeDefender();
      players[1].BecomeAttacker();
      gameState = new string[9];
      gameBoard.ResetCells();
      resultText.text = "";
    }

    public void RegisterClickedCell(GameCell cell)
    {
      UpdateDelegates[(int)gameMode](cell);
      MarkCellDelegates[(int)gameMode](cell);
      SwapPlayersDelegates[(int)gameMode]();
    }

    public void UpdateNormalState(GameCell cell)
    {
      int cellIndex = cell.GetCellIndex();
      gameState[cellIndex] = currentPlayer.GetPlayerName();

      if (CheckWinDelegates[(int)gameMode](cellIndex))
      {
        resultText.text = currentPlayer.GetPlayerName() + " wins!";
        gameBoard.DisableCells();
      }
      else if (CheckTieDelegates[(int)gameMode]())
      {
        resultText.text = "It's a tie!";
      }
    }

    public void UpdateTacticoState(GameCell cell)
    {
      if (!currentPlayer.GetIsDefending() && !cell.GetIsDefended())
      {
        UpdateNormalState(cell);
      }
    }

    private void SwapPlayersNormal()
    {
      currentPlayer = currentPlayer == players[0] ? players[1] : players[0];
    }

    private void SwapPlayersTactico()
    {
      if (currentPlayer.GetIsDefending())
      {
        currentPlayer.SwapDefending();
        SwapPlayersNormal();
      }
      else
      {
        currentPlayer.SwapDefending();
      }
    }

    private void MarkCellNormal(GameCell cell)
    {
      cell.MarkCell(currentPlayer);
    }

    private void MarkCellTactico(GameCell cell)
    {
      if (currentPlayer.GetIsDefending())
      {
        cell.SetIsDefended(true);
      }
      else if (!cell.GetIsDefended())
      {
        MarkCellNormal(cell);
        gameBoard.ResetDefenses();
      }
      else
      {
        gameBoard.ResetDefenses();
      }
    }

    private bool CheckWinNormal(int cellIndex)
    {
      int[][] combosToCheck = winningCombosByCell[cellIndex];
      string currentPlayerName = currentPlayer.GetPlayerName();
      return combosToCheck.Any(combo => combo.All(cell => gameState[cell] == currentPlayerName));
    }

    private bool CheckWinTactico(int cellIndex)
    {
      return CheckWinNormal(cellIndex) || gameState.Count<string>(cell => cell == currentPlayer.GetPlayerName()) >= 5;
    }

    private bool CheckTieNormal()
    {
      return !gameState.Any(cell => cell == null);
    }

    private bool CheckTieTactico()
    {
      return gameState.Count<string>(cell => cell == null) == 1;
    }
}
