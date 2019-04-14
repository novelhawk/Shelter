using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using Game.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CharacterCreateAnimationControl : MonoBehaviour
{
    public string currentAnimation;
    public float interval = 10f;
    public HERO_SETUP setup;
    public float timeElapsed;

    private void play(string id)
    {
        this.currentAnimation = id;
        animation.Play(id);
    }

    public void playAttack(string id)
    {
        switch (id)
        {
            case "mikasa":
                this.currentAnimation = "attack3_1";
                break;

            case "levi":
                this.currentAnimation = "attack5";
                break;

            case "sasha":
                this.currentAnimation = "special_sasha";
                break;

            case "jean":
                this.currentAnimation = "grabbed_jean";
                break;

            case "marco":
                this.currentAnimation = "special_marco_0";
                break;

            case "armin":
                this.currentAnimation = "special_armin";
                break;

            case "petra":
                this.currentAnimation = "special_petra";
                break;
        }
        animation.Play(this.currentAnimation);
    }

    private void Start()
    {
        this.setup = gameObject.GetComponent<HERO_SETUP>();
        this.currentAnimation = "stand_levi";
        this.play(this.currentAnimation);
    }

    public void toStand()
    {
        if (this.setup.myCostume.sex == Sex.Female)
        {
            this.currentAnimation = "stand";
        }
        else
        {
            this.currentAnimation = "stand_levi";
        }
        animation.CrossFade(this.currentAnimation, 0.1f);
        this.timeElapsed = 0f;
    }

    private void Update()
    {
        if (this.currentAnimation != "stand" && this.currentAnimation != "stand_levi")
        {
            if (animation[this.currentAnimation].normalizedTime >= 1f)
            {
                if (this.currentAnimation == "attack3_1")
                {
                    this.play("attack3_2");
                }
                else if (this.currentAnimation == "special_sasha")
                {
                    this.play("run_sasha");
                }
                else
                {
                    this.toStand();
                }
            }
        }
        else
        {
            this.timeElapsed += Time.deltaTime;
            if (this.timeElapsed > this.interval)
            {
                this.timeElapsed = 0f;
                if (Random.Range(0, 100) < 35)
                {
                    this.play("salute");
                }
                else if (Random.Range(0, 100) < 35)
                {
                    this.play("supply");
                }
                else
                {
                    this.play("dodge");
                }
            }
        }
    }
}

