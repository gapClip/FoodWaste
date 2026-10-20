using UnityEngine;

public class shipmentAnim : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(30, 0, 0), speed * Time.deltaTime);
    }
}
