using Game;
using Game.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class HERO_DEAD_BODY_SETUP : MonoBehaviour
{
    public GameObject blood_arm_l;
    public GameObject blood_arm_r;
    public GameObject blood_lower;
    public GameObject blood_upper;
    public GameObject blood_upper1;
    public GameObject blood_upper2;
    public GameObject chest;
    public GameObject col_chest;
    public GameObject col_head;
    public GameObject col_lower_arm_l;
    public GameObject col_lower_arm_r;
    public GameObject col_shin_l;
    public GameObject col_shin_r;
    public GameObject col_thigh_l;
    public GameObject col_thigh_r;
    public GameObject col_upper_arm_l;
    public GameObject col_upper_arm_r;
    public GameObject hand_l;
    public GameObject hand_r;
    public GameObject head;
    public GameObject leg;
    private float lifetime = 15f;

    public void init(string aniname, float time, BodyPart part)
    {
        animation.Play(aniname);
        animation[aniname].normalizedTime = time;
        animation[aniname].speed = 0f;
        switch (part)
        {
            case BodyPart.Upper:
                this.col_upper_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_upper_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_r.GetComponent<CapsuleCollider>().enabled = false;
                Destroy(this.leg);
                Destroy(this.hand_l);
                Destroy(this.hand_r);
                Destroy(this.blood_lower);
                Destroy(this.blood_arm_l);
                Destroy(this.blood_arm_r);
                gameObject.GetComponent<HERO_SETUP>().createHead2();
                gameObject.GetComponent<HERO_SETUP>().createUpperBody2();
                break;

            case BodyPart.LeftArm:
                this.col_upper_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_head.GetComponent<CapsuleCollider>().enabled = false;
                this.col_chest.GetComponent<BoxCollider>().enabled = false;
                Destroy(this.head);
                Destroy(this.chest);
                Destroy(this.leg);
                Destroy(this.hand_r);
                Destroy(this.blood_lower);
                Destroy(this.blood_upper);
                Destroy(this.blood_upper1);
                Destroy(this.blood_upper2);
                Destroy(this.blood_arm_r);
                gameObject.GetComponent<HERO_SETUP>().createLeftArm();
                break;

            case BodyPart.RightArm:
                this.col_upper_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_thigh_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_shin_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_head.GetComponent<CapsuleCollider>().enabled = false;
                this.col_chest.GetComponent<BoxCollider>().enabled = false;
                Destroy(this.head);
                Destroy(this.chest);
                Destroy(this.leg);
                Destroy(this.hand_l);
                Destroy(this.blood_lower);
                Destroy(this.blood_upper);
                Destroy(this.blood_upper1);
                Destroy(this.blood_upper2);
                Destroy(this.blood_arm_l);
                gameObject.GetComponent<HERO_SETUP>().createRightArm();
                break;

            case BodyPart.Lower:
                this.col_upper_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_l.GetComponent<CapsuleCollider>().enabled = false;
                this.col_upper_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_lower_arm_r.GetComponent<CapsuleCollider>().enabled = false;
                this.col_head.GetComponent<CapsuleCollider>().enabled = false;
                this.col_chest.GetComponent<BoxCollider>().enabled = false;
                Destroy(this.head);
                Destroy(this.chest);
                Destroy(this.hand_l);
                Destroy(this.hand_r);
                Destroy(this.blood_upper);
                Destroy(this.blood_upper1);
                Destroy(this.blood_upper2);
                Destroy(this.blood_arm_l);
                Destroy(this.blood_arm_r);
                gameObject.GetComponent<HERO_SETUP>().createLowerBody();
                break;
        }
    }

    private void Update()
    {
        this.lifetime -= Time.deltaTime;
        if (this.lifetime <= 0f)
        {
            gameObject.GetComponent<HERO_SETUP>().deleteCharacterComponent2();
            Destroy(gameObject);
        }
    }
}

