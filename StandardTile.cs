using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Standard Tile", menuName = "Itiles/Standard Tile")]
public class StandardTile : ScriptableObject
{
    public string tileId;
    public string macAddress;
    public bool status;
}
