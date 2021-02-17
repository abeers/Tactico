using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Color myColor;
    [SerializeField] string myName;
    private bool isDefending = true;

    public Color GetPlayerColor()
    {
      return myColor;
    }

    public string GetPlayerName()
    {
      return myName;
    }

    public bool GetIsDefending()
    {
      return isDefending;
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
