using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Portal : MonoBehaviour
{
    public int portalFloor;
    public GameObject bridgeToDisable;
    public GameObject collider;

    private void Start()
    {
        GenerateNavMesh();
    }

    void GenerateNavMesh()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

        NavMeshSurface surface = floors.First().AddComponent<NavMeshSurface>();
        surface.layerMask = LayerMask.GetMask("Ground");

        surface.BuildNavMesh();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerMovement playerGameObj = other.GetComponent<PlayerMovement>();

                if (portalFloor > playerGameObj.actualFloor)
                {
                    playerGameObj.actualFloor++;
                    bridgeToDisable.SetActive(false);
                    collider.SetActive(true);

                    foreach (var obj in GameObject.FindGameObjectsWithTag("AI"))
                    {
                        StartCoroutine(obj.GetComponent<AIBehaviour>().FindBrick());
                    }
                }

                BrickSpawnerNextFloors gameControllerPlayer = GameObject.Find("GameController" + playerGameObj.actualFloor)
                    .GetComponent<BrickSpawnerNextFloors>();

                for (int i = playerGameObj.actualFloor - 1; i > 0; i--)
                {
                    if ("GameController" + i == "GameController1")
                    {
                        GameObject.Find("GameController1").GetComponent<BrickSpawner>().brickList.RemoveAll(x => x.name == playerGameObj.playerColor);
                    }
                    else
                    {
                        GameObject.Find("GameController" + i).GetComponent<BrickSpawnerNextFloors>().brickList.RemoveAll(x => x.name == playerGameObj.playerColor);
                    }
                }

                foreach (var obj in GameObject.FindGameObjectsWithTag(playerGameObj.playerColor).Where(x => x.transform.parent != playerGameObj.backpack.transform).ToList())
                {
                    Destroy(obj);
                }

                if (!gameControllerPlayer.brickList.Contains(playerGameObj.playerColorGameObject))
                {
                    playerGameObj.brickListPosition = gameControllerPlayer.brickList.Count;
                    gameControllerPlayer.brickList.Add(playerGameObj.playerColorGameObject);
                    gameControllerPlayer.numOfBricks.Add(0);
                }

                break;

            case "AI":
                AIBehaviour aiGameObj = other.GetComponent<AIBehaviour>();

                if (portalFloor > aiGameObj.actualFloor)
                {
                    aiGameObj.actualFloor++;
                    bridgeToDisable.SetActive(false);
                    collider.SetActive(true);

                    foreach (var obj in GameObject.FindGameObjectsWithTag("AI"))
                    {
                        StartCoroutine(obj.GetComponent<AIBehaviour>().FindBrick());
                    }
                }

                BrickSpawnerNextFloors gameControllerAI = GameObject.Find("GameController" + aiGameObj.actualFloor).GetComponent<BrickSpawnerNextFloors>();

                for (int i = aiGameObj.actualFloor - 1; i > 0; i--)
                {
                    if ("GameController" + i == "GameController1")
                    {
                        GameObject.Find("GameController1").GetComponent<BrickSpawner>().brickList.RemoveAll(x=>x.name == aiGameObj.playerColor);
                    } 
                    else
                    {
                        GameObject.Find("GameController" + i).GetComponent<BrickSpawnerNextFloors>().brickList.RemoveAll(x => x.name == aiGameObj.playerColor);
                    }
                }

                foreach (var obj in GameObject.FindGameObjectsWithTag(aiGameObj.playerColor).Where(x => x.transform.parent != aiGameObj.backpack.transform).ToList())
                {
                    Destroy(obj);
                }

                if (!gameControllerAI.brickList.Contains(aiGameObj.playerColorGameObject))
                {
                    aiGameObj.brickListPosition = gameControllerAI.brickList.Count;
                    gameControllerAI.brickList.Add(aiGameObj.playerColorGameObject);
                    gameControllerAI.numOfBricks.Add(0);
                }

                StartCoroutine(aiGameObj.FindBrick());

                break;

            default: 
                break;
        }

        NavMesh.RemoveAllNavMeshData();
        GenerateNavMesh();
    }
}
