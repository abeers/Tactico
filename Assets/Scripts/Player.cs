using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] string myName;
    [SerializeField] int myColor;

    private bool isDefending = true;

    public Color GetPlayerColor()
    {
      return colors[myColor];
    }

    public string GetPlayerName()
    {
      return myName;
    }

    public bool GetIsDefending()
    {
      return isDefending;
    }

    public void SetName(string name)
    {
      myName = name;
    }

    public void SetColor(int colorIndex)
    {
      myColor = colorIndex;
    }

    public void BecomeAttacker()
    {
      isDefending = false;
    }

    public void BecomeDefender()
    {
      isDefending = true;
    }

    public void SwapDefending()
    {
      isDefending = !isDefending;
    }
}
