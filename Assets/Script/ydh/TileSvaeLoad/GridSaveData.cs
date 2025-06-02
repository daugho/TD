using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public int x;
    public int z;
    public TileState state;
}

[Serializable]
public class GridData
{
    public int width;
    public int height;
    public List<TileData> tiles = new();
}
