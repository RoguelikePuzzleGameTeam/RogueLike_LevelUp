using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RopeConnect : MonoBehaviour
{
    Camera cam;

    private DistanceJoint2D distanceJoint;
    private SpringJoint2D springJoint;

    private bool clickedAnchored = false;
    private bool canClickTwice = false;
    List<LineRenderer> ropes = new List<LineRenderer>();
    List<Transform> ropePositions = new List<Transform>();
    List<Transform> ropeEndPositions = new List<Transform>();

    [Header("Used Objects")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject ropeAnchorPoint;

    [Header("Distance Joint Settings")]
    public float dampening = 1f;
    public float distance = 5f;
    public float frequency = 1f;

    public bool autoConfigureDistance = false;
    public bool maxDistanceOnly = true;



    /*
     * 
     *  TO DO: Make the clicking create both the anchor points for the spring joint and distance joint. When the object is within the proper distance have the distance joint enabled.
     *  when the object is outside of the distance joint range, have the spring joint enabled to pull the object back into range. Make sure to give the spring joint a low frequency
     *  to make sure it does not pull too hard. Give the switching distance a bit of leeway so that it does not switch back and forth between the two joints too quickly. The two joints
     *  should never be enabled at the same time.
     * 
     */


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CheckClick();
        Click2();
        UpdateTethers();

        double distance = Vector2.Distance(distanceJoint.transform.TransformPoint(distanceJoint.anchor), distanceJoint.connectedAnchor);

        Debug.Log("Distance: " + distance);

    }

    /* CLICK DETECTION
     */
    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0)) { 
            GameObject clickedFirst;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);

            if (hit && hit.collider.gameObject.CompareTag("CanBeTied") && !canClickTwice)
            {
                //find the clicked object and add a spring joint to it
                clickedFirst = hit.collider.gameObject;
                distanceJoint = clickedFirst.AddComponent<DistanceJoint2D>();

                // create a spring joint to the clicked object to allow the object to get pulled in with more fluid physics
                springJoint = clickedFirst.AddComponent<SpringJoint2D>();

                // disable both joints until the second click is made
                distanceJoint.enabled = false;
                springJoint.enabled = false;

                //Configure the distance joint settings
                distanceJoint.autoConfigureDistance = autoConfigureDistance;
                distanceJoint.enableCollision = true;
                distanceJoint.distance = distance;
                distanceJoint.maxDistanceOnly = maxDistanceOnly;


                //Create the line
                line = Instantiate(line, clickedFirst.transform);
                line.useWorldSpace = true;
                ropes.Add(line);

                //create a new anchor point at the hit point and set it as the anchor for the spring joint
                var newAnchorPoint = Instantiate(ropeAnchorPoint);
                newAnchorPoint.transform.position = hit.point;

                // Set the parent of the new anchor point to the clicked object
                newAnchorPoint.transform.parent = clickedFirst.transform;

                // Set the anchor of the spring joint to the hit point relative to the clicked object's position
                distanceJoint.anchor = hit.point - (Vector2)clickedFirst.transform.position;
                ropePositions.Add(newAnchorPoint.transform);

                // Check if they clicked before allowing them to click again
                clickedAnchored = true;
                StartCoroutine(ClickCooldown());
            }
        
        }

    }


    void Click2()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (distanceJoint != null && clickedAnchored && Input.GetMouseButtonDown(0) && canClickTwice)
        {

            RaycastHit2D hit2 = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);

            if (hit2.collider != null)
            {
                if (hit2.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    distanceJoint.connectedBody = hit2.collider.gameObject.GetComponent<Rigidbody2D>();
                    var newAnchorPoint = Instantiate(ropeAnchorPoint, hit2.transform);
                    newAnchorPoint.transform.position = distanceJoint.connectedBody.transform.TransformPoint(distanceJoint.connectedAnchor);
                    ropeEndPositions.Add(newAnchorPoint.transform);
                }
                else
                {
                    distanceJoint.connectedAnchor = hit2.point;
                    var newAnchorPoint = Instantiate(ropeAnchorPoint, hit2.transform);
                    newAnchorPoint.transform.position = hit2.point;
                    ropeEndPositions.Add(newAnchorPoint.transform);
                }
              
                distanceJoint.enabled = true;
                canClickTwice = false;
            }

        }
    }

    IEnumerator ClickCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        canClickTwice = true;
    }

    /* LINE RENDERERS
     */

    private void UpdateTethers()
    {
        if (ropes.Count > 0 && ropePositions.Count > 0 && ropeEndPositions.Count > 0 && ropeEndPositions.Count == ropes.Count)
        {
            for (int i = 0; i < ropes.Count; i++)
            {
                if (ropes[i] != null && ropePositions[i] != null && ropeEndPositions[i] != null)
                {



                    Debug.Log("Log");
                    ropes[i].SetPosition(0, ropePositions[i].position);
                    ropes[i].SetPosition(1, ropeEndPositions[i].position);
                }
            }
        }
    }
}
