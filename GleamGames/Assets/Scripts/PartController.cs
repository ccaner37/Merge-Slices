using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartController : MonoBehaviour
{
    Point pointList;

    public Transform findWheelPart;
    public Transform flyingPart;

    bool shouldSend = false;
    GameObject selectedWheelPart;
    GameObject myRightPart;
    GameObject myLeftPart;
    Transform createdPart;
    Transform oldCreatedPart;

    int flyingpoint;
    int selectedPartPoint;
    int selectedPartNumber;
    public Text tempScore;
    public Text oldTempScore;

    private float startTime;
    public float speed = 45.9F;
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
    }

    void NewPart()
    {
        createdPart = Instantiate(flyingPart, flyingPart.transform.position, flyingPart.transform.rotation);
        tempScore = createdPart.transform.Find("Canvas/Text").GetComponent<Text>();
        if (GameManager.Instance.score >= 16 && Random.Range(1, 10) <= 7 )
        {
            tempScore.text = "4";
            flyingpoint = 4;
        }
        else
        {
            tempScore.text = "2";
            flyingpoint = 2;
        }

        GameManager.Instance.GiveColor(flyingpoint, createdPart.gameObject);

        startTime = Time.time;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isNewPartCoroutineStarted == false)
            {
                SendPart();
            }
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
            if (flyingpoint == selectedPart.PartPoint)
            {

            }
            else
            {
                selectedPart.myPartPrefab = createdPart.gameObject;
            }
                

         //   Debug.Log(selectedPartPoint);

            if (selectedPartPoint == 0)
            {
                selectedPart.SetPoint(flyingpoint);

                shouldSend = true;
                oldCreatedPart = createdPart;
                oldTempScore = oldCreatedPart.transform.Find("Canvas/Text").GetComponent<Text>();
            }
            else if (flyingpoint == selectedPart.PartPoint)
            {
                if(selectedPart.PartPoint >= GameManager.Instance.score) { GameManager.Instance.score = selectedPartPoint * 2; }
                
                selectedPart.SetPoint(selectedPartPoint * 2);
                oldCreatedPart = createdPart;
                oldTempScore = oldCreatedPart.transform.Find("Canvas/Text").GetComponent<Text>();
                oldTempScore.text = (selectedPartPoint * 2).ToString();
                Destroy(selectedPart.myPartPrefab, 1.95f);
                GameManager.Instance.GiveColor(selectedPartPoint * 2, oldCreatedPart.gameObject);
                selectedPart.myPartPrefab = oldCreatedPart.gameObject;

                shouldSend = true;
                oldCreatedPart = createdPart;
            }
            else if (flyingpoint != selectedPart.PartPoint)
            {
                shouldSend = false;
            }

            
            distCovered = (Time.time - startTime + 1) * speed;
            journeyLength = Vector3.Distance(createdPart.position, hit.collider.gameObject.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (shouldSend)  // go to the wheel
        {
            
            float fractionOfJourney = distCovered / journeyLength;
            oldCreatedPart.position = Vector3.Lerp(oldCreatedPart.position, selectedWheelPart.transform.position, fractionOfJourney);
            oldCreatedPart.rotation = Quaternion.Lerp(oldCreatedPart.rotation, selectedWheelPart.transform.rotation, fractionOfJourney);
            oldCreatedPart.transform.position = new Vector3(oldCreatedPart.transform.position.x, oldCreatedPart.transform.position.y, oldCreatedPart.transform.position.z - 0.01f);
            oldCreatedPart.parent = selectedWheelPart.transform;

            //  Create new part for sending
            if (!isNearbyCoroutineStarted)
            {
                StartCoroutine(CheckCoroutine());
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
        yield return new WaitForSeconds(1.85f);
        shouldSend = false;
        NewPart();
        isNewPartCoroutineStarted = false;
    }

    IEnumerator CheckCoroutine()
    {
        isNearbyCoroutineStarted = true;
        yield return new WaitForSeconds(0.95f);
        CheckNearbyPartsForAll();
        isNearbyCoroutineStarted = false;
    }

    void CheckNearbyPartsForAll()
    {
        for (int i = 0; i <= 10; i++)
        {
            //Check Right
            int eksi1f = i - 1;
            if (i == 0)
            {
                eksi1f = 10;
            }
            var checkingOne = GameObject.Find(eksi1f.ToString());
            var rightOnef = GameObject.Find(i.ToString());

            Point pRight = rightOnef.GetComponent<Point>();
            int pointfinal = pRight.PartPoint;

            Point pRighti = checkingOne.GetComponent<Point>();
            int pointfinalforrighti = pRighti.PartPoint;

            if (pointfinal == pointfinalforrighti && pointfinal != 0 && pointfinalforrighti != 0)
            {
                pRighti.myPartPrefab.transform.position = pRight.myPartPrefab.transform.position;
                pRight.myPartPrefab.transform.parent = pRighti.myPartPrefab.transform.parent;

                Destroy(pRight.myPartPrefab, 0.1f);
                pRight.myPartPrefab = pRighti.myPartPrefab;
               
                pRight.PartPoint = 0;
                pRighti.PartPoint = pointfinal * 2;

                if (pRighti.PartPoint >= GameManager.Instance.score) { GameManager.Instance.score = pointfinal * 2; }

                //GameManager.Instance.score = pointfinalforrighti * 2;

                pRighti.myPartPrefab.transform.Find("Canvas/Text").GetComponent<Text>().text = (pointfinal * 2).ToString();
                GameManager.Instance.GiveColor(pRighti.PartPoint, pRighti.myPartPrefab.gameObject);
                StartCoroutine(CheckCoroutine());
                return;
            }
        }

        for (int z = 0; z <= 10; z++) 
        { 
            //Check Left
            int plusonef = z + 1;
            if (z == 10)
            {
                plusonef = 0;
            }
            var checkingOneLeft = GameObject.Find(plusonef.ToString());
            var leftOnef = GameObject.Find(z.ToString());

            Point pLeft = leftOnef.GetComponent<Point>();
            int pointfinalLeft = pLeft.PartPoint;

            Point pLefti = checkingOneLeft.GetComponent<Point>();
            int pointfinalfori = pLefti.PartPoint;

            

            if (pointfinalLeft == pointfinalfori && pointfinalLeft != 0 && pointfinalfori != 0)
            {
                pLefti.myPartPrefab.transform.position = pLeft.myPartPrefab.transform.position;
                pLeft.myPartPrefab.transform.parent = pLefti.myPartPrefab.transform.parent;

                Destroy(pLeft.myPartPrefab, 0.1f);
                pLeft.myPartPrefab = pLefti.myPartPrefab;

                pLefti.PartPoint = 0;
                pLeft.PartPoint = pointfinalLeft * 2;

                if (pLeft.PartPoint >= GameManager.Instance.score) { GameManager.Instance.score = pointfinalLeft * 2; }
                //GameManager.Instance.score = pointfinalLeft * 2;
                pLefti.myPartPrefab.transform.Find("Canvas/Text").GetComponent<Text>().text = (pointfinalLeft * 2).ToString();
                GameManager.Instance.GiveColor(pLefti.PartPoint, pLefti.myPartPrefab.gameObject);
                StartCoroutine(CheckCoroutine());
                return;
            }
        }
    }
}

