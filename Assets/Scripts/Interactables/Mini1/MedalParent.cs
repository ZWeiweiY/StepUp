using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalParent : MonoBehaviour
{ 
    public Vector3 playerToMedalDistanceOffset = new Vector3(0f, 0f, 10f);

    [SerializeField] private float degreesPerSecond = 60.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    private float safteyRotationSpeed = 0.2f;

    [SerializeField] private float rotateTime = 5.0f;

    [SerializeField] private float scatterTime = 4f;

    public GameObject[] children {get; private set;}

    private Vector3[] childrenFinalPos;

    private float originalRotationSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        // Example usage of GetAllChildGameObjects
        children = GetAllChildGameObjects();
        childrenFinalPos = SetupRandomPositions();
        originalRotationSpeed = rotationSpeed;
    }

    // Method to get all child game objects
    private GameObject[] GetAllChildGameObjects()
    {
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childObjects.Add(child.gameObject);
        }
        return childObjects.ToArray();
    }

    private Vector3[] SetupRandomPositions()
    {
        List<Vector3> vector3s = new List<Vector3>();
        foreach (Transform child in transform)
        {
            vector3s.Add(new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-2f, 2f)) + playerToMedalDistanceOffset);
        }
        return vector3s.ToArray();
    }

    // Update is called once per frame
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCoroutine(TurnAndScatterTest());
        // }
    }

    private void SetupInitPositions()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].transform.position = playerToMedalDistanceOffset;
            children[i].transform.localRotation = Quaternion.Euler(0, 360f / children.Length * i, 0);
            // Debug.Log(children[i] + " reset pos " + children[i].transform.localRotation);
        }
    }

    public IEnumerator TurnAndScatter()
    {
        // rotationSpeed = originalRotationSpeed;
        SetupInitPositions();
        yield return StartCoroutine(WheelTurn());

        yield return StartCoroutine(ScatterToRandom());

        foreach (GameObject piece in children)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = true;
            rb.isKinematic = true;
            rb.excludeLayers = ~0;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        yield return null;
    }

    private IEnumerator WheelTurn()
    {
        float elapsedTime = 0;
        while (elapsedTime < rotateTime)
        {
            Vector3 transformRotationVector = new Vector3(0, degreesPerSecond, 0);

            transform.position = playerToMedalDistanceOffset;
            // Debug.Log("tpos" + transform.position);

            transform.Rotate(transformRotationVector * rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            rotationSpeed = Mathf.Max(safteyRotationSpeed, rotationSpeed - rotationSpeed / rotateTime * Time.deltaTime);
            foreach (GameObject piece in children)
            {
                Rigidbody rb = piece.GetComponent<Rigidbody>();

                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.excludeLayers = 0;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;


                Vector3 moveDirection = CalculatePieceMoveDirection(piece);
                // Debug.Log(piece + " moving towards: " + (piece.transform.position+moveDirection));
                // piece.transform.position = playerToMedalDistanceOffset;
                // Debug.Log(piece + " pos: " + piece.transform.position);
                // piece.transform.position -= -(moveDirection + transformRotationVector.normalized) * 0.6f * Time.deltaTime;
                piece.transform.position = transform.position + moveDirection;
            }
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ScatterToRandom()
    {
        //Debug.Log("Start Scatter");

        float elapsedTime = 0;
        
        while(elapsedTime < scatterTime)
        {
            elapsedTime += Time.deltaTime;

            for(int i = 0; i < children.Length; i++)
            {
                Vector3 startPos = children[i].transform.position;
                //Debug.Log(children[i] + " move towards " + childrenFinalPos[i]);
                children[i].transform.position += (childrenFinalPos[i] - startPos) * Time.deltaTime;
                children[i].transform.localRotation *= Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 1f) / scatterTime, 0f);
            }
            
            yield return null;
        }
        
        yield return new WaitForEndOfFrame();
    }

    private Vector3 GetObjectCenterPositionOnXZ(GameObject gameObject)
    {
        UnityEngine.Vector3 centerPosOnXZ;
        Renderer gameObjectRenderer = gameObject.GetComponent<Renderer>();

        centerPosOnXZ = gameObjectRenderer.bounds.center + playerToMedalDistanceOffset;
        // centerPosOnXZ.y = 0;
        // Debug.Log("CenterAt" + centerPosOnXZ);
        return centerPosOnXZ;
    }

    private Vector3 CalculatePieceMoveDirection(GameObject medalPiece)
    {
        Vector3 centerPosition = medalPiece.transform.position + playerToMedalDistanceOffset;
        centerPosition.y = 0;
        Vector3 realPieceCenterPos = GetObjectCenterPositionOnXZ(medalPiece);
        realPieceCenterPos.y = centerPosition.y;
        Vector3 moveDirection = (realPieceCenterPos - centerPosition + playerToMedalDistanceOffset).normalized;
        // Debug.Log(medalPiece + " moving towards: " + moveDirection);

        return moveDirection;
    }
}
