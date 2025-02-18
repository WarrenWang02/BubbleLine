using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridObjectsData", menuName = "Game/GridObjectsAsset")]
public class GridObjectsAsset : ScriptableObject
{
    public Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();
}
