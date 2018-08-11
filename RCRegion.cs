using UnityEngine;

public class RCRegion
{
    private float dimX;
    private float dimY;
    private float dimZ;
    public Vector3 location;
    public GameObject myBox;

    public RCRegion(Vector3 loc, float x, float y, float z)
    {
        this.location = loc;
        this.dimX = x;
        this.dimY = y;
        this.dimZ = z;
    }

    public float GetRandomX()
    {
        return this.location.x + Random.Range(-this.dimX / 2f, this.dimX / 2f);
    }

    public float GetRandomY()
    {
        return this.location.y + Random.Range(-this.dimY / 2f, this.dimY / 2f);
    }

    public float GetRandomZ()
    {
        return this.location.z + Random.Range(-this.dimZ / 2f, this.dimZ / 2f);
    }
}

