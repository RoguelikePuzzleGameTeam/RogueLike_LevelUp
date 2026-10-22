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
    List<Vector3> ropePositions = new List<Vector3>();
    List<Vector3> ropeEndPositions = new List<Vector3>();

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
                LineRenderer line = clickedFirst.AddComponent<LineRenderer>();
                if (line)
                {
line.positionCount = 2;
                line.useWorldSpace = true;

                ropes.Add(line);
                ropePositions.Add(hit.point);
                }
                

                springJoint.anchor = hit.point - (Vector2)clickedFirst.transform.position;

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
                }
                else
                {
                    springJoint.connectedAnchor = hit2.point;
                }
                ropeEndPositions.Add(hit2.point);
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
        if (ropes.Count > 0 && ropePositions.Count > 0 && ropeEndPositions.Count > 0)
        {
            for (int i = 0; i < ropes.Count; i++)
            {
                if (ropes[i] != null && ropePositions[i] != null && ropeEndPositions[i] != null)
                {
                    ropes[i].SetPosition(0, ropePositions[i]);
                    ropes[i].SetPosition(1, ropeEndPositions[i]);
                }
            }
        }
    }
}
