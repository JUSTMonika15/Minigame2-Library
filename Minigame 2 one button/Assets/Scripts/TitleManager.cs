using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public TMP_Text titleTmp, guideTmp;
    public Image fadeImg;
    private float blinkSpeed = 4f;
    private bool blink;
    public ChoiceHandler choiceHandler;
    public StoryManager storyManager;

    void Start()
    {
        blink = false;
        choiceHandler.gameObject.SetActive(false);
        storyManager.gameObject.SetActive(false);
        StartCoroutine(TypeTitle());
    }

    void Update()
    {
        if (blink)
        {
            // Guide TMP blinks while title manager is active
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
            Color c = guideTmp.color;
            c.a = alpha;
            guideTmp.color = c;
        }

        if (blink && Keyboard.current.spaceKey.wasPressedThisFrame)
            StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        blink = false;
        storyManager.gameObject.SetActive(true);

        // Fade out all children of this canvas
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            fadeImg.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, t));
            titleTmp.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t));
            guideTmp.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        choiceHandler.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private IEnumerator TypeTitle()
    {
        float delay = 0.03f;
        titleTmp.ForceMeshUpdate();
        TMP_TextInfo textInfo = titleTmp.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;
        int visibleCount = 0;

        AudioManager.instance.PlayTyping();

        while (visibleCount <= totalVisibleCharacters)
        {
            titleTmp.maxVisibleCharacters = visibleCount;
            visibleCount++;
            yield return new WaitForSeconds(delay);
        }

        AudioManager.instance.StopTyping();

        yield return new WaitForSeconds(1f);
        blink = true;
    }
}
