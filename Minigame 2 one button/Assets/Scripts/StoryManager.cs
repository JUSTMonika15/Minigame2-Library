using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScenarioWrapper
{
    public StoryNode[] nodes;
}

public class StoryManager : MonoBehaviour
{
    public Text storyTextUI;
    private TextAsset jsonFile;
    private Dictionary<string, StoryNode> storyNodes;
    public Text[] option;

    void Awake()
    {
        // Load JSON text file from Resources
        jsonFile = Resources.Load<TextAsset>("Scenario");
        if (jsonFile == null)
        {
            Debug.LogError("Scenario file not found in Resources");
            return;
        }
    }

    void Start()
    {
        storyNodes = new Dictionary<string, StoryNode>();

        // Hide option text
        option[0].gameObject.SetActive(false);
        option[1].gameObject.SetActive(false);

        LoadScenario();
        ShowStory("");
    }

    void LoadScenario()
    {
        // Parse to JsonUtility
        ScenarioWrapper wrapper = JsonUtility.FromJson<ScenarioWrapper>(jsonFile.text);

        if (wrapper == null || wrapper.nodes == null)
        {
            Debug.LogError("Failed to parse scenario JSON");
            return;
        }

        // Save to Dictionary
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
        int index = 0;

        if (!storyNodes.ContainsKey(nodeKey))
        {
            storyTextUI.text = "Node not found: " + nodeKey;
            return;
        }

        StoryNode node = storyNodes[nodeKey];
        StopAllCoroutines();
        StartCoroutine(ShowStoryCoroutine(node));
    }

    IEnumerator ShowStoryCoroutine(StoryNode node)
    {
        storyTextUI.text = "";

        // TODO: change delay time
        // Show story text
        foreach (string line in node.storyText)
        {
            storyTextUI.text = line;
            yield return new WaitForSeconds(0.5f);
        }

        option[0].text = node.option[0];
        option[1].text = node.option[1];

        // Show option text
        option[0].gameObject.SetActive(true);
        option[1].gameObject.SetActive(true);
    }
}