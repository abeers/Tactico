using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void ResetCells()
    {
      foreach (GameCell cell in cells)
      {
        cell.ResetCell();
      }
    }
}
