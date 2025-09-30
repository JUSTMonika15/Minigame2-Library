using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ScenarioWrapper
{
    public StoryNode[] nodes;
}

public class StoryManager : MonoBehaviour
{
    public TMP_Text storyTmp;
    private Dictionary<string, StoryNode> storyNodes;
    public TMP_Text[] option;
    private string currentKey;
    private bool tutorialCompleted = false;

    void Awake()
    {
        
    }

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
            tutorialCompleted = true;
            LoadScenario("Scenario");
            currentKey = "";
            ShowStory(currentKey);
            return;
        }

        StoryNode node = storyNodes[nodeKey];
        StopAllCoroutines();
        StartCoroutine(ShowStoryCoroutine(node));
    }

    IEnumerator ShowStoryCoroutine(StoryNode node)
    {
        storyTmp.text = "";

        // TODO: change delay time
        // TODO: stack story text (not disappearing)
        // Show story text
        foreach (string line in node.storyText)
        {
            storyTmp.text = line;
            yield return TypingEffect(storyTmp);
            yield return new WaitForSeconds(1f);
        }

        Debug.Log(node.option);
        // Empty option -> Reset story (Bad end, loop)
        if (node.option.Length == 0)
        {
            currentKey = "";
            ShowStory(currentKey);
        }
        else
        {
            option[0].text = node.option[0];
            option[1].text = node.option[1];

            // Show option text
            option[0].gameObject.SetActive(true);
            option[1].gameObject.SetActive(true);
        }
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
        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;
        int visibleCount = 0;

        AudioManager.instance.PlayTyping();
        while (visibleCount <= totalVisibleCharacters)
        {
            tmpText.maxVisibleCharacters = visibleCount;
            visibleCount++;
            yield return new WaitForSeconds(delay);
        }
        AudioManager.instance.StopTyping();
    }
}