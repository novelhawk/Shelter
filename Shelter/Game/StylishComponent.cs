using Game;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class StylishComponent : MonoBehaviour // Used by GO "Stylish"
{
    public GameObject bar;
    public int chainKillRank;
    public float[] chainRankMultiplier;
    public float chainTime;
    public float duration;
    public Vector3 exitPosition;
    public bool flip;
    public bool hasLostRank;
    public GameObject labelChain;
    public GameObject labelHits;
    public GameObject labelS;
    public GameObject labelS1;
    public GameObject labelS2;
    public GameObject labelsub;
    public GameObject labelTotal;
    public Vector3 originalPosition;
    public float R;
    public int styleHits;
    public float stylePoints;
    public int styleRank;
    public int[] styleRankDepletions;
    public int[] styleRankPoints;
    public string[,] styleRankText;
    public int styleTotalDamage;

    public StylishComponent()
    {
        string[,] textArray1 = { { "D", "eja Vu" }, { "C", "asual" }, { "B", "oppin!" }, { "A", "mazing!" }, { "S", "ensational!" }, { "S", "pectacular!!" }, { "S", "tylish!!!" }, { "X", "TREEME!!!" } };
        styleRankText = textArray1;
        chainRankMultiplier = new[] { 1f, 1.1f, 1.2f, 1.3f, 1.5f, 1.7f, 2f, 2.3f, 2.5f };
        styleRankPoints = new[] { 350, 950, 2450, 4550, 7000, 15000, 100000 };
        styleRankDepletions = new[] { 1, 2, 5, 10, 15, 20, 25, 25 };
    }

    private int GetRankPercentage()
    {
        if (styleRank > 0 && styleRank < styleRankPoints.Length)
        {
            return (int) ((stylePoints - styleRankPoints[styleRank - 1]) * 100f / (styleRankPoints[styleRank] - styleRankPoints[styleRank - 1]));
        }
        if (styleRank == 0)
        {
            return (int) (stylePoints * 100f) / styleRankPoints[styleRank];
        }
        return 100;
    }

    private int GetStyleDepletionRate()
    {
        return styleRankDepletions[styleRank];
    }

    public void reset()
    {
        styleTotalDamage = 0;
        chainKillRank = 0;
        chainTime = 0f;
        styleRank = 0;
        stylePoints = 0f;
        styleHits = 0;
    }

    private void setPosition()
    {
        originalPosition = new Vector3((int) (Screen.width * 0.5f - 2f), (int) (Screen.height * 0.5f - 150f), 0f);
        exitPosition = new Vector3(Screen.width, originalPosition.y, originalPosition.z);
    }

    private void SetRank()
    {
        int oldStyleRank = this.styleRank;
        int index = 0;
        while (index < styleRankPoints.Length)
        {
            if (stylePoints <= styleRankPoints[index])
            {
                break;
            }
            index++;
        }
        if (index < styleRankPoints.Length)
        {
            this.styleRank = index;
        }
        else
        {
            this.styleRank = styleRankPoints.Length;
        }
        if (this.styleRank < oldStyleRank)
        {
            if (hasLostRank)
            {
                stylePoints = 0f;
                styleHits = 0;
                styleTotalDamage = 0;
                this.styleRank = 0;
            }
            else
            {
                hasLostRank = true;
            }
        }
        else if (this.styleRank > oldStyleRank)
        {
            hasLostRank = false;
        }
    }

    private void setRankText()
    {
        var markLabel = labelS.GetComponent<UILabel>();
 
        labelS2.GetComponent<UILabel>().text = string.Empty;
        labelS1.GetComponent<UILabel>().text = string.Empty;
        switch (styleRank)
        {
            case 0:
                markLabel.text = $"[{FengColor.RateD}]D";
                break;
            case 1:
                markLabel.text = $"[{FengColor.RateC}]C";
                break;
            case 2:
                markLabel.text = $"[{FengColor.RateB}]B";
                break;
            case 3:
                markLabel.text = $"[{FengColor.RateA}]A";
                break;
            case 4:
                markLabel.text = $"[{FengColor.RateS}]S";
                break;
            case 5:
                markLabel.text = $"[{FengColor.RateSS}]S";
                markLabel.text = $"[{FengColor.RateSS}]S";
                break;
            case 6:
                markLabel.text = $"[{FengColor.RateSSS}]S";
                markLabel.text = $"[{FengColor.RateSSS}]S";
                markLabel.text = $"[{FengColor.RateSSS}]S";
                break;
            case 7:
                markLabel.text = $"[{FengColor.RateX}]X";
                break;
            default:
                markLabel.text = string.Empty;
                break;
        }
        
        labelsub.GetComponent<UILabel>().text = styleRankText[styleRank, 1];
    }

    private void shakeUpdate()
    {
        if (duration > 0f)
        {
            duration -= Time.deltaTime;
            if (flip)
            {
                gameObject.transform.localPosition = originalPosition + Vector3.up * R;
            }
            else
            {
                gameObject.transform.localPosition = originalPosition - Vector3.up * R;
            }
            flip = !flip;
            if (duration <= 0f)
            {
                gameObject.transform.localPosition = originalPosition;
            }
        }
    }

    private void Start()
    {
        setPosition();
        transform.localPosition = exitPosition;
    }

    private void startShake(int intensity, float time)
    {
        if (this.duration < time)
        {
            this.R = intensity;
            this.duration = time;
        }
    }

    public void Style(int damage)
    {
        if (damage != -1)
        {
            stylePoints += (int) ((damage + 200) * chainRankMultiplier[chainKillRank]);
            styleTotalDamage += damage;
            chainKillRank = chainKillRank >= chainRankMultiplier.Length - 1 ? chainKillRank : chainKillRank + 1;
            chainTime = 5f;
            styleHits++;
            SetRank();
        }
        else if (stylePoints == 0f)
        {
            stylePoints++;
            SetRank();
        }
        startShake(5, 0.3f);
        setPosition();
        labelTotal.GetComponent<UILabel>().text = ((int) stylePoints).ToString();
        labelHits.GetComponent<UILabel>().text = styleHits + (styleHits <= 1 ? "Hit" : "Hits");
        if (chainKillRank == 0)
        {
            labelChain.GetComponent<UILabel>().text = string.Empty;
        }
        else
        {
            labelChain.GetComponent<UILabel>().text = "x" + chainRankMultiplier[chainKillRank] + "!";
        }
    }

    private void Update()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing)
        {
            if (stylePoints > 0f)
            {
                setRankText();
                bar.GetComponent<UISprite>().fillAmount = GetRankPercentage() * 0.01f;
                stylePoints -= GetStyleDepletionRate() * Time.deltaTime * 10f;
                SetRank();
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, exitPosition, Time.deltaTime * 3f);
            }
            if (chainTime > 0f)
            {
                chainTime -= Time.deltaTime;
            }
            else
            {
                chainTime = 0f;
                chainKillRank = 0;
            }
            shakeUpdate();
        }
    }
}

