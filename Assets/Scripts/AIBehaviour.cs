using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour
{
    [Header("Player Speed")]
    public float speed;

    [Header("Player Rotation Speed")]
    public float rotSpeed;

    [Header("Player Animator")]
    public Animator playerAnimator;

    [Header("Player Backpack")]
    public GameObject backpack;
    public float backpackHeight = 0;

    [Header("Player Color")]
    public string playerColor;
    public GameObject playerColorGameObject;

    public int actualFloor = 1;
    public Vector3 destination;
    public NavMeshAgent agent;
    public GameObject objectToScatter;
    List<GameObject> portals;
    public int brickListPosition;

    private void Start()
    {
        playerAnimator.SetBool("Run", true);
        StartCoroutine(FindBrick());
    }

    public IEnumerator FindBrick()
    {
        yield return new WaitForSeconds(0.2f);
        List<GameObject> myBricks = GameObject.FindGameObjectsWithTag(playerColor).Where(x => x.transform.parent != backpack.transform).ToList();

        float distanceToNearest = float.MaxValue;
        float distanceToCalc;
        GameObject nearestBrick = null;

        foreach (GameObject brick in myBricks) 
        {
            distanceToCalc = Vector3.Distance(transform.position, brick.transform.position);

            if (distanceToCalc < distanceToNearest)
            {
                distanceToNearest = distanceToCalc;
                nearestBrick = brick;
            }
        }

        if (nearestBrick != null)
        {
            try
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(nearestBrick.transform.position);
                }
            }
            catch
            {
                gameObject.transform.position = GameObject.Find("Floor" + actualFloor).transform.position;
            }
        }
        else
        { 
            if (backpack.transform.childCount > 0)
            {
                GoToBridge();
            }
        }
    }

    private void GoToBridge()
    {
        portals = GameObject.FindGameObjectsWithTag("PortalEnd").Where(x => x.GetComponent<Portal>().portalFloor == actualFloor + 1).ToList();

        agent.SetDestination(portals[Random.Range(0, portals.Count)].transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerColor))
        {
            AddObjectToPoint(other.gameObject);

            if (actualFloor == 1)
            {
                GameObject.Find("GameController1").GetComponent<BrickSpawner>().
                    floorGrid[other.gameObject.GetComponent<BrickCoordinates>().brickX,
                    other.gameObject.GetComponent<BrickCoordinates>().brickY] = -5;
            }
            else
            {
                BrickSpawnerNextFloors gameObject = GameObject.Find("GameController" + actualFloor).GetComponent<BrickSpawnerNextFloors>();

                gameObject.floorGrid[other.gameObject.GetComponent<BrickCoordinates>().brickX,
                    other.gameObject.GetComponent<BrickCoordinates>().brickY] = -5;

                if (gameObject.waitingBricks.Where(x => x.Key == brickListPosition).Count() < gameObject.maxBricks)
                    gameObject.waitingBricks.Add(new CustomDict(brickListPosition, -5));
            }

            Destroy(other.gameObject);

            if (backpack.transform.childCount < 8)
            {
                StartCoroutine(FindBrick());
            } 
            else
            {
                GoToBridge();
            }
        } 
        else if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI")) 
        {
            if (transform.GetChild(2).childCount < other.transform.GetChild(2).childCount)
            {
                ScatterObjects();

                foreach (Transform child in transform.GetChild(2))
                {
                    Destroy(child.gameObject);

                    if (actualFloor > 1)
                    {
                        GameObject.Find("GameController" + actualFloor).GetComponent<BrickSpawnerNextFloors>()
                            .waitingBricks.Add(new CustomDict(brickListPosition, -5));
                    }
                }

                StartCoroutine(FindBrick());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bridge"))
        {
            gameObject.GetComponent<NavMeshAgent>().radius = 0.1f;
            UnityEngine.ColorUtility.TryParseHtmlString(playerColor, out Color myColor);

            if (myColor != other.gameObject.GetComponent<MeshRenderer>().material.color && backpack.transform.childCount > 0)
            {
                other.gameObject.GetComponent<MeshRenderer>().enabled = true;
                other.gameObject.GetComponent<MeshRenderer>().material.color = myColor;

                int lastChildIndex = backpack.transform.childCount - 1;

                if (actualFloor > 1)
                {
                    BrickSpawnerNextFloors brickSpawner = GameObject.Find("GameController" + actualFloor).GetComponent<BrickSpawnerNextFloors>();
                    if (brickSpawner.numOfBricks[brickListPosition] >= 1)
                    {
                        brickSpawner.numOfBricks[brickListPosition]--;
                    }
                }

                Destroy(backpack.transform.GetChild(lastChildIndex).gameObject);

                if (backpack.transform.childCount == 1)
                {
                    StartCoroutine(FindBrick());
                }
            }
        }
    }

    private void AddObjectToPoint(GameObject obj)
    {
        backpackHeight = backpack.transform.childCount / 4f;
        GameObject gameObj = Instantiate(obj, new Vector3(backpack.transform.position.x, backpack.transform.position.y + backpackHeight,
            backpack.transform.position.z), transform.rotation);

        gameObj.transform.SetParent(backpack.transform);
    }

    void ScatterObjects()
    {
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            Vector3 scatterPosition = Random.insideUnitSphere * 0.5f;
            scatterPosition += new Vector3(transform.position.x, 2.0f, transform.position.z);
            GameObject gameObject = Instantiate(objectToScatter, scatterPosition, Quaternion.identity);

            Destroy(gameObject, 3.0f);
        }
    }
}
