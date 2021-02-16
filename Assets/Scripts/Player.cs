using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Color myColor;
    [SerializeField] string myName;

    public Color GetPlayerColor()
    {
      return myColor;
    }

    public string GetPlayerName()
    {
      return myName;
    }
}
