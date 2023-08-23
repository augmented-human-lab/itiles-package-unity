using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Master Tile", menuName = "Itiles/Master Tile")]
public class MasterTile : ScriptableObject
{
    public string macAddress;
    public string status;
    public byte[] receivedData;
    public int noOfTilesPaired;
    public int[] tileIds;

    public List<StandardTile> children;
}
