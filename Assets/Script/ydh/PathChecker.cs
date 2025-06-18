using Photon.Pun;
using UnityEngine;

public static class PathChecker
{
    public static bool IsPathAvailable(TileContext tileContext)
    {
        if (tileContext == null) return false;

        int width = tileContext.Width;
        int height = tileContext.Height;
        TileBehaviour[,] tiles = new TileBehaviour[width, height];

        foreach (Transform t in tileContext.TileParent)
        {
            TileBehaviour tb = t.GetComponent<TileBehaviour>();
            if (tb == null) continue;

            int x = tb.CoordX;
            int z = tb.CoordZ;

            tiles[x, z] = tb;
        }

        Vector2Int? start = null, end = null;
        bool isMaster = PhotonNetwork.IsMasterClient;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var tile = tiles[x, z];
                if (tile == null) continue;

                if (tile._tileState == TileState.StartPoint)
                {
                    if (tile._accessType == TileAccessType.Everyone ||
                        (tile._accessType == TileAccessType.MasterOnly && isMaster) ||
                        (tile._accessType == TileAccessType.ClientOnly && !isMaster))
                    {
                        start = new Vector2Int(x, z);
                    }
                }

                if (tile._tileState == TileState.EndPoint)
                {
                    end = new Vector2Int(x, z);
                }
            }
        }

        if (start == null || end == null) return false;

        var pathfinder = new AStarPathfinder(tiles, width, height);
        var path = pathfinder.FindPath(start.Value, end.Value);
        return path != null && path.Count > 0;
    }
}