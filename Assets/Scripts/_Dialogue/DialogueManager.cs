using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Text nameText;
    public Text dialogueText;
    public Image charPortrait;

    public Animator animator;

    // Keep track of all sentences. As the user reads through the dialogue we'll just load new sentences from the end of the queue.
    private Queue<string> sentences;
	
    // Initialization
    void Start()
    {
        sentences = new Queue<string>();
    }
	

	public void StartDialogue (Dialogue dialogue)
    {
        //Debug.Log("Starting Conversation with " + dialogue.name);

        // Sets paramater IsOpen to True when starting dialogue.
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;
        charPortrait = dialogue.charImage;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
	}

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        //dialogueText.text = sentence;
        //Debug.Log(sentence);
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        //charPortrait.enabled = false;
        //Debug.Log("End of Conversation.");
    }
}
