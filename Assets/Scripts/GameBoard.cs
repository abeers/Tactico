using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameCell[] cells;
    Player[] players;

    private void Awake()
    {
      players = FindObjectOfType<GameStatus>().GetPlayers();
    }

    public int GetCellIndex(GameCell cell)
    {
      return Array.IndexOf(cells, cell);
    }

    public GameCell[] GetUnoccupiedCells()
    {
      return cells.Where(cell => cell.GetComponent<Button>().enabled).ToArray<GameCell>();
    }

    public void ShiftBoardPositive()
    {
      for (int i = 0; i < cells.Length; i++)
      {
        if (i < cells.Length - 3)
        {
          cells[i].CopyCell(cells[i + 3]);
        }
        else
        {
          cells[i].ResetCell();
        }
      }
    }

    public void ShiftBoardNegative()
    {
      for (int i = 0; i < cells.Length; i++)
      {
        if (i > 2)
        {
          cells[i].CopyCell(cells[i - 3]);
        }
        else
        {
          cells[i].ResetCell();
        }
      }
    }

    public void ResetCells()
    {
      foreach (GameCell cell in cells)
      {
        cell.ResetCell();
      }
    }

    public void ResetDefenses()
    {
      foreach (GameCell cell in cells)
      {
        cell.SetIsDefended(false);
      }
    }

    public void DisableCells()
    {
      foreach (GameCell cell in cells)
      {
        cell.DisableCell();
      }
    }
}
