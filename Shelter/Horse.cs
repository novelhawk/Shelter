using Mod;
using Photon;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
    private float awayTimer;
    private TITAN_CONTROLLER controller;
    public GameObject dust;
    public GameObject myHero;
    private Vector3 setPoint;
    private float speed = 45f;
    private string State = "idle";
    private float timeElapsed;

    private void crossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
        if (PhotonNetwork.connected && photonView.isMine)
        {
            photonView.RPC(Rpc.CrossFade, PhotonTargets.Others, aniName, time);
        }
    }

    private void followed()
    {
        if (this.myHero != null)
        {
            this.State = "follow";
            this.setPoint = this.myHero.transform.position + Vector3.right * Random.Range(-6, 6) + Vector3.forward * Random.Range(-6, 6);
            this.setPoint.y = this.getHeight(this.setPoint + Vector3.up * 5f);
            this.awayTimer = 0f;
        }
    }

    private float getHeight(Vector3 pt)
    {
        RaycastHit hit;
        LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(pt, -Vector3.up, out hit, 1000f, mask2.value))
        {
            return hit.point.y;
        }
        return 0f;
    }

    public bool IsGrounded()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = 1 << LayerMask.NameToLayer("EnemyBox");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, mask3.value);
    }

    private void LateUpdate()
    {
        if (this.myHero == null && photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        if (this.State == "mounted")
        {
            if (this.myHero == null)
            {
                this.unmounted();
                return;
            }
            this.myHero.transform.position = transform.position + Vector3.up * 1.68f;
            this.myHero.transform.rotation = transform.rotation;
            this.myHero.rigidbody.velocity = rigidbody.velocity;
            if (this.controller.targetDirection != -874f)
            {
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), 100f * Time.deltaTime / (rigidbody.velocity.magnitude + 20f));
                if (this.controller.isWALKDown)
                {
                    rigidbody.AddForce(transform.forward * this.speed * 0.6f, ForceMode.Acceleration);
                    if (rigidbody.velocity.magnitude >= this.speed * 0.6f)
                    {
                        rigidbody.AddForce(-this.speed * 0.6f * rigidbody.velocity.normalized, ForceMode.Acceleration);
                    }
                }
                else
                {
                    rigidbody.AddForce(transform.forward * this.speed, ForceMode.Acceleration);
                    if (rigidbody.velocity.magnitude >= this.speed)
                    {
                        rigidbody.AddForce(-this.speed * rigidbody.velocity.normalized, ForceMode.Acceleration);
                    }
                }
                if (rigidbody.velocity.magnitude > 8f)
                {
                    if (!animation.IsPlaying("horse_Run"))
                    {
                        this.crossFade("horse_Run", 0.1f);
                    }
                    if (!this.myHero.animation.IsPlaying("horse_Run"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
                    }
                    if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                    {
                        this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                        photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, true);
                    }
                }
                else
                {
                    if (!animation.IsPlaying("horse_WALK"))
                    {
                        this.crossFade("horse_WALK", 0.1f);
                    }
                    if (!this.myHero.animation.IsPlaying("horse_idle"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
                    }
                    if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                    {
                        this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                        object[] objArray2 = new object[] { false };
                        photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, objArray2);
                    }
                }
            }
            else
            {
                this.toIdleAnimation();
                if (rigidbody.velocity.magnitude > 15f)
                {
                    if (!this.myHero.animation.IsPlaying("horse_Run"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
                    }
                }
                else if (!this.myHero.animation.IsPlaying("horse_idle"))
                {
                    this.myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
                }
            }
            if ((this.controller.isAttackDown || this.controller.isAttackIIDown) && this.IsGrounded())
            {
                rigidbody.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
            }
        }
        else if (this.State == "follow")
        {
            if (this.myHero == null)
            {
                this.unmounted();
                return;
            }
            if (rigidbody.velocity.magnitude > 8f)
            {
                if (!animation.IsPlaying("horse_Run"))
                {
                    this.crossFade("horse_Run", 0.1f);
                }
                if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                    object[] objArray3 = new object[] { true };
                    photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, objArray3);
                }
            }
            else
            {
                if (!animation.IsPlaying("horse_WALK"))
                {
                    this.crossFade("horse_WALK", 0.1f);
                }
                if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                    object[] objArray4 = new object[] { false };
                    photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, objArray4);
                }
            }
            float num = -Mathf.DeltaAngle(FengMath.getHorizontalAngle(transform.position, this.setPoint), gameObject.transform.rotation.eulerAngles.y - 90f);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (rigidbody.velocity.magnitude + 20f));
            if (Vector3.Distance(this.setPoint, transform.position) < 20f)
            {
                rigidbody.AddForce(transform.forward * this.speed * 0.7f, ForceMode.Acceleration);
                if (rigidbody.velocity.magnitude >= this.speed)
                {
                    rigidbody.AddForce(-this.speed * 0.7f * rigidbody.velocity.normalized, ForceMode.Acceleration);
                }
            }
            else
            {
                rigidbody.AddForce(transform.forward * this.speed, ForceMode.Acceleration);
                if (rigidbody.velocity.magnitude >= this.speed)
                {
                    rigidbody.AddForce(-this.speed * rigidbody.velocity.normalized, ForceMode.Acceleration);
                }
            }
            this.timeElapsed += Time.deltaTime;
            if (this.timeElapsed > 0.6f)
            {
                this.timeElapsed = 0f;
                if (Vector3.Distance(this.myHero.transform.position, this.setPoint) > 20f)
                {
                    this.followed();
                }
            }
            if (Vector3.Distance(this.myHero.transform.position, transform.position) < 5f)
            {
                this.unmounted();
            }
            if (Vector3.Distance(this.setPoint, transform.position) < 5f)
            {
                this.unmounted();
            }
            this.awayTimer += Time.deltaTime;
            if (this.awayTimer > 6f)
            {
                this.awayTimer = 0f;
                LayerMask mask2 = 1 << LayerMask.NameToLayer("Ground");
                if (Physics.Linecast(transform.position + Vector3.up, this.myHero.transform.position + Vector3.up, mask2.value))
                {
                    transform.position = new Vector3(this.myHero.transform.position.x, this.getHeight(this.myHero.transform.position + Vector3.up * 5f), this.myHero.transform.position.z);
                }
            }
        }
        else if (this.State == "idle")
        {
            this.toIdleAnimation();
            if (this.myHero != null && Vector3.Distance(this.myHero.transform.position, transform.position) > 20f)
            {
                this.followed();
            }
        }
        rigidbody.AddForce(new Vector3(0f, -50f * rigidbody.mass, 0f));
    }

    public void mounted()
    {
        this.State = "mounted";
        gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
    }

    [RPC]
    private void NetCrossFade(string aniName, float time)
    {
        animation.CrossFade(aniName, time);
    }

    [RPC]
    private void NetPlayAnimation(string aniName)
    {
        animation.Play(aniName);
    }

    [RPC]
    private void NetPlayAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
    }

    public void playAnimation(string aniName)
    {
        animation.Play(aniName);
        if (PhotonNetwork.connected && photonView.isMine)
        {
            photonView.RPC(Rpc.PlayAnimation, PhotonTargets.Others, aniName);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        animation.Play(aniName);
        animation[aniName].normalizedTime = normalizedTime;
        if (PhotonNetwork.connected && photonView.isMine)
        {
            photonView.RPC(Rpc.PlayAnimationAt, PhotonTargets.Others, aniName, normalizedTime);
        }
    }

    [RPC]
    private void SetDust(bool enable)
    {
        if (this.dust.GetComponent<ParticleSystem>().enableEmission)
        {
            this.dust.GetComponent<ParticleSystem>().enableEmission = enable;
        }
    }

    private void Start()
    {
        this.controller = gameObject.GetComponent<TITAN_CONTROLLER>();
    }

    private void toIdleAnimation()
    {
        if (rigidbody.velocity.magnitude > 0.1f)
        {
            if (rigidbody.velocity.magnitude > 15f)
            {
                if (!animation.IsPlaying("horse_Run"))
                {
                    this.crossFade("horse_Run", 0.1f);
                }
                if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                    photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, true);
                }
            }
            else
            {
                if (!animation.IsPlaying("horse_WALK"))
                {
                    this.crossFade("horse_WALK", 0.1f);
                }
                if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                    object[] objArray2 = new object[] { false };
                    photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, objArray2);
                }
            }
        }
        else
        {
            if (animation.IsPlaying("horse_idle1") && animation["horse_idle1"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (animation.IsPlaying("horse_idle2") && animation["horse_idle2"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (animation.IsPlaying("horse_idle3") && animation["horse_idle3"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (!animation.IsPlaying("horse_idle0") && !animation.IsPlaying("horse_idle1") && !animation.IsPlaying("horse_idle2") && !animation.IsPlaying("horse_idle3"))
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (animation.IsPlaying("horse_idle0"))
            {
                int num = Random.Range(0, 10000);
                if (num < 10)
                {
                    this.crossFade("horse_idle1", 0.1f);
                }
                else if (num < 20)
                {
                    this.crossFade("horse_idle2", 0.1f);
                }
                else if (num < 30)
                {
                    this.crossFade("horse_idle3", 0.1f);
                }
            }
            if (this.dust.GetComponent<ParticleSystem>().enableEmission)
            {
                this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                object[] objArray3 = new object[] { false };
                photonView.RPC(Rpc.HorseParticles, PhotonTargets.Others, objArray3);
            }
            rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
        }
    }

    public void unmounted()
    {
        this.State = "idle";
        gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
    }
}

