using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    LayerMask layer;

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
    public string playerColor = "Red"; 
    public GameObject playerColorGameObject;

    public int actualFloor = 1;
    public GameObject objectToScatter;
    public int brickListPosition;

    void Start()
    {
        layer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            playerAnimator.SetBool("Run", true);

            if (Physics.Raycast(ray, out RaycastHit hit, 4000f, layer))
            {
                Vector3 newPos = new(hit.point.x, 0.2f, hit.point.z);
                transform.SetPositionAndRotation(Vector3.Lerp(transform.position, newPos, speed * Time.deltaTime),
                    Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newPos - transform.position),
                    rotSpeed * Time.deltaTime));
            }
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            playerAnimator.SetBool("Run", false);
        }
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

                gameObject.waitingBricks.Add(new CustomDict(brickListPosition, -5));
            }

            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("AI"))
        {
            if (transform.GetChild(2).childCount < other.transform.GetChild(2).childCount)
            {
                ScatterObjects();

                foreach (Transform child in transform.GetChild(2))
                {
                    Destroy(child.gameObject);

                    if (actualFloor > 1)
                    {
                        GameObject.Find("GameController" + actualFloor).GetComponent<BrickSpawnerNextFloors>().waitingBricks
                            .Add(new CustomDict(brickListPosition, -5));
                    }
                }

                if (other.gameObject.CompareTag("AI"))
                {
                    StartCoroutine(other.GetComponent<AIBehaviour>().FindBrick());
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bridge"))
        {
            ColorUtility.TryParseHtmlString(playerColor, out Color myColor);

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
