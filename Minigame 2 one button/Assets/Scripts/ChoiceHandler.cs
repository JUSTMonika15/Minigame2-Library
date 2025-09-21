using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChoiceHandler : MonoBehaviour
{
    public TextMeshProUGUI[] options;
    private int currentIndex = 0;
    private float holdTime = 0f;
    public float confirmDuration = 2f; //how many seconds you need to hold the space bar to confirm
    private bool isHolding = false;

    void Start()
    {
        HighlightOption();
    }

    void Update()
    {
        //press space for switching New: I found out that hold the space still trigger this press logic, so I need to count how long we press
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isHolding = true;
            holdTime = 0f;
        }

        //hold space add more time
        if (isHolding && Keyboard.current.spaceKey.isPressed)
        {
            holdTime += Time.deltaTime;
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
        }

    }

    void HighlightOption()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    void ConfirmOption()
    {
        Debug.Log("Confirmed option: " + currentIndex);
    }
}