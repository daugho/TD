using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public struct TurretData
{
    public TowerTypes Name;
    public string GachaPath;
    public AttackType Type;
    public TowerRarity Rarity;
    public string Bullet;
    public float BulletSpeed;
    public int Atk;
    public float AtkSpeed;
    public int Range;
    public int Price;
    public int Upgrade;
    public string FireEffectPath;
    public string HitEffectPath;
    public int CommonSummonTicket;
    public int RareSummonTicket;
}

public enum TowerRarity
{
    Normal, Rare, Epic, Legendary
}
public enum TowerTypes
{
    RifleTower, MachinegunTower, 
    FlameTower, MissileTower, 
    RailgunTower, GravityTower, 
    GrenadeTower, ElectricTower, 
    LaserTower
}

public struct MonsterData
{
    public int Key;
    public string Name;
    public DroneSize DroneSize;
    public int HP;
    public float MoveSpeed;
    public int MonsterReward;
    public string PrefabPath;
}

public enum DroneSize
{
    Small, Medium, Large
}

public struct RoundData
{
    public int Stage;
    public int Wave;
    public List<MonsterSpawnInfo> Monsters;
    public float SpeedMultiplier;
    public float HpMultiplier;
    public int Reward;
}

public enum DroneTypes
{
    Sentinel = 1, Scout, Hunter, Warden, Vanguard, CargoShip, Reaper, Juggernaut, Dreadnought
}

public class MonsterSpawnInfo
{
    public DroneTypes Type;
    public int Count;
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

    private Dictionary<TowerTypes, TurretData> _turretDatas = new Dictionary<TowerTypes, TurretData>();
    public Dictionary<TowerTypes, TurretData> TurretDatas
    {
        get { return _turretDatas; }
    }

    public TurretData GetTurretData(TowerTypes name) { return _turretDatas[name]; }

    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();
    public Dictionary<int, MonsterData> MonsterDatas
    {
        get { return _monsterDatas; }
    }

    public MonsterData GetMonsterData(int key) { return _monsterDatas[key]; }

    private Dictionary<(int, int), RoundData> _roundDatas = new Dictionary<(int, int), RoundData>();
    public Dictionary<(int, int), RoundData> RoundDatas
    {
        get { return _roundDatas; }
    }

    public RoundData GetRoundData((int, int) key) { return _roundDatas[key]; }

    public void LoadTurretData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/TurretTable");

        string text = textAsset.text;

        string[] rowData = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] datas = rowData[i].Split(",");

            TurretData data;
            data.Name = Enum.Parse<TowerTypes>(datas[0]);
            data.GachaPath = datas[0];
            data.Type = Enum.Parse<AttackType> (datas[1]);
            data.Rarity = Enum.Parse<TowerRarity>(datas[2]);
            data.Bullet = datas[3];
            data.BulletSpeed = float.Parse(datas[4]);
            data.Atk = int.Parse(datas[5]);
            data.AtkSpeed = float.Parse(datas[6]);
            data.Range = int.Parse(datas[7]);
            data.Price = int.Parse(datas[8]);
            data.Upgrade = int.Parse(datas[9]);
            data.FireEffectPath = datas[10];
            data.HitEffectPath = datas[11];
            data.CommonSummonTicket = int.Parse(datas[12]);
            data.RareSummonTicket = int.Parse(datas[13]);

            _turretDatas.Add(data.Name, data);
        }
    }

    public void LoadMonsterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/MonsterTable");

        string text = textAsset.text;

        string[] rowData = text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            if (rowData[i].Length == 0)
                break;

            string[] datas = rowData[i].Split(",");

            MonsterData data;
            data.Key = int.Parse(datas[0]);
            data.Name = datas[1];
            data.DroneSize = Enum.Parse<DroneSize>(datas[2]);
            data.HP = int.Parse(datas[3]);
            data.MoveSpeed = float.Parse(datas[4]);
            data.MonsterReward = int.Parse(datas[5]);
            data.PrefabPath = datas[6];
   
            _monsterDatas.Add(data.Key, data);
        }
    }

    public void LoadRoundData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/RoundTable");

        string text = textAsset.text;
        string[] rowData = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] datas = Regex.Split(rowData[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            int stage = int.Parse(datas[0]);
            int wave = int.Parse(datas[1]);
            string rawDroneTypes = datas[2].Trim('"');
            string[] droneTypes = rawDroneTypes.Split(',');

            string rawCounts = datas[3].Trim('"');
            string[] counts = rawCounts.Split(',');

            float speedMultiplier = float.Parse(datas[4]);
            float hpMultiplier = float.Parse(datas[5]);
            int reward = int.Parse(datas[6]);

            RoundData waveData = new RoundData
            {
                Stage = stage,
                Wave = wave,
                SpeedMultiplier = speedMultiplier,
                HpMultiplier = hpMultiplier,
                Reward = reward,
                Monsters = new List<MonsterSpawnInfo>()
            };

            for (int j = 0; j < droneTypes.Length; j++)
            {
                DroneTypes type = Enum.Parse<DroneTypes>(droneTypes[j].Trim());
                int count = int.Parse(counts[j].Trim());

                waveData.Monsters.Add(new MonsterSpawnInfo
                {
                    Type = type,
                    Count = count
                });
            }

            _roundDatas[(stage, wave)] = waveData;
        }
    }
}

