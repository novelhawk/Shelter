using UnityEngine;

// ReSharper disable once CheckNamespace
public class CheckHitGround : MonoBehaviour // Used by TITAN, FEMALE_TITAN & such
{
    public bool isGrounded;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("EnemyAABB"))
            this.isGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("EnemyAABB"))
            this.isGrounded = true;
    }
}