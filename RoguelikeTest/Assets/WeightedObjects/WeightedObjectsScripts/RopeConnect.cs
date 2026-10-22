using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RopeConnect : MonoBehaviour
{
    Camera cam;
    private SpringJoint2D springJoint;
    private bool clickedAnchored = false;
    private bool canClickTwice = false;
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


    }

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
}
