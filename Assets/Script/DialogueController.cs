using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;


public class DialogueController : MonoBehaviour
{
    
    [Header("UI")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] public GameObject dialoguePanel;

    [Header("会話データ")]
private DialogueData[] dialogues;
    [Header("会話終了時イベント")]
    [SerializeField] private UnityEvent onDialogueEnd;

    private int currentIndex = 0;
    private bool isTalking = false;
    public bool isStart=false;
private float nextInputTime;
[SerializeField] private float inputInterval = 0.2f;
    private void Start()
    {
        
    }

    private void Update()
{
    if (!isTalking) return;

    if (Time.time < nextInputTime)
        return;

    bool pressed =Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
        

    if (pressed)
    {
        nextInputTime = Time.time + inputInterval;
        NextDialogue();
    }
}

    public void StartDialogue(DialogueSequence sequence)
{
    dialogues = sequence.dialogues;

    currentIndex = 0;
    isTalking = true;
    dialoguePanel.SetActive(true);

    ShowDialogue();
}

    private void NextDialogue()
    {
        Debug.Log("NextDialogue");
        currentIndex++;

        if (currentIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        dialogueText.text = dialogues[currentIndex].text;
        characterImage.sprite = dialogues[currentIndex].image;
    }

    private void EndDialogue()
    {
        //draw.isCanDraw = true; // Re-enable drawing after dialogue
        isTalking = false;
        dialoguePanel.SetActive(false);

        onDialogueEnd?.Invoke();
    }
}