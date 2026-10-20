using UnityEngine;

public class SceneStartManager : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueSequence openingDialogue;

    void Start()
    {
        dialogueController.StartDialogue(openingDialogue);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
