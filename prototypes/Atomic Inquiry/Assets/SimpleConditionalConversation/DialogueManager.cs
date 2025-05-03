using System.Collections;
using UnityEngine;
using System;
using StarterAssets;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
	public static SimpleConditionalConversation scc;

	public static Action<string, string> DialogueAction;

	[SerializeField] DialogueUIManager uiManager;

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask npcLayer;
	[SerializeField] FirstPersonController fpsController;

	bool cantLookAtNPC = false;
	bool lookingAtNPC = false;
	string npcLookAtName = "";

    Coroutine dialogueScene = null;

	bool keepCheckingEveryoneAgrees = true;


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

        if (Input.GetKeyDown(KeyCode.P))
        {
            scc.DebugPrintAllCharacterStates();
        }

        if (Input.GetKeyDown(KeyCode.E) && lookingAtNPC && dialogueScene == null)
        {
			dialogueScene = StartCoroutine(DialogueScene(npcLookAtName));

        }

		if (!cantLookAtNPC) {
			lookForNPC();
		}


		if (DialogueManager.scc != null)
		{
			if (keepCheckingEveryoneAgrees)
			{
				if (DialogueManager.scc.checkIfEveryoneAgrees())
				{
					keepCheckingEveryoneAgrees = false;
					scc.setGameStateValue("Initiate Mission", "everyoneAgrees", "equals", "true");

				}
			}

			var startMission = DialogueManager.scc.getGameStateValue("Initiate Mission", "startMission");
			
            if (startMission != null && (bool) startMission) {
                var Argo2State = DialogueManager.scc.getGameStateValue("Argonaut 2", "questState");
                var Argo3State = DialogueManager.scc.getGameStateValue("Argonaut 3", "questState");

                string[] parts2 = ((string)Argo2State).Split('T');
                string questArgo2 = parts2[0];
                int questNumber2 = Convert.ToInt32(questArgo2.Substring(1));


                string[] parts3 = ((string)Argo3State).Split('T');
                string questArgo3 = parts3[0];
                int questNumber3 = Convert.ToInt32(questArgo3.Substring(1));

				if (questNumber2 == 7 && questNumber3 == 7)
				{
                    SceneManager.LoadScene("LetItHappenEnding");
                }
				else if (questNumber2 == 8 && questNumber3 == 8)
				{
                    SceneManager.LoadScene("BigGunEnding");
                }
				else if (questNumber2 == 9 && questNumber3 == 9) {
                    SceneManager.LoadScene("BigMagnetEnding");
                }

            }


		}


	}
    private IEnumerator DialogueScene(string name)
    {
		uiManager.HideNPCInformation();
		cantLookAtNPC = true;
		bool end = false;
		fpsController.lockMovementAndCamera(true);



        yield return StartCoroutine(uiManager.ShowDialogueUI(name));

        while (!end)
		{
			SCCLine dialogueResult = DialogueManager.scc.getSCCLine(name);
			string line = dialogueResult.renderLine();

			//start a coroutine in another script but make this numerator wait until this one is finished.
			yield return StartCoroutine(uiManager.MakeDialogueText(line));

			bool choiceOneExists = !string.IsNullOrEmpty(dialogueResult.choice1);
            bool choiceTwoExists = !string.IsNullOrEmpty(dialogueResult.choice2);
			bool choiceThreeExists = !string.IsNullOrEmpty(dialogueResult.choice3);
			bool choiceFourExists = !string.IsNullOrEmpty(dialogueResult.choice4);

			if (choiceOneExists || choiceTwoExists || choiceThreeExists || choiceFourExists)
			{

				yield return StartCoroutine(uiManager.ShowChoicesUI(choiceOneExists, choiceTwoExists, choiceThreeExists, choiceFourExists));

            }
			if (choiceOneExists)
			{

                yield return StartCoroutine(uiManager.MakeChoice1Text(dialogueResult.choice1));

            }
			if (choiceTwoExists)
			{
				yield return StartCoroutine(uiManager.MakeChoice2Text(dialogueResult.choice2));

			}
            if (choiceThreeExists)
            {
                yield return StartCoroutine(uiManager.MakeChoice3Text(dialogueResult.choice3));

            }
            if (choiceFourExists)
            {
                yield return StartCoroutine(uiManager.MakeChoice4Text(dialogueResult.choice4));

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
                else if (Input.GetKeyDown(KeyCode.UpArrow) && choiceThreeExists)
                {
                    DialogueManager.scc.makeChoice(3, dialogueResult);
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && choiceFourExists)
                {
                    DialogueManager.scc.makeChoice(4, dialogueResult);
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.E))
				{
					end = true;
					break;
				}
				yield return null;
			}
			Coroutine hideChoices = StartCoroutine(uiManager.ShowChoicesUI(false, false, false, false));
			Coroutine hideChoice1Text = StartCoroutine(uiManager.fadeChoice1Text(true));
            Coroutine hideChoice2Text = StartCoroutine(uiManager.fadeChoice2Text(true));
            Coroutine hideChoice3Text = StartCoroutine(uiManager.fadeChoice3Text(true));
            Coroutine hideChoice4Text = StartCoroutine(uiManager.fadeChoice4Text(true));
            yield return hideChoice2Text;
            yield return hideChoice1Text;
            yield return hideChoices;
			yield return hideChoice3Text;
			yield return hideChoice4Text;
			
			
        }
		yield return StartCoroutine(uiManager.HideDialogueUI());
		cantLookAtNPC = false;
        fpsController.lockMovementAndCamera(false);
		dialogueScene = null;

    }

	void lookForNPC() {
        
		RaycastHit hit;


        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, npcLayer))
		{
			if (hit.collider.CompareTag("NPC"))
			{
				string npcName = hit.collider.gameObject.GetComponent<NPCInformation>().npcName;

				uiManager.ShowNPCInformation(npcName);
				npcLookAtName = npcName;
				lookingAtNPC = true;
			}
			else
			{
				uiManager.HideNPCInformation();
				lookingAtNPC=false;
                npcLookAtName = "";
            }
		}
		else {
			uiManager.HideNPCInformation();
			lookingAtNPC=false;
            npcLookAtName = "";
        }
    }



}
