using System;
using System.Collections.Generic;

[Serializable]
public class TileData
{
    public int x;
    public int z;
    public TileState state;
    public TileAccessType access;
}

[Serializable]
public class GridData
{
    public int width;
    public int height;
    public List<TileData> tiles = new();
}
