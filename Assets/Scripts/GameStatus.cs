using System;
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
    bool isOver;
    GameBoard gameBoard;
    string[] gameState;
    int plusMinus;

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

    public enum PlayerMode { LocalMultiplayer, SinglePlayer }
    private PlayerMode playerMode;
    private int numPlayerModes = System.Enum.GetValues(typeof(PlayerMode)).Length;

    public enum GameLength { Quick, Warpath }
    private GameLength gameLength;
    private int numGameLengths = System.Enum.GetValues(typeof(GameLength)).Length;

    private delegate void UpdateStateDelegate(GameCell cell);
    private UpdateStateDelegate[] UpdateStateDelegates;

    private delegate void SwapPlayersDelegate();
    private SwapPlayersDelegate[,] SwapPlayersDelegates;

    private delegate void MarkCellDelegate(GameCell cell);
    private MarkCellDelegate[] MarkCellDelegates;

    private delegate bool CheckWinDelegate(int cellIndex);
    private CheckWinDelegate[] CheckWinDelegates;

    private delegate bool CheckTieDelegate();
    private CheckTieDelegate[] CheckTieDelegates;

    private delegate void WinResultDelegate(int cellIndex);
    private WinResultDelegate[] WinResultDelegates;

    private delegate void TieResultDelegate();
    private TieResultDelegate[] TieResultDelegates;

    void Awake()
    {
        UpdateStateDelegates = new UpdateStateDelegate[(int)numGameModes];
        SwapPlayersDelegates = new SwapPlayersDelegate[(int)numPlayerModes, (int)numGameModes];
        MarkCellDelegates = new MarkCellDelegate[(int)numGameModes];
        CheckWinDelegates = new CheckWinDelegate[(int)numGameModes];
        CheckTieDelegates = new CheckTieDelegate[(int)numGameModes];
        WinResultDelegates = new WinResultDelegate[(int)numGameLengths];
        TieResultDelegates = new TieResultDelegate[(int)numGameLengths];

        UpdateStateDelegates[(int)GameMode.Normal] = UpdateStateNormal;
        UpdateStateDelegates[(int)GameMode.Tactico] = UpdateStateTactico;
        SwapPlayersDelegates[(int)PlayerMode.LocalMultiplayer, (int)GameMode.Normal] = SwapPlayersLocalMultiplayerNormal;
        SwapPlayersDelegates[(int)PlayerMode.LocalMultiplayer, (int)GameMode.Tactico] = SwapPlayersLocalMultiplayerTactico;
        SwapPlayersDelegates[(int)PlayerMode.SinglePlayer, (int)GameMode.Normal] = SwapPlayersSinglePlayerNormal;
        SwapPlayersDelegates[(int)PlayerMode.SinglePlayer, (int)GameMode.Tactico] = SwapPlayersSinglePlayerTactico;
        MarkCellDelegates[(int)GameMode.Normal] = MarkCellNormal;
        MarkCellDelegates[(int)GameMode.Tactico] = MarkCellTactico;
        CheckWinDelegates[(int)GameMode.Normal] = CheckWinNormal;
        CheckWinDelegates[(int)GameMode.Tactico] = CheckWinTactico;
        CheckTieDelegates[(int)GameMode.Normal] = CheckTieNormal;
        CheckTieDelegates[(int)GameMode.Tactico] = CheckTieTactico;
        WinResultDelegates[(int)GameLength.Quick] = WinResultQuick;
        WinResultDelegates[(int)GameLength.Warpath] = WinResultWarpath;
        TieResultDelegates[(int)GameLength.Quick] = TieResultQuick;
        TieResultDelegates[(int)GameLength.Warpath] = TieResultWarpath;

        gameMode = (GameMode)PlayerPrefs.GetInt("gameMode");
        playerMode = (PlayerMode)PlayerPrefs.GetInt("playerMode");
        gameLength = (GameLength)PlayerPrefs.GetInt("gameLength");

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
      ResetState();
      gameBoard.ResetCells();
      resultText.text = "";

      isOver = false;

      currentPlayer.BecomeDefender();
      players[1].BecomeAttacker();

      plusMinus = 0;
    }

    public void RegisterClickedCell(GameCell cell)
    {
      MarkCellDelegates[(int)gameMode](cell);
      UpdateStateDelegates[(int)gameMode](cell);

      if (!isOver)
      {
        SwapPlayersDelegates[(int)playerMode, (int)gameMode]();
      }
    }

    public GameCell GetRandomAISelection()
    {
      GameCell[] unoccupiedCells = gameBoard.GetUnoccupiedCells();
      int cellIndex = UnityEngine.Random.Range(0, unoccupiedCells.Length);
      return unoccupiedCells[cellIndex];
    }

    private void ShiftStatePositive()
    {
      for (int i = 0; i < gameState.Length; i++)
      {
        if (i < gameState.Length - 3)
        {
          gameState[i] = gameState[i + 3];
        }
        else
        {
          gameState[i] = null;
        }
      }
    }

    public void ShiftStateNegative()
    {
      for (int i = gameState.Length - 1; i >=0; i--)
      {
        if (i > 2)
        {
          gameState[i] = gameState[i - 3];
        }
        else
        {
          gameState[i] = null;
        }
      }
    }

    public void ResetState()
    {
      gameState = new string[9];
    }

    public void UpdateStateNormal(GameCell cell)
    {
      int cellIndex = cell.GetCellIndex();
      gameState[cellIndex] = currentPlayer.GetPlayerName();

      if (CheckWinDelegates[(int)gameMode](cellIndex))
      {
        WinResultDelegates[(int)gameLength](cellIndex);
      }
      else if (CheckTieDelegates[(int)gameMode]())
      {
        TieResultDelegates[(int)gameLength]();
      }
    }

    public void UpdateStateTactico(GameCell cell)
    {
      if (!currentPlayer.GetIsDefending() && !cell.GetIsDefended())
      {
        UpdateStateNormal(cell);
      }
    }

    private void SwapPlayers()
    {
      currentPlayer = currentPlayer == players[0] ? players[1] : players[0];
    }

    private void SwapPlayersLocalMultiplayerNormal()
    {
      SwapPlayers();
    }

    private void SwapPlayersLocalMultiplayerTactico()
    {
      if (currentPlayer.GetIsDefending())
      {
        currentPlayer.SwapDefending();
        SwapPlayers();
      }
      else
      {
        currentPlayer.SwapDefending();
      }
    }

    private void SwapPlayersSinglePlayerNormal()
    {
      SwapPlayers();
      if (currentPlayer == players[1])
      {
        RegisterClickedCell(GetRandomAISelection());
      }
    }

    private void SwapPlayersSinglePlayerTactico()
    {
      if (currentPlayer.GetIsDefending())
      {
        currentPlayer.SwapDefending();
        SwapPlayers();
      }
      else
      {
        currentPlayer.SwapDefending();
      }

      if (currentPlayer == players[1])
      {
        RegisterClickedCell(GetRandomAISelection());
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
        Debug.Log("Blocked!");
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

    private void WinResultQuick(int cellIndex)
    {
      resultText.text = currentPlayer.GetPlayerName() + " wins!";
      gameBoard.DisableCells();
      isOver = true;
    }

    private void WinResultWarpath(int cellIndex)
    {
      while (cellIndex >= 0 && cellIndex < gameState.Length && CheckWinDelegates[(int)gameMode](cellIndex))
      {
        Debug.Log(plusMinus);
        if (Math.Abs(plusMinus) >= 3)
        {
          WinResultQuick(cellIndex);
          break;
        }
        else if (currentPlayer == players[0])
        {
          gameBoard.ShiftBoardPositive();
          ShiftStatePositive();
          plusMinus++;
          cellIndex -= 3;
        }
        else
        {
          gameBoard.ShiftBoardNegative();
          ShiftStateNegative();
          plusMinus--;
          cellIndex += 3;
        }
      }
    }

    private void TieResultQuick()
    {
      resultText.text = "It's a tie!";
      isOver = true;
    }

    private void TieResultWarpath()
    {
      gameBoard.ResetCells();
      ResetState();
    }
}
