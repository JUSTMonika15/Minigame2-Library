using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChoiceHandler : MonoBehaviour
{
    public TMP_Text[] options;
    private int currentIndex = 0;
    private float holdTime = 0f;
    public float confirmDuration = 1f; //how many seconds you need to hold the space bar to confirm
    private bool isHolding = false;

    //adding link to story
    public StoryManager storyManager;

    public Image holdProgressImage;
    public Vector2 progressBarOffset = new Vector2(0, -30); // The bar will show below the options，can be changed in the inspector
    void Start()
    {
        HighlightOption();
        holdProgressImage.fillAmount = 0f;
    }

    void Update()
    {

        bool anyOptionActive = options != null && options.Length > 0 && System.Array.Exists(options, o => o.gameObject.activeSelf);
        if (holdProgressImage != null)
            holdProgressImage.gameObject.SetActive(anyOptionActive);

        if (!anyOptionActive)
            return;
        //press space for switching New: I found out that hold the space still trigger this press logic, so I need to count how long we press
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isHolding = true;
            holdTime = 0f;
            if (holdProgressImage != null)
            {
                MoveProgressBarToCurrentOption();
                holdProgressImage.gameObject.SetActive(true);
                holdProgressImage.fillAmount = 0f;
            }
        }

        //hold space add more time
        if (isHolding && Keyboard.current.spaceKey.isPressed)
        {
            holdTime += Time.deltaTime;
            if (holdProgressImage != null)
            {
                holdProgressImage.fillAmount = Mathf.Clamp01(holdTime / confirmDuration);
            }
        }

        //release the space bar and count time
        if (isHolding && Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            if (holdTime >= confirmDuration)
            {
                ConfirmOption(); // enough time for holding
            }
            else
            {
                currentIndex = (currentIndex + 1) % options.Length;
                HighlightOption();
            }
            isHolding = false;
            holdTime = 0f;
            if (holdProgressImage != null)
            {
                holdProgressImage.gameObject.SetActive(false);
                holdProgressImage.fillAmount = 0f;
            }
        }

    }

    void HighlightOption()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    void MoveProgressBarToCurrentOption()
    {
        if (holdProgressImage != null && options != null && options.Length > currentIndex)
        {
            // let the bar follows the option
            //  set as a child
            holdProgressImage.transform.SetParent(options[currentIndex].transform, false);
            // set offset
            RectTransform barRect = holdProgressImage.GetComponent<RectTransform>();
            barRect.anchoredPosition = progressBarOffset;
        }
    }
    void ConfirmOption()
    {
        // link to story manager
        if (storyManager != null)
        {
            string choice = currentIndex == 0 ? "A" : "B";
            Debug.Log("Confirmed option: " + choice);
            storyManager.OnChoiceConfirmed(choice);
        }
    }
}