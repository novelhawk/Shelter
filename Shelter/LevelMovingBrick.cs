using UnityEngine;

public class LevelMovingBrick : MonoBehaviour
{
    private Vector3 pointA;
    private Vector3 pointB;
    public GameObject pointGOA;
    public GameObject pointGOB;
    public float speed = 10f;
    public bool towardsA = true;

    private void Start()
    {
        this.pointA = this.pointGOA.transform.position;
        this.pointB = this.pointGOB.transform.position;
        Destroy(this.pointGOA);
        Destroy(this.pointGOB);
    }

    private void Update()
    {
        if (this.towardsA)
        {
            transform.position = Vector3.MoveTowards(transform.position, this.pointA, this.speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, this.pointA) < 2f)
            {
                this.towardsA = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, this.pointB, this.speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, this.pointB) < 2f)
            {
                this.towardsA = true;
            }
        }
    }
}

