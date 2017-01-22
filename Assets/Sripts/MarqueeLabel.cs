using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MarqueeLabel : MonoBehaviour
{
    private float animTime;
    private float animOffset;

    public Text label;
    private Text labelClone = null;

    private const float FREEZE_TIME = 0.0f; // In seconds
    private const float LABEL_SPEED = 70.0f; // In pixels per second
    private const float LABEL_MARGIN = 40.0f; // In pixels

    private RectTransform rect = null;

    [SerializeField]
    private string initialTextToShow = "123456789A123456789B123456789C123456789D123456789E123456789F123456789G";

    public string TextToShow
    {
        set
        {
            if (label.text == value)
                return;

            animTime = 0;
            animOffset = 0;

            if (labelClone != null)
            {
                Destroy(labelClone);
                labelClone = null;
            }

            label.text = value;
        }
        get
        {
            return label.text;
        }
    }

    void Awake()
    {
        //Forces always first time
		if (initialTextToShow.Length == 0) {
			TextToShow = label.text;
		} else {
			TextToShow = initialTextToShow;
		}
    }

    void Start ()
    {
        rect = GetComponent<RectTransform>();
    }
	
	
	void Update ()
    {
        animTime += Time.deltaTime;

        if (animTime > FREEZE_TIME)
        {
            float preferredWidth = LayoutUtility.GetPreferredWidth(label.rectTransform);
            float marqueeWidth = rect.rect.width;

            if (preferredWidth > marqueeWidth)
            {
                if (labelClone == null)
                {
                    labelClone = Instantiate(label);
                    labelClone.transform.SetParent(label.transform.parent, false);
                }
                
                float cloneOffset = animOffset + preferredWidth + LABEL_MARGIN;
                if (cloneOffset <= 0)
                {
                    animOffset += preferredWidth + LABEL_MARGIN;
                    cloneOffset = animOffset + preferredWidth + LABEL_MARGIN;
                }

                animOffset -= Time.deltaTime * LABEL_SPEED;
                labelClone.rectTransform.anchoredPosition = new Vector2(cloneOffset, 0);
            }
            else
            {
                if (labelClone != null)
                {
                    Destroy(labelClone);
                    labelClone = null;
                }

                animOffset = 0;
            }
        }

        label.rectTransform.anchoredPosition = new Vector2(animOffset, 0);
    }
}
