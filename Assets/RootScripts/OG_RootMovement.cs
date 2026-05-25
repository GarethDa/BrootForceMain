using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OG_RootMovement : MonoBehaviour
{
    float horizInput;
    float vertInput;
    //[SerializeField] Camera mainCam;
    [SerializeField] float poopThreshold = 1f;
    [SerializeField] GameObject poop;
    [SerializeField] LineRenderer lRendererFront;
    [SerializeField] LineRenderer lRendererBack;

    [SerializeField] float baseVelocity = 3f;
    [SerializeField] float maxVelocity = 6f;
    [SerializeField] float speedUpTime = 3f;

    [SerializeField] GameObject rootParent;

    float targetAngle = 0;

    float poopTime = 0f;

    Vector3 oldPosition;

    float currentSpeedTime = 0f;

    int nodeParentNum = 0;
    GameObject currentNodeParent;

    List<GameObject> nodes = new List<GameObject>();

    float collisionGracePeriod = 2f;
    bool isInvincible = true;

    float tutorialTimer = 8f;
    bool faded = false;

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = gameObject.transform.position;
        lRendererFront.SetPosition(0, gameObject.transform.position);

        currentNodeParent = new GameObject();
        currentNodeParent.name = "Nodes0";
        currentNodeParent.transform.parent = rootParent.transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (nodes.Count == 0)
            lRendererFront.SetPosition(1, gameObject.transform.position);

        else
            lRendererFront.SetPosition(lRendererFront.positionCount - 1, gameObject.transform.position);

        poopTime += Time.deltaTime;

        if (poopTime >= poopThreshold * (Mathf.Pow(baseVelocity / gameObject.GetComponent<Rigidbody2D>().linearVelocity.magnitude, 0.7f)))
        {
            nodes.Add(Instantiate(poop, oldPosition, Quaternion.identity));
            nodes[nodes.Count - 1].transform.parent = rootParent.transform;
            nodes[nodes.Count - 1].transform.parent = currentNodeParent.transform;

            poopTime = 0f;

            oldPosition = gameObject.transform.position;

            DrawNodeConnection();

            if (nodes.Count % 50 == 0)
            {
                nodeParentNum++;

                GameObject nodesParent = new GameObject();

                nodesParent.name = "Nodes" + nodeParentNum;
                nodesParent.transform.parent = rootParent.transform;

                currentNodeParent = nodesParent;

                if (nodeParentNum >= 2)
                {
                    GameObject.Find("Nodes" + (nodeParentNum - 2)).SetActive(false);
                }
            }
        }

        /*
        if (hitScreen != null)
        {
            if (hitScreen.GetComponent<Image>().color.a > 0)
            {
                var color = hitScreen.GetComponent<Image>().color;

                color.a -= 0.01f;

                hitScreen.GetComponent<Image>().color = color;
            }
        }
        */
    }

    void FixedUpdate()
    {
        HandleInput();
        MoveRoot();

        collisionGracePeriod -= Time.fixedDeltaTime;
        if (isInvincible && collisionGracePeriod <= 0)
        {
            isInvincible = false;
        }
    }

    void HandleInput()
    {
        {
            horizInput = Input.GetAxis("Horizontal");
            vertInput = Input.GetAxis("Vertical");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Bye Bye");
            Application.Quit();
        }
    }

    void MoveRoot()
    {
        targetAngle += horizInput * -3;

        gameObject.GetComponent<Rigidbody2D>().SetRotation(targetAngle);

        //gameObject.GetComponent<Rigidbody2D>().velocity = -3 * gameObject.transform.up;

        if (vertInput >= 1)
        {
            if (currentSpeedTime < speedUpTime)
                currentSpeedTime += Time.fixedDeltaTime;

            else
                currentSpeedTime = speedUpTime;
        }

        else if (currentSpeedTime > 0f)
        {
            currentSpeedTime -= 2 * Time.fixedDeltaTime;

            if (currentSpeedTime < 0)
                currentSpeedTime = 0f;
        }

        gameObject.GetComponent<Rigidbody2D>().linearVelocity = -Mathf.Lerp(baseVelocity, maxVelocity, currentSpeedTime / speedUpTime) * gameObject.transform.up;

        //mainCam.gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, mainCam.gameObject.transform.position.z);
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
    }

    void DrawNodeConnection()
    {
        if (nodes.Count < 12)
            lRendererFront.positionCount = nodes.Count + 1;

        else if (nodes.Count == 12)
        {
            lRendererBack.positionCount = 3;
            lRendererBack.SetPosition(0, lRendererFront.GetPosition(0));
            lRendererBack.SetPosition(1, lRendererFront.GetPosition(1));
            lRendererBack.SetPosition(2, lRendererFront.GetPosition(2));

            for (int i = 0; i < 11; i++)
            {
                lRendererFront.SetPosition(i, lRendererFront.GetPosition(i + 1));
            }
        }

        else
        {
            lRendererBack.positionCount = nodes.Count - 9;
            lRendererBack.SetPosition(nodes.Count - 12, lRendererFront.GetPosition(0));
            lRendererBack.SetPosition(nodes.Count - 11, lRendererFront.GetPosition(1));
            lRendererBack.SetPosition(nodes.Count - 10, lRendererFront.GetPosition(2));

            for (int i = 0; i < 11; i++)
            {
                lRendererFront.SetPosition(i, lRendererFront.GetPosition(i + 1));
            }
        }

        lRendererFront.SetPosition(lRendererFront.positionCount - 2, nodes[nodes.Count - 1].transform.position);

        lRendererFront.SetPosition(lRendererFront.positionCount - 1, gameObject.transform.position);

        if (nodes.Count >= 2)
        {
            nodes[nodes.Count - 2].AddComponent<CircleCollider2D>();
        }
    }

    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Wall")) && !isInvincible)
        {
            if (CameraScript.instance.endGame == false)
            {
                hitDetected();
            }

            WaterUI.instance.WaterSubtraction(100);

            isInvincible = true;
            collisionGracePeriod = 2f;
        }

        if (collision.gameObject.CompareTag("Fountain"))
        {
            ScoreManager.instance.AddScore(100);
            CameraScript.instance.endGame = true;
        }
    }
    */

    /*
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Staying");
        if (collision.gameObject.CompareTag("Wall") && !isInvincible)
        {
            WaterUI.instance.WaterSubtraction(100);

            isInvincible = true;
            collisionGracePeriod = 2f;
        }
    }

    void hitDetected()
    {
        var color = hitScreen.GetComponent<Image>().color;
        color.a = 0.8f;

        hitScreen.GetComponent<Image>().color = color;
    }
    */
}