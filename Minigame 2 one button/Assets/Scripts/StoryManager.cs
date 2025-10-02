using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScenarioWrapper
{
    public StoryNode[] nodes;
}

public class StoryManager : MonoBehaviour
{
    public Image storyImg, fadeImg;
    public TMP_Text storyTmp;
    private Dictionary<string, StoryNode> storyNodes;
    public TMP_Text[] option;
    private string currentKey;
    //adding typing var
    private StoryNode currentNode;
    private int currentLineIndex = 0;
    private Coroutine typingRoutine;
    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        // Hide option text
        option[0].gameObject.SetActive(false);
        option[1].gameObject.SetActive(false);

        LoadScenario("Tutorial");
        currentKey = "";
        ShowStory(currentKey);
    }

    void LoadScenario(string fileName)
    {
        TextAsset jsonFile;

        // Load JSON text file from Resources
        jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogError("Scenario file not found in Resources");
            return;
        }

        // Parse to JsonUtility
        ScenarioWrapper wrapper = JsonUtility.FromJson<ScenarioWrapper>(jsonFile.text);

        if (wrapper == null || wrapper.nodes == null)
        {
            Debug.LogError("Failed to parse scenario JSON");
            return;
        }

        // Save to Dictionary
        storyNodes = new Dictionary<string, StoryNode>();
        foreach (StoryNode node in wrapper.nodes)
        {
            if (!storyNodes.ContainsKey(node.key))
            {
                storyNodes.Add(node.key, node);
            }
            else
            {
                Debug.LogWarning("Duplicate node key: " + node.key);
            }
        }
        Debug.Log("Scenario loaded. Total nodes: " + storyNodes.Count);
    }

    void ShowStory(string nodeKey)
    {
        Debug.Log("Current story key: " + currentKey);
        if (!storyNodes.ContainsKey(nodeKey))
        {
            LoadScenario("Scenario");
            StartCoroutine(RestartStory());
            return;
        }

        currentNode = storyNodes[nodeKey];
        // reset line index
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);
        typingRoutine = null;
        isTyping = false;
        skipTyping = false;

        storyTmp.text = "";
        currentLineIndex = 0;

        // start showing story
        ShowNextLineOrOptions();
    }

    public void OnAdvanceOrFastForward()
    {
        if (currentNode == null) return;

        if (isTyping)
        {
            //first time pressing space during typing: fast forward to complete current line
            skipTyping = true;
        }
        else
        {
            // Not typing: second space press/afterwards: go to next line or show options
            currentLineIndex++;
            ShowNextLineOrOptions();
        }
    }
   
    private void ShowNextLineOrOptions()
    {
        // story line available
        if (currentNode != null && currentLineIndex < currentNode.storyText.Length)
        {
            StartTypingCurrentLine();
            return;
        }

        // Only one option available: Go back to tutorial
        if (currentNode != null && currentNode.option != null && currentNode.option.Length == 1)
        {
            LoadScenario("Tutorial");
            StartCoroutine(RestartStory());
        }

        // No story line available: enter branching or reset
        else if (currentNode != null && currentNode.option != null && currentNode.option.Length > 0)
        {
            option[0].text = currentNode.option[0];
            option[1].text = currentNode.option[1];
            option[0].gameObject.SetActive(true);
            option[1].gameObject.SetActive(true);
        }
        else
        {
            // No options available: loop back to start or your desired "bad ending/reset" logic
            StartCoroutine(RestartStory());
        }
    }

    private void StartTypingCurrentLine()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);

        string line = currentNode.storyText[currentLineIndex];

        // Load image
        if (line.Contains('*'))
        {
            StartCoroutine(ShowSprite(line.Split('*')[1]));
            return;
        }

        // set text and reset
        storyTmp.text = line;
        storyTmp.maxVisibleCharacters = 0;

        typingRoutine = StartCoroutine(TypingEffect(storyTmp));
    }

    private IEnumerator ShowSprite(string fileName)
    {
        Debug.Log($"Showing sprite {fileName}");
        // Create new image
        Image newImg = Instantiate(storyImg, storyImg.transform.parent);
        newImg.sprite = Resources.Load<Sprite>($"Sprites/{fileName}");

        // Set layer order
        storyImg.transform.SetAsLastSibling();

        // Fade out original image and delete it
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            Color c = storyImg.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            storyImg.color = c;
            yield return null;
        }
        Destroy(storyImg.gameObject);

        // Switch storyImg to newImg
        storyImg = newImg;

        currentLineIndex++;
        ShowNextLineOrOptions();
    }

    public void OnChoiceConfirmed(string choice)
    {
        Debug.Log("StoryManager received choice: " + choice);
        currentKey += choice;
        ShowStory(currentKey);

        // Hide option text
        option[0].gameObject.SetActive(false);
        option[1].gameObject.SetActive(false);
    }

    IEnumerator TypingEffect(TMP_Text tmpText, float delay = 0.03f)
    {
        isTyping = true;
        skipTyping = false;

        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;
        int visibleCount = 0;

        AudioManager.instance.PlayTyping();

        while (visibleCount <= totalVisibleCharacters)
        {
            if (skipTyping)
            {
                // show all characters immediately
                tmpText.maxVisibleCharacters = totalVisibleCharacters;
                break;
            }

            tmpText.maxVisibleCharacters = visibleCount;
            visibleCount++;
            yield return new WaitForSeconds(delay);
        }

        AudioManager.instance.StopTyping();

        // Complete the line display and enter "wait for player to press space to continue" state
        isTyping = false;
        typingRoutine = null;
    }

    private IEnumerator RestartStory()
    {
        option[0].gameObject.SetActive(false);
        option[1].gameObject.SetActive(false);
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);
        typingRoutine = null;
        isTyping = false;

        Debug.Log("Fade in");
        // Fade in
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            Color c = fadeImg.color;
            c.a = Mathf.Lerp(0f, 1f, t);
            fadeImg.color = c;
            yield return null;
        }

        // Reset scene
        storyImg.sprite = Resources.Load<Sprite>("Sprites/library");
        storyTmp.text = "";
        currentKey = "";

        Debug.Log("Fade out");
        // Fade out
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            Color c = fadeImg.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            fadeImg.color = c;
            yield return null;
        }

        fadeImg.color = new Color(0f, 0f, 0f, 0f);
        
        ShowStory(currentKey);
    }
}