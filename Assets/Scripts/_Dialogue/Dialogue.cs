using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Whenever we create a class like this and want it to show up...
// in the inspector so that we can edit it. We need to mark it...
// as Serializable. So we add this attribute.
[System.Serializable]

// Dialogue class used as an object that we can pass into the... 
// DialogueManager whenever we want to start a new dialogue.
// Host all information that we need about a single dialogue.s
public class Dialogue {

    // Name of NPC that we're talking of.
    public string name;
    public Image charImage;

    // Minimum amount of lines the text area will use and the maximum
    [TextArea(3, 10)]
    // Sentences that we will load into our queue.
    public string[] sentences;

}
