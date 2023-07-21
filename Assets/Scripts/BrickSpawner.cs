using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public int width, height;
    public List<GameObject> brickList;
    public Vector3 spawnPoint;
    public int[,] floorGrid;

    private GameObject brick;

    void Start()
    {
        floorGrid = new int[width, height];
        SpawnBricks();
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        while (true)
        {
            SpawnBricks();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SpawnBricks()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (floorGrid[i, j] < 0)
                {
                    floorGrid[i, j] += 1;
                }
                else if (floorGrid[i, j] == 0)
                {
                    int queue = Random.Range(0, brickList.Count);
                    floorGrid[i, j] = queue + 1;
                    spawnPoint = new Vector3(transform.position.x + i, 0.4f, transform.position.z + j);
                    brick = Instantiate(brickList[queue], spawnPoint, transform.rotation);
                    brick.GetComponent<BrickCoordinates>().brickX = i;
                    brick.GetComponent<BrickCoordinates>().brickY = j;
                }
            }
        }
    }
}
