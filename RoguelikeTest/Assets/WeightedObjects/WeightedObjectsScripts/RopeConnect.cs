using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RopeConnect : MonoBehaviour
{
    Camera cam;
    private SpringJoint2D springJoint;
    private bool clickedAnchored = false;
    private bool canClickTwice = false;
    List<LineRenderer> ropes = new List<LineRenderer>();
    List<Transform> ropePositions = new List<Transform>();
    List<Transform> ropeEndPositions = new List<Transform>();
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject ropeAnchorPoint;

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
                clickedFirst = hit.collider.gameObject;
                springJoint = clickedFirst.GetComponent<SpringJoint2D>();
                line = Instantiate(line, clickedFirst.transform);
                line.useWorldSpace = true;
                ropes.Add(line);
                var newAnchorPoint = Instantiate(ropeAnchorPoint, clickedFirst.transform);
                ropeAnchorPoint.transform.position = transform.TransformPoint(springJoint.anchor);
                springJoint.anchor = hit.point - (Vector2)clickedFirst.transform.position;
                ropePositions.Add(ropeAnchorPoint.transform);

                clickedAnchored = true;
                StartCoroutine(ClickCooldown());
            }
        
        }

    }

    void Click2()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (springJoint != null && clickedAnchored && Input.GetMouseButtonDown(0) && canClickTwice)
        {

            RaycastHit2D hit2 = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);

            if (hit2.collider != null)
            {
                if (hit2.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    springJoint.connectedBody = hit2.collider.gameObject.GetComponent<Rigidbody2D>();
                    var newAnchorPoint = Instantiate(ropeAnchorPoint, hit2.transform);
                    ropeAnchorPoint.transform.position = springJoint.connectedBody.transform.TransformPoint(springJoint.connectedAnchor);
                    ropeEndPositions.Add(newAnchorPoint.transform);
                }
                else
                {
                    springJoint.connectedAnchor = hit2.point;
                    var newAnchorPoint = Instantiate(ropeAnchorPoint, hit2.transform);
                    ropeAnchorPoint.transform.position = transform.TransformPoint(springJoint.connectedAnchor);
                    ropeEndPositions.Add(newAnchorPoint.transform);
                }
              
                springJoint.enabled = true;
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
