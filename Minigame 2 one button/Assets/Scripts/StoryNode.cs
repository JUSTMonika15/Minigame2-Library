using UnityEngine;
using UnityEngine.UI;

public class StoryNode : MonoBehaviour
{
    public string storyText; // Story text for the current node
    public string choiceA;
    public string choiceB;
    public int nextANode; // Node index to move to if choice A is selected
    public int nextBNode; // Node index to move to if choice B is selected
}