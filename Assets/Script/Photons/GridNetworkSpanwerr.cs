using Photon.Pun;
using UnityEngine;

public class GridNetworkSpanwerr : MonoBehaviour
{
    public int xCount = 4;
    public int zCount = 4;
    public GameObject tilePrefab; // Resources 폴더 안에 위치한 프리팹
    public float tileSize = 1f;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int x = 0; x < xCount; x++)
            {
                for (int z = 0; z < zCount; z++)
                {
                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                    PhotonNetwork.Instantiate("TilePrefab", pos, Quaternion.identity);
                }
            }
        }
    }
}
