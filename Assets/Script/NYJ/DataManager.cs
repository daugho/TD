using System.Collections.Generic;
using UnityEngine;

public struct TurretData
{
    public string Name;
    public string Type;
    public string Bullet;
    public float BulletSpeed;
    public float Atk;
    public float AtkSpeed;
    public int Price;
    public int Upgrade;
    public string fireEffectPath;
    public string hitEffectPath;
}
public class DataManager
{
    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
            }

            return _instance;
        }
    }

    private Dictionary<string, TurretData> _turretDatas = new Dictionary<string, TurretData>();
    public Dictionary<string, TurretData> TurretDatas
    {
        get { return _turretDatas; }
    }

    public TurretData GetTurretData(string name) { return _turretDatas[name]; }

    public void LoadTurretData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/TurretTable");

        string text = textAsset.text;

        string[] rowData = text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            if (rowData[i].Length == 0)
                break;

            string[] datas = rowData[i].Split(",");

            TurretData data;
            data.Name = datas[0];
            data.Type = datas[1];
            data.Bullet = datas[2];
            data.BulletSpeed = float.Parse(datas[3]);
            data.Atk = float.Parse(datas[4]);
            data.AtkSpeed = float.Parse(datas[5]);
            data.Price = int.Parse(datas[6]);
            data.Upgrade = int.Parse(datas[7]);
            data.fireEffectPath = datas[8];
            data.hitEffectPath = datas[9];

            _turretDatas.Add(data.Name, data);
        }
    }
}

