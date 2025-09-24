using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryNode
{
    public string key;
    public string[] storyText; // Story text for the current node
    public string[] option;
}