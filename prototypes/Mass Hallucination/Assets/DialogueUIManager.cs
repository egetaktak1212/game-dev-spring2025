using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    GameObject dialoguePanel;
    [SerializeField]
    GameObject dialogueText;
    [SerializeField]
    GameObject choicesEmpty;
    [SerializeField]
    GameObject choice1Panel;
    [SerializeField]
    GameObject choice2Panel;
    [SerializeField]
    GameObject choice1Text;
    [SerializeField]
    GameObject choice2Text;
    [SerializeField]
    GameObject speakerText;
    [SerializeField]
    GameObject exit;
    [SerializeField]
    GameObject npcInfoCanvas;
    [SerializeField]
    TextMeshProUGUI npcInfoText;

    float fadeSpeed = 3.0f;

    /*
     * THINGS I HAVE RN
     * The fading in and out works well, setting text and all that
     * 
     * I need to make dialogue manager go through these. I need to set up an input system.
     * 
     * 
     * GUUUUUUUHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH IVE BEEN CODING FOR NEARLY 9 HOURS
     * 
     * 
     * 
     * 
     */

    void Start()
    {
        enableEverything(false);

        npcInfoCanvas.SetActive(false);
        npcInfoText.text = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            StartCoroutine(ShowDialogueUI("Emma"));
        
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(HideDialogueUI());

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(ShowChoicesUI(true, false));

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(ShowChoicesUI(false, true));

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(MakeDialogueText("AAAAA"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(MakeChoice1Text("This is choice 1"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(MakeChoice2Text("This is choice 2"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(MakeChoice1Text("This is different choice 1"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(MakeChoice2Text("This is different choice 2"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(DebugCoroutine());
        }


    }

    public IEnumerator DebugCoroutine() {
        Coroutine debug1 = StartCoroutine(WaitFor(1));
        Debug.Log("A");
        Coroutine debug2 = StartCoroutine(WaitFor(2));
        Debug.Log("B");
        Coroutine debug3 = StartCoroutine(WaitFor(3));
        Debug.Log("C");

        yield return debug1;
        yield return debug2;
        yield return debug3;

        Debug.Log("Debug Coroutine Done");
    }

    public IEnumerator WaitFor(int count) { 
        yield return new WaitForSeconds(count);
    }

    public IEnumerator ShowDialogueUI(string name)
    {
        prepareDialogue();

        setSpeakerName(name);

        Coroutine fadeName = StartCoroutine(fadeSpeakerText(false));
        Coroutine fadePanel = StartCoroutine(fadeDialoguePanel(false));
        Coroutine fadeText = StartCoroutine(fadeSpeakerText(false));
        Coroutine fadeExitButton = StartCoroutine(fadeExit(false));

        Debug.Log("A");
        yield return fadeName;
        Debug.Log("B");
        yield return fadePanel;
        Debug.Log("C");
        yield return fadeText;
        Debug.Log("D");
        yield return fadeExitButton;
        Debug.Log("ShowDialogueUI is finshed");
    }

    public IEnumerator HideDialogueUI()
    {
        

        Coroutine fadeName = StartCoroutine(fadeSpeakerText(true));
        Coroutine fadePanel = StartCoroutine(fadeDialoguePanel(true));
        Coroutine fadeDialogue = StartCoroutine(fadeDialogueText(true));
        Coroutine fadeText = StartCoroutine(fadeSpeakerText(true));
        Coroutine fadeChoice1Textt = StartCoroutine(fadeChoice1Text(true));
        Coroutine fadeChoice1Panell = StartCoroutine(fadeChoice1Panel(true));
        Coroutine fadeChoice2Textt = StartCoroutine(fadeChoice2Text(true));
        Coroutine fadeChoice2Panell = StartCoroutine(fadeChoice2Panel(true));
        Coroutine fadeExitButton = StartCoroutine(fadeExit(true));

        yield return fadePanel;
        yield return fadeText;
        yield return fadeName;
        yield return fadeChoice1Textt;
        yield return fadeChoice1Panell;
        yield return fadeChoice2Textt;
        yield return fadeChoice2Panell;
        yield return fadeExitButton;

        //enableEverything(false);
        Debug.Log("HideDialogueUI is finshed");
    }


    public IEnumerator ShowChoicesUI(bool choice1, bool choice2) {
        Coroutine fadeChoice1;
        Coroutine fadeChoice2;
        
        fadeChoice1 = StartCoroutine(fadeChoice1Panel(!choice1));

        fadeChoice2 = StartCoroutine(fadeChoice2Panel(!choice2));

        if (fadeChoice1 != null)
        {
            yield return fadeChoice1;
        }
        if (fadeChoice2 != null)
        {
            yield return fadeChoice2;
        }
        Debug.Log("ShowChoicesUI is finshed");
    }
    public IEnumerator MakeDialogueText(string text)
    {
        if (!string.IsNullOrEmpty(dialogueText.GetComponent<TextMeshProUGUI>().text))
        {
            yield return StartCoroutine(fadeDialogueText(true));
        }

        dialogueText.GetComponent<TextMeshProUGUI>().text = text;

        StartCoroutine(fadeDialogueText(false));
        Debug.Log("MakeDialogueText is finshed");
    }


    public IEnumerator MakeChoice1Text(string text) {
        if (!string.IsNullOrEmpty(choice1Text.GetComponent<TextMeshProUGUI>().text))
        {
            yield return StartCoroutine(fadeChoice1Text(true));
        }

        choice1Text.GetComponent<TextMeshProUGUI>().text = text;

        StartCoroutine(fadeChoice1Text(false));
        Debug.Log("MakeChoice1Text is finshed");
    }
    public IEnumerator MakeChoice2Text(string text)
    {
        if (!string.IsNullOrEmpty(choice2Text.GetComponent<TextMeshProUGUI>().text))
        {
            yield return StartCoroutine(fadeChoice2Text(true));
        }

        choice2Text.GetComponent<TextMeshProUGUI>().text = text;

        StartCoroutine(fadeChoice2Text(false));
        Debug.Log("MakeChoice2Text is finshed");
    }



    void prepareDialogue() {
        enableEverything(true);
        resetAllText();
        resetALlAlphas();
    }

    void resetALlAlphas() { 
        speakerText.GetComponent<CanvasRenderer>().SetAlpha(0);
        dialoguePanel.GetComponent<CanvasRenderer>().SetAlpha(0);
        dialogueText.GetComponent<CanvasRenderer>().SetAlpha(0);
        choice1Panel.GetComponent<CanvasRenderer>().SetAlpha(0);
        choice2Panel.GetComponent<CanvasRenderer>().SetAlpha(0);
        choice1Text.GetComponent<CanvasRenderer>().SetAlpha(0);
        choice2Text.GetComponent<CanvasRenderer>().SetAlpha(0);
        exit.GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    void enableEverything(bool enable) {
        dialoguePanel.gameObject.SetActive(enable);
        dialogueText.gameObject.SetActive(enable);
        choice1Panel.gameObject.SetActive(enable);
        choice2Panel.gameObject.SetActive(enable);
        choice1Text.gameObject.SetActive(enable);
        choice2Text.gameObject.SetActive(enable);
        speakerText.gameObject.SetActive(enable);
        exit.gameObject.SetActive(enable);
    }

    void resetAllText() {
        choice1Text.GetComponent<TextMeshProUGUI>().text = "";
        choice2Text.GetComponent<TextMeshProUGUI>().text = "";
        dialogueText.GetComponent<TextMeshProUGUI>().text = "";
        speakerText.GetComponent<TextMeshProUGUI>().text = "";
    }


    public void setSpeakerName(string name) {
        speakerText.GetComponent<TextMeshProUGUI>().text = name;
    }
    public IEnumerator fadeExit(bool shouldI)
    {
        if (shouldI)
        {
            while (exit.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = exit.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                exit.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
        else
        {
            while (exit.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = exit.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                exit.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
    }


    public IEnumerator fadeDialoguePanel(bool shouldI) {
        if (shouldI)
        {
            while (dialoguePanel.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = dialoguePanel.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                dialoguePanel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        } else {
            while (dialoguePanel.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = dialoguePanel.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                dialoguePanel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                Debug.Log(dialoguePanel.GetComponent<CanvasRenderer>().GetAlpha());
                yield return null;
            }
        }
    }
    public IEnumerator fadeDialogueText(bool shouldI)
    {
        if (shouldI)
        {
            while (dialogueText.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = dialogueText.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                dialogueText.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
        else
        {
            while (dialogueText.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = dialogueText.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                dialogueText.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
    }

    public IEnumerator fadeSpeakerText(bool shouldI)
    {
        if (shouldI)
        {
            while (speakerText.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = speakerText.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                speakerText.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
        else
        {
            while (speakerText.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = speakerText.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                speakerText.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
    }

    public IEnumerator fadeChoice1Text(bool shouldI) {
        if (shouldI) {
            while (choice1Text.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = choice1Text.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                choice1Text.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        } else {
            while (choice1Text.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = choice1Text.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                choice1Text.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
    }

    public IEnumerator fadeChoice2Text(bool shouldI)
    {
        if (shouldI)
        {
            while (choice2Text.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    Debug.Log("here");
                }
                float debugSpeed = fadeSpeed * Time.deltaTime;
                float debugAlpha = choice2Text.GetComponent<CanvasRenderer>().GetAlpha();
                float newAlpha = choice2Text.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                choice2Text.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                Debug.Log(choice2Text.GetComponent<CanvasRenderer>().GetAlpha());
                yield return null;
            }
        }
        else
        {
            while (choice2Text.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = choice2Text.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                choice2Text.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
                yield return null;
            }
        }
    }



    public IEnumerator fadeChoice1Panel(bool shouldI) {
        if (shouldI)
        {
            while (choice1Panel.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = choice1Panel.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                choice1Panel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);

                yield return null;
            }
        }
        else {
            while (choice1Panel.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = choice1Panel.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                choice1Panel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);

                yield return null;
            }
        }
    }

    public IEnumerator fadeChoice2Panel(bool shouldI)
    {
        if (shouldI)
        {
            while (choice2Panel.GetComponent<CanvasRenderer>().GetAlpha() > 0)
            {
                float newAlpha = choice2Panel.GetComponent<CanvasRenderer>().GetAlpha() - fadeSpeed * Time.deltaTime;
                choice2Panel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);

                yield return null;
            }
        }
        else
        {
            while (choice2Panel.GetComponent<CanvasRenderer>().GetAlpha() < 1)
            {
                float newAlpha = choice2Panel.GetComponent<CanvasRenderer>().GetAlpha() + fadeSpeed * Time.deltaTime;
                choice2Panel.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);

                yield return null;
            }
        }
    }

    public void ShowNPCInformation(string name) { 
        npcInfoCanvas.gameObject.SetActive(true);
        npcInfoText.text = name;
    }

    public void HideNPCInformation()
    {
        npcInfoCanvas.gameObject.SetActive(false);
        npcInfoText.text = "";
    }





}
