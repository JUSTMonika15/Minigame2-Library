using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public Text storyTextUI;
    public string selectedChoice;

    public StoryNode[] storyNodes;
    private int currentNode = 0;

    void Start()
    {
        // ShowStory(currentNode);
    }

    // Shows the story of the current story node where the player is at
    void ShowStory(int nodeIndex)
    {
        StoryNode node = storyNodes[nodeIndex];
        storyTextUI.text = node.storyText;

        // TODO: add coroutine animate the story text ui
    }

    void Choose(int nextNode)
    {
        if (nextNode == -1) // end
        {
            storyTextUI.text = "THE END";
            // TODO: loop to story node 0 (first scene)
            return;
        }

        currentNode = nextNode;
        ShowStory(currentNode);
    }
}