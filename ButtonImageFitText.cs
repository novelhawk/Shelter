using UnityEngine;
using UnityEngine.UI;

public class ButtonImageFitText : MonoBehaviour
{
    public Image image;
    public Text text;

    private void Start()
    {
        print(string.Concat(new object[] { this.text.flexibleWidth, " ", this.text.minWidth, " ", this.text.preferredWidth }));
    }

    private void Update()
    {
    }
}

