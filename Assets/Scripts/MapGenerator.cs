using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject[] cells;

    public int mapWidth;
    public int mapHeight;

    public Transform origin;

    private int[,] field;

    private int GetCellState(int i, int j) {
        if (i == mapWidth)
            return 100;
        if (j == mapHeight)
            return 100;
        return field[i,j] * 1 + //1
        field[i + 1,j] * 2 +  //2
        field[i + 1,j + 1] * 4 + //3
        field[i,j + 1] * 8; //4
    }
    private void Awake()
    {
        var cols = 1 + mapWidth;
        var rows = 1 + mapHeight;

        field = new int[rows, cols];

        for (var i = 0; i < cols; ++i)
        {
            for (var j = 0; j < rows; ++j)
            {
                field[i, j] = (int)Mathf.Floor(Random.value + 0.5f);
            }
        }

        for (var i = 0; i < cols - 1; ++i)
        {
            for (var j = 0; j < rows - 1; ++j)
            {
                int cellIndex = GetCellState(i, j);
                if (cellIndex >= cells.Length) {
                    continue;
                }
                GameObject prefab_instance = PrefabUtility.InstantiatePrefab(cells[GetCellState(i, j)], origin) as GameObject;
                prefab_instance.transform.position = new Vector3(-i*200, 0,-j*200);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
