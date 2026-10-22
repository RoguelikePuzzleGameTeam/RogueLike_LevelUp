using UnityEngine;

public class ShowCase : MonoBehaviour
{
    private Rigidbody2D rig;
    [SerializeField] private Vector2 force = new Vector2(10, 10);
    [SerializeField] private Rigidbody2D rigField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.AddForce(force, ForceMode2D.Impulse);
        rigField.AddForce(force, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
