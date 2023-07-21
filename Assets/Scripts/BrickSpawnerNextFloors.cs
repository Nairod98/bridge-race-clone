using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomDict
{
    public int Key;
    public int Value;

    public CustomDict(int key, int value)
    {
        Key = key;
        Value = value;
    }
}

public class BrickSpawnerNextFloors : MonoBehaviour
{
    public int width, height;
    public List<GameObject> brickList;
    public List<int> numOfBricks;
    public List<CustomDict> waitingBricks;
    public Vector3 spawnPoint;
    public int[,] floorGrid;
    public int maxBricks;

    private GameObject brick;

    void Start()
    {
        floorGrid = new int[width, height];
        numOfBricks = new List<int>();
        waitingBricks = new List<CustomDict>();
        brickList = new List<GameObject>();
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        while (true)
        {
            for (int i = 0; i < brickList.Count; i++)
            {
                if (numOfBricks[i] < maxBricks - waitingBricks.Where(x => x.Key == i).Count())
                {
                    SpawnBricks(i);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (floorGrid[i, j] < 0)
                    {
                        floorGrid[i, j] += 1;
                    }
                }
            }

            List<CustomDict> list = new();

            foreach (var bricks in waitingBricks)
            {
                if (bricks.Value < 0)
                {
                    bricks.Value++;
                } 
                else
                {
                    list.Add(bricks);
                }
            }

            foreach (var bricks in list)
            {
                waitingBricks.Remove(bricks);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SpawnBricks(int colorID)
    {
        while (numOfBricks[colorID] < maxBricks - waitingBricks.Where(x => x.Key == colorID).Count())
        {
            int randX = Random.Range(0, width);
            int randY = Random.Range(0, height);

            if (floorGrid[randX, randY] == 0) 
            {
                floorGrid[randX, randY] = colorID + 1;

                if (numOfBricks[colorID] + 1 <= maxBricks)
                {
                    numOfBricks[colorID]++;
                }

                spawnPoint = new Vector3(transform.position.x + randX, 0.4f, transform.position.z + randY);
                brick = Instantiate(brickList[colorID], spawnPoint, transform.rotation);
                brick.GetComponent<BrickCoordinates>().brickX = randX;
                brick.GetComponent<BrickCoordinates>().brickY = randY;
            }
        }
    }
}
