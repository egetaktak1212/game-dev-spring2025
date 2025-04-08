using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
	public static SimpleConditionalConversation scc;

	public static Action<string, string> DialogueAction;

	[SerializeField] DialogueUIManager uiManager;

	// NOTE: When you do not use the google sheet option, it is expecting the file
	// to be named "data.csv" and for it to be in the Resources folder in Assets.
	public bool useGoogleSheet = false;
	public string googleSheetDocID = "";

	// Start is called before the first frame update
	void Start()
	{
		if (useGoogleSheet) {
			// This will start the asyncronous calls to Google Sheets, and eventually
			// it will give a value to scc, and also call LoadInitialHistory().
			GoogleSheetSimpleConditionalConversation gs_ssc = gameObject.AddComponent<GoogleSheetSimpleConditionalConversation>();
			gs_ssc.googleSheetDocID = googleSheetDocID;
		} else {
			scc = new SimpleConditionalConversation("data");
			LoadInitialSCCState();
		}
	}
	
	public static void LoadInitialSCCState()
	{
		// Example of setting the initial state:
		// NOTE: If you are putting a number or bool, make sure not to store them
		// as strings.
		//
		// scc.setGameStateValue("playerWearing", "equals", "Green shirt");
	}
	
	// Update is called once per frame
	void Update()
	{
		// An example of getting a line of dialogue.
		if (Input.GetKeyDown(KeyCode.Space)) {
			SCCLine line = DialogueManager.scc.getSCCLine("Mike");
            Debug.Log("Mike says: " + line.renderLine());
			DialogueAction?.Invoke("Mike", line.renderLine());
		}

        if (Input.GetKeyDown(KeyCode.P))
        {
            scc.DebugPrintAllCharacterStates();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
			StartCoroutine(DialogueScene("Emma"));

        }


        // An example of modifying the state outside of the DialogueManager (e.g. you could put this in some
        // OnTriggerEnter or something)
        if (Input.GetKeyDown(KeyCode.G)) {
			scc.setGameStateValue("Emma", "playerWearing", "equals", "Green shirt");
			Debug.Log("Emma puts on a green shirt.");
		}
	}
    private IEnumerator DialogueScene(string name)
    {
		bool end = false;

        yield return StartCoroutine(uiManager.ShowDialogueUI(name));

        while (!end)
		{
			SCCLine dialogueResult = DialogueManager.scc.getSCCLine(name);
			string line = dialogueResult.renderLine();

			//start a coroutine in another script but make this numerator wait until this one is finished.
			yield return StartCoroutine(uiManager.MakeDialogueText(line));

			Debug.Log("Emma says: " + line);
			bool choiceOneExists = !string.IsNullOrEmpty(dialogueResult.choice1);
            bool choiceTwoExists = !string.IsNullOrEmpty(dialogueResult.choice2);

			if (choiceOneExists || choiceTwoExists)
			{
				yield return StartCoroutine(uiManager.ShowChoicesUI(choiceOneExists, choiceTwoExists));
			}
			if (choiceOneExists)
			{
				yield return StartCoroutine(uiManager.MakeChoice1Text(dialogueResult.choice1));
				Debug.Log("Choice 1 (Left): " + dialogueResult.choice1);
			}
			if (choiceTwoExists)
			{
				yield return StartCoroutine(uiManager.MakeChoice2Text(dialogueResult.choice2));
				Debug.Log("Choice 2 (Right): " + dialogueResult.choice2);
			}
			while (true)
			{
				if (Input.GetKeyDown(KeyCode.LeftArrow) && choiceOneExists)
				{
					DialogueManager.scc.makeChoice(1, dialogueResult);
					break;
				}
				else if (Input.GetKeyDown(KeyCode.RightArrow) && choiceTwoExists)
				{
					DialogueManager.scc.makeChoice(2, dialogueResult);
					break;
				}
				else if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					end = true;
					break;
				}
				yield return null;
			}
			Coroutine hideChoices = StartCoroutine(uiManager.ShowChoicesUI(false, false));
			Coroutine hideChoice1Text = StartCoroutine(uiManager.fadeChoice1Text(true));
            Coroutine hideChoice2Text = StartCoroutine(uiManager.fadeChoice2Text(true));
			yield return hideChoices;
			yield return hideChoice1Text;
			yield return hideChoice2Text;
        }
		yield return StartCoroutine(uiManager.HideDialogueUI());
    }



}
