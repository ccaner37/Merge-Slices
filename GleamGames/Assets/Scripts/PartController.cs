using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartController : MonoBehaviour
{
    public Transform findWheelPart;
    public Transform flyingPart;

    bool shouldSend = false;
    GameObject selectedWheelPart;
    GameObject myRightPart;
    GameObject myLeftPart;
    Transform createdPart;

    int flyingpoint;
    int selectedPartPoint;
    int selectedPartNumber;
    public Text tempScore;

    private float startTime;
    public float speed = 14.5F;
    private float journeyLength;
    private float distCovered;

    bool isNewPartCoroutineStarted = false;
    bool isNearbyCoroutineStarted = false;

    private void Awake()
    {
        shouldSend = false;
    }

    void Start()
    {
        NewPart();
        InvokeRepeating("CheckNearbyParts", 1, 2);
    }

    void NewPart()
    {
        createdPart = Instantiate(flyingPart, flyingPart.transform.position, flyingPart.transform.rotation);
        tempScore = createdPart.transform.Find("Canvas/Text").GetComponent<Text>();
        if (GameManager.Instance.score >= 16 && Random.Range(1, 2) == 2)
        {
            tempScore.text = "4";
            flyingpoint = 4;
        }
        else
        {
            tempScore.text = "2";
            flyingpoint = 2;
        }

        startTime = Time.time;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SendPart();
        }
    }

    void SendPart()
    {
        RaycastHit hit; // know where you are going to go
        if (Physics.Raycast(findWheelPart.position, new Vector3(0, 0, 0.3f), out hit, 100, 13))
        {
            selectedWheelPart = hit.collider.gameObject;

            Point selectedPart = hit.collider.GetComponent<Point>();

            selectedPartPoint = selectedPart.PartPoint;
            selectedPartNumber = selectedPart.Number;
            selectedPart.myPartPrefab = createdPart.gameObject;
            
            Debug.Log(selectedPartPoint);

            if (selectedPartPoint == 0)
            {
                selectedPart.SetPoint(flyingpoint);
            }
            else if (flyingpoint == selectedPart.PartPoint)
            {
                GameManager.Instance.score = selectedPartPoint * 2;
                tempScore.text = GameManager.Instance.score.ToString();
                Destroy(selectedPart.myPartPrefab.gameObject, 2f);
            }
            else if (flyingpoint != selectedPart.Number)
            {
                return;
            }

            shouldSend = true;
            distCovered = (Time.time - startTime) * speed;
            journeyLength = Vector3.Distance(createdPart.position, hit.collider.gameObject.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (shouldSend)  // go to the wheel
        {
            
            float fractionOfJourney = distCovered / journeyLength;
            createdPart.position = Vector3.Lerp(createdPart.position, selectedWheelPart.transform.position, fractionOfJourney);
            createdPart.rotation = Quaternion.Lerp(createdPart.rotation, selectedWheelPart.transform.rotation, fractionOfJourney);
            createdPart.transform.position = new Vector3(createdPart.transform.position.x, createdPart.transform.position.y, createdPart.transform.position.z - 0.01f);
            createdPart.parent = selectedWheelPart.transform;

          //  Create new part and check the sended part 
            if (!isNearbyCoroutineStarted)
            {
                StartCoroutine(CheckNearbyParts());
            }
            if (!isNewPartCoroutineStarted)
            {
                StartCoroutine(CreateNewPart());
            }
        }
    }

    IEnumerator CreateNewPart()
    {
        isNewPartCoroutineStarted = true;
        yield return new WaitForSeconds(1.5f);
        shouldSend = false;
        NewPart();
        isNewPartCoroutineStarted = false;

    }

    IEnumerator CheckNearbyParts()
    {
        isNearbyCoroutineStarted = true;
        yield return new WaitForSeconds(1.5f);

        //Check Right
        int eksi1 = selectedPartNumber - 1;
        if (selectedPartNumber == 0) 
        {
            eksi1 = 0; 
        }
        var rightOne = GameObject.Find(eksi1.ToString());
        Point p = rightOne.GetComponent<Point>();
        int pointfinal = p.PartPoint;
        GameObject rightPrefab = p.myPartPrefab;

        if (pointfinal == flyingpoint)
        {
            createdPart.transform.position = rightOne.transform.position;
            createdPart.transform.parent = rightOne.transform.parent;
            Destroy(rightPrefab);
            GameManager.Instance.score = pointfinal * 2;
            tempScore.text = GameManager.Instance.score.ToString();
        }

        //Check Left
        int plusone = selectedPartNumber + 1;
        if (plusone == 10)
        {
            plusone = 0;
        }
        var leftOne = GameObject.Find(plusone.ToString());
        Point pLeft = leftOne.GetComponent<Point>();
        int pointfinalLeft = pLeft.PartPoint;
        GameObject leftPrefab = pLeft.myPartPrefab;

        if (pointfinalLeft == flyingpoint)
        {
            createdPart.transform.position = leftOne.transform.position;
            createdPart.transform.parent = leftOne.transform.parent;
            Destroy(leftPrefab);
            GameManager.Instance.score = pointfinalLeft * 2;
            tempScore.text = GameManager.Instance.score.ToString();
        }



        isNearbyCoroutineStarted = false;
    }
}

