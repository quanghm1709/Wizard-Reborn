using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private enum GeneratedRoomType { Start, Normal, Elite, Healing, Treasure, Shop, End, Boss }

    [Header("Layout")]
    [SerializeField, Min(2)] private int distanceToEnd = 4;
    [SerializeField, Min(0)] private int minBranches = 1;
    [SerializeField, Min(0)] private int maxBranches = 3;
    [SerializeField, Min(1)] private int minBranchLength = 1;
    [SerializeField, Min(1)] private int maxBranchLength = 3;
    [SerializeField] private Transform generatorPoint;
    [SerializeField] private Transform gridParent;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private float xOffset = 12f;
    [SerializeField] private float yOffset = 9f;

    [Header("Room Prefabs")]
    [SerializeField] private GameObject instatiateRoom;
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject endRoom;

    [Header("Runtime")]
    [SerializeField] private List<GameObject> listRoom;
    [SerializeField] private ObjectPool trapPool;
    [SerializeField, Range(0f, 1f)] private float trapRoomChance = .6f;

    private readonly Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private Vector3 startRoomPos;
    private int specialRoom = 1;
    private int currentRoomId = 1;
    private bool floorGenerated;

    public int SpecialRoom => specialRoom;

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void Start()
    {
        startRoomPos = generatorPoint != null ? generatorPoint.position : Vector3.zero;
        StartCoroutine(GenerateWhenReady());
    }

    private IEnumerator GenerateWhenReady()
    {
        yield return new WaitUntil(() => FloorManager.readyGenerate);
        if (!floorGenerated)
        {
            CreateFloor();
        }
    }

    private void HandlePlayerEnterGate(object param)
    {
        StartCoroutine(ResetFloorNextFrame());
    }

    private IEnumerator ResetFloorNextFrame()
    {
        yield return null;
        ResetFloor();
    }

    internal void Load()
    {
        specialRoom = Mathf.Clamp(SaveData.LoadSingleData("specialRoom"), 1, 6);
    }

    public void ApplySaveData(int savedSpecialRoom)
    {
        specialRoom = Mathf.Clamp(savedSpecialRoom, 1, 6);
    }

    private void ResetFloor()
    {
        EnemyGenerator.instance?.ResetFloorCombat();

        if (trapPool != null)
        {
            foreach (GameObject trap in trapPool.pooledGobjects)
            {
                if (trap != null)
                {
                    trap.SetActive(false);
                }
            }
        }

        foreach (GameObject room in listRoom)
        {
            if (room != null)
            {
                room.SetActive(false);
                Destroy(room);
            }
        }
        listRoom.Clear();
        floorGenerated = false;
        CreateFloor();
    }

    public void CreateFloor()
    {
        if (floorGenerated)
        {
            return;
        }

        floorGenerated = true;
        currentRoomId = 1;
        specialRoom = (Mathf.Max(1, FloorManager.currentFloor) - 1) % 6 + 1;

        int seed = unchecked(Environment.TickCount * 397 ^ FloorManager.currentFloor * 7919);
        System.Random random = new System.Random(seed);
        Dictionary<Vector2Int, GeneratedRoomType> layout = GenerateLayout(random);

        foreach (KeyValuePair<Vector2Int, GeneratedRoomType> roomData in layout)
        {
            InstantiateRoom(roomData.Key, roomData.Value, random);
        }

        Physics2D.SyncTransforms();
    }

    private Dictionary<Vector2Int, GeneratedRoomType> GenerateLayout(System.Random random)
    {
        int mainPathRoomCount = Mathf.Max(3, distanceToEnd + 2);
        List<Vector2Int> mainPath = BuildSelfAvoidingPath(mainPathRoomCount, random);
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>(mainPath);
        List<Vector2Int> branchRooms = new List<Vector2Int>();

        int lowerBranchCount = Mathf.Max(0, minBranches);
        int upperBranchCount = Mathf.Max(lowerBranchCount, maxBranches);
        int branchCount = random.Next(lowerBranchCount, upperBranchCount + 1);

        for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
        {
            Vector2Int cursor = mainPath[random.Next(1, mainPath.Count - 1)];
            int lowerLength = Mathf.Max(1, minBranchLength);
            int upperLength = Mathf.Max(lowerLength, maxBranchLength);
            int targetLength = random.Next(lowerLength, upperLength + 1);

            for (int step = 0; step < targetLength; step++)
            {
                List<Vector2Int> candidates = GetFreeNeighbours(cursor, occupied);
                if (candidates.Count == 0)
                {
                    break;
                }

                cursor = candidates[random.Next(candidates.Count)];
                occupied.Add(cursor);
                branchRooms.Add(cursor);
            }
        }

        Dictionary<Vector2Int, GeneratedRoomType> result = new Dictionary<Vector2Int, GeneratedRoomType>();
        foreach (Vector2Int position in occupied)
        {
            result[position] = GeneratedRoomType.Normal;
        }

        result[mainPath[0]] = GeneratedRoomType.Start;
        result[mainPath[mainPath.Count - 1]] = specialRoom == 6 ? GeneratedRoomType.Boss : GeneratedRoomType.End;

        if (specialRoom % 3 == 0)
        {
            List<Vector2Int> shopCandidates = branchRooms.Count > 0
                ? branchRooms
                : mainPath.GetRange(1, mainPath.Count - 2);
            if (shopCandidates.Count > 0)
            {
                Vector2Int shopPosition = shopCandidates[random.Next(shopCandidates.Count)];
                if (result[shopPosition] == GeneratedRoomType.Normal)
                {
                    result[shopPosition] = GeneratedRoomType.Shop;
                }
            }
        }


        List<Vector2Int> specialCandidates = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, GeneratedRoomType> room in result)
        {
            if (room.Value == GeneratedRoomType.Normal)
            {
                specialCandidates.Add(room.Key);
            }
        }
        Shuffle(specialCandidates, random);
        if (specialCandidates.Count > 0) result[specialCandidates[0]] = GeneratedRoomType.Elite;
        if (specialCandidates.Count > 1) result[specialCandidates[1]] = GeneratedRoomType.Healing;
        if (specialCandidates.Count > 2) result[specialCandidates[2]] = GeneratedRoomType.Treasure;

        return result;
    }

    private List<Vector2Int> BuildSelfAvoidingPath(int roomCount, System.Random random)
    {
        List<Vector2Int> path = new List<Vector2Int> { Vector2Int.zero };
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int> { Vector2Int.zero };
        int attempts = 0;

        while (path.Count < roomCount && attempts < roomCount * 100)
        {
            attempts++;
            Vector2Int cursor = path[path.Count - 1];
            List<Vector2Int> candidates = GetFreeNeighbours(cursor, occupied);
            if (candidates.Count > 0)
            {
                Vector2Int next = candidates[random.Next(candidates.Count)];
                path.Add(next);
                occupied.Add(next);
            }
            else if (path.Count > 1)
            {
                occupied.Remove(path[path.Count - 1]);
                path.RemoveAt(path.Count - 1);
            }
        }

        if (path.Count < roomCount)
        {
            throw new InvalidOperationException("Unable to generate a connected main dungeon path.");
        }
        return path;
    }

    private List<Vector2Int> GetFreeNeighbours(Vector2Int position, HashSet<Vector2Int> occupied)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int candidate = position + direction;
            if (!occupied.Contains(candidate))
            {
                result.Add(candidate);
            }
        }
        return result;
    }

    private void InstantiateRoom(Vector2Int gridPosition, GeneratedRoomType roomType, System.Random random)
    {
        GameObject prefab = GetPrefab(roomType);
        if (prefab == null)
        {
            Debug.LogError($"Missing prefab for generated room type {roomType}.");
            return;
        }

        Vector3 worldPosition = startRoomPos + new Vector3(gridPosition.x * xOffset, gridPosition.y * yOffset, 0f);
        GameObject room = Instantiate(prefab, worldPosition, Quaternion.identity, gridParent);
        room.name = $"{roomType} Room [{gridPosition.x},{gridPosition.y}]";

        RoomController controller = room.GetComponent<RoomController>();
        if (controller != null)
        {
            controller.roomId = currentRoomId++;
            controller.Configure(ToRoomCategory(roomType), GetObjective(roomType, random));
        }
        listRoom.Add(room);

        if ((roomType == GeneratedRoomType.Normal || roomType == GeneratedRoomType.Elite) && trapPool != null && TrapManager.trapGridName.Count > 0 && random.NextDouble() < trapRoomChance)
        {
            int trapIndex = random.Next(0, TrapManager.trapGridName.Count);
            GameObject trap = trapPool.GetObject(TrapManager.trapGridName[trapIndex]);
            if (trap != null)
            {
                trap.transform.position = worldPosition;
            }
        }
    }

    private GameObject GetPrefab(GeneratedRoomType roomType)
    {
        switch (roomType)
        {
            case GeneratedRoomType.Start: return startRoom;
            case GeneratedRoomType.Shop: return shopRoom;
            case GeneratedRoomType.End: return endRoom;
            case GeneratedRoomType.Boss: return bossRoom;
            default: return instatiateRoom;
        }
    }

    private static RoomCategory ToRoomCategory(GeneratedRoomType roomType)
    {
        switch (roomType)
        {
            case GeneratedRoomType.Start: return RoomCategory.Start;
            case GeneratedRoomType.Elite: return RoomCategory.Elite;
            case GeneratedRoomType.Healing: return RoomCategory.Healing;
            case GeneratedRoomType.Treasure: return RoomCategory.Treasure;
            case GeneratedRoomType.Shop: return RoomCategory.Shop;
            case GeneratedRoomType.End: return RoomCategory.Exit;
            case GeneratedRoomType.Boss: return RoomCategory.Boss;
            default: return RoomCategory.Combat;
        }
    }

    private static EncounterObjective GetObjective(GeneratedRoomType roomType, System.Random random)
    {
        if (roomType == GeneratedRoomType.Elite || roomType == GeneratedRoomType.Boss)
        {
            return EncounterObjective.Elimination;
        }
        if (roomType != GeneratedRoomType.Normal)
        {
            return EncounterObjective.None;
        }
        return (EncounterObjective)random.Next((int)EncounterObjective.Elimination, (int)EncounterObjective.Ritual + 1);
    }

    private static void Shuffle<T>(IList<T> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }
}
