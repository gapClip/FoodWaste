using UnityEngine;

public class RunningWorker : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float speed = 1.0f; // 走る速度
    [SerializeField] private Vector3 RespornPosition;
    [SerializeField] private Vector3 ReturnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        // 走るアニメーションを再生しながら、オブジェクトを移動させる
        transform.position = Vector3.MoveTowards(transform.position, ReturnPosition, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, ReturnPosition) < 0.1f)
        {
            transform.position = RespornPosition;
        }
        
    }
}
