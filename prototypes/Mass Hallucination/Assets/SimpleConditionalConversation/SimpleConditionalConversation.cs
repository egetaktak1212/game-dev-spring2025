using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConditionalConversation 
{

	/*
	 * I need to make a getDialogueOptions function that just returns a list of the options
	 * Then, I need a pickDialogueOption function that gets what the excel sheet says should happen for that option
	 * And then that'll take me to a new questState where it'll automatically get line.
	 * 
	 * 
	 */



	public Dictionary<string, Dictionary<string, object>> gameStates;




	Hashtable lines;

    public void DebugPrintAllCharacterStates()
    {
        Debug.Log("=== DEBUG: Character GameStates ===");
        foreach (var character in gameStates.Keys)
        {
            Dictionary<string, object> characterState = gameStates[character] as Dictionary<string, object>;
            if (characterState == null)
            {
                Debug.LogWarning($"[WARN] No state dictionary for character: {character}");
                continue;
            }

            Debug.Log($"--- {character}'s Blackboard ---");
            foreach (var kvp in characterState)
            {
                Debug.Log($"[{character}] {kvp.Key} = {kvp.Value}");
            }
        }
        Debug.Log("=== END ===");
    }




    public SimpleConditionalConversation(string dataPath)
	{
		this.gameStates = new Dictionary<string, Dictionary<string, object>>();
        this.gameStates["player"] = new Dictionary<string, object>();
        List<Dictionary<string, object>> data = CSVReader.Read(dataPath);
		this.loadLines(data);
	}
	
	public SimpleConditionalConversation(List<Dictionary<string, object>> data)
	{
        this.gameStates = new Dictionary<string, Dictionary<string, object>>();
		this.gameStates["player"] = new Dictionary<string, object>();
        this.loadLines(data);
	}
	
	
	// Loads data from the data structure that CSVReader creates when it loads
	// a CSV file.
	public void loadLines(List<Dictionary<string, object>> data) 
	{
		this.lines = new Hashtable();
	
		for (var i = 0; i < data.Count; i++) {
			if (!lines.ContainsKey((string)data[i]["questState"])) {
				lines.Add((string)data[i]["questState"], new Dictionary<string, List<SCCLine>>());
			}
			Dictionary<string, List<SCCLine>> questStateLines = (Dictionary<string, List<SCCLine>>)lines[(string)data[i]["questState"]];
			if (!questStateLines.ContainsKey((string)data[i]["character"])) {
				questStateLines[(string)data[i]["character"]] = new List<SCCLine>();
			}
			if (!gameStates.ContainsKey((string)data[i]["character"])) { 
				Dictionary<string,object> characterState = new Dictionary<string, object>();
				characterState["questState"] = "Q1T1";
				gameStates[(string)data[i]["character"]] = characterState;
			}
			List<SCCLine> characterLines = questStateLines[(string)data[i]["character"]];
			SCCLine line = new SCCLine();
			line.questState = (string)data[i]["questState"];
			line.character = (string)data[i]["character"];
			line.condition1Left = (string)data[i]["condition1Left"];
			line.condition1Comp = (string)data[i]["condition1Comp"];
			line.condition1Right = data[i]["condition1Right"];
			line.condition2Left = (string)data[i]["condition2Left"];
			line.condition2Comp = (string)data[i]["condition2Comp"];
			line.condition2Right = data[i]["condition2Right"];
			line.effectLeft = (string)data[i]["effectLeft"];
			line.effectOp = (string)data[i]["effectOp"];
			line.effectRight = data[i]["effectRight"];
			line.line1 = (string)data[i]["line1"];
			line.line2 = (string)data[i]["line2"];
			line.line3 = (string)data[i]["line3"];
			line.choice1 = (string)data[i]["choice1"];
            line.choice2 = (string)data[i]["choice2"];
			line.choice1EffectLeft = (string)data[i]["choice1EffectLeft"];
			line.choice1EffectRight = (string)data[i]["choice1EffectRight"];
			line.choice1EffectOp = (string)data[i]["choice1EffectOp"];
            line.choice2EffectLeft = (string)data[i]["choice2EffectLeft"];
            line.choice2EffectRight = (string)data[i]["choice2EffectRight"];
            line.choice2EffectOp = (string)data[i]["choice2EffectOp"];
            characterLines.Add(line);
		}
	}
	
	/*
	 * General game programming entry point for getting dialogue from the Google
	 * Sheet-based system. Dialogue retrieved is based on the conditions set in the
	 * sheet, the current quest state (as stored by the GameManager), and number of
	 * times the character was interacted wtih in the current quest state.
	 * @param  {string} name            The name of the character character
	 * @return {SCCLine}                Context-specific dialogue.
	*/
	public SCCLine getSCCLine(string name) {

        string questState = (string)getGameStateValue(name, "questState");
        if (!this.lines.ContainsKey(questState))
        {
            return null;
        }
        Dictionary<string, List<SCCLine>> questLines = (Dictionary<string, List<SCCLine>>)this.lines[questState];

        if (!questLines.ContainsKey(name)) {
			return null;
		} 
		List <SCCLine> lines = questLines[name];
	
		foreach (SCCLine line in lines) {
			//Check both conditions until default is reached. Return the first one
			//Check condition1
			bool condition1 = checkCondition(name, line.condition1Left, line.condition1Comp, line.condition1Right);
			//Debug.Log(condition1);
			bool condition2 = checkCondition(name, line.condition2Left, line.condition2Comp, line.condition2Right);
			//Debug.Log(condition2);
			if (condition1 && condition2) {
				setGameStateValue(name, line.effectLeft, line.effectOp, line.effectRight);
				return line;
			}
		}
		return null;
	}

	public void makeChoice(int choice, SCCLine line) {
		if (choice == 1)
		{
			setGameStateValue(line.character, line.choice1EffectLeft, line.choice1EffectOp, line.choice1EffectRight);
		}
		else if (choice == 2)
		{
            setGameStateValue(line.character, line.choice2EffectLeft, line.choice2EffectOp, line.choice2EffectRight);
        }
	}

	
	/**
	 * This checks a condition on dialogue for its truth value.
	 * @param  {String} left  The left side of the conditional.
	 * @param  {String} op    The comparison operator to use.
	 * @param  {object} right The right side of the conditional.
	 * @return {bool}       The truth value of the conditional.
	 */
	public bool checkCondition(string name, string left, string op, object right) 
	{
		//Debug.Log("CHECKING: " + left + " " + op + " " + (string)right);
		object leftValue = getGameStateValue(name, left);
	
		//If there's nothing there, and it is checking if it doesn't equal something, return true
		if (op == "not equals" && leftValue == null) {
			return true;
		} else if (leftValue == null && op != "") {
			return false;
		}
	
		if (leftValue is int) {
			int leftInt = (int)leftValue;
			//This means we can this as an int
			if (op == "greater") {
				return leftInt > (int)right;
			} else if (op == "less") {
				return leftInt  < (int)right;
			} else if (op == "equals") {
				return leftInt == (int)right;
			} else if (op == "not equals") {
				return leftInt != (int)right;
			}
		} else if (leftValue is float) {
			float leftFloat = (float)leftValue;
			//This means we can this as a float
			float rightFloat;
			float.TryParse(right.ToString(), out rightFloat);
			if (op == "greater") {
				//return leftFloat > (float)right;
				return leftFloat > rightFloat;
			} else if (op == "less") {
				return leftFloat < (float)right;
			} else if (op == "equals") {
				//round to int. Floats can't be equal.
				int leftInt = (int)leftFloat;
				int rightInt = (int)right;
				return leftInt == rightInt;
			} else if (op == "not equals") {
				//round to int. Floats can't be equal.
				int leftInt = (int)leftFloat;
				int rightInt = (int)right;
				return leftInt != rightInt;
			}
		} else if (leftValue is bool) {
			bool leftBool = (bool)leftValue;
			right = right.ToString().ToLower();
			right = bool.Parse((string)right);
			//This means we can this as a bool
			if (op == "equals") {
				return leftBool == (bool)right;
			} else if (op == "not equals") {
				return leftBool != (bool)right;
			}
		} else {
			//By default, treat it as a string
			if (op == "equals") {
				return (string)leftValue == (string)right;
			} else if (op == "not equals") {
				if (leftValue == null || (string)leftValue != (string)right) {
					return true;
				} else {
					return false;
				}
			}
		}
		//If we get down here, that means there was nothing there I think
		return true;
	}

	/*
	 * Retrieves the value associated with the given in either the queststate/
	 * dialogue structure or in the blackboard memory (A.K.A. this.gameState).
	 * Values will be converted into numbers when possible.
	 * @param  {String} id The id of the value to look up.
	 * @return {Object}    The data associated with the id.
	 */
	public object getGameStateValue(string name, string id) 
	{

		if (id.Contains("."))
		{
			string[] parts = id.Split('.');
			string target = parts[0];
			string key = parts[1];

			if (this.gameStates.ContainsKey(target) && this.gameStates[target].ContainsKey(key))
			{
				return this.gameStates[target][key];
			}
		}

            if (this.gameStates[name].ContainsKey(id)) {
			return this.gameStates[name][id];
		}
		return null;
	}

	/*
	 * Sets id/value pairs in game state for use by conditionals (and perhaps
	 * more). It looks for existing entries to update. If no entry is found, an
	 * entry in the GameManager's gameState is created. The value is modifed by
	 * operation parameter (e.g. add, equals/set, etc.). The default for new ids
	 * is 0. Values will be converted into numbers when possible.
	 * @param {String} id    Id of the id/value pair.
	 * @param {String} op    The operation to apply when setting the value.
	 * @param {object} right The value side of the value to set.
	 */
	public void setGameStateValue(string name, string id, string op, object right) 
	{
		var character = this.gameStates[name];
        if (id.Contains("."))
		{
            string[] parts = id.Split('.');
            id = parts[1];
            character = this.gameStates[parts[0]];
        }
		
		if (op == "add") {
			if (!character.ContainsKey(id)) {
                character.Add(id, 0);
			}
            character[id] = (int)character[id] + (int)right;
		} else if (op == "subtract") {
			if (!character.ContainsKey(id)) {
				character.Add(id, 0);
			}
            character[id] = (int)character[id] - (int)right;
		} else if (op == "equals" || op == "set") {
			bool rightBool;
			float rightFloat;
			if (float.TryParse(right.ToString(), out rightFloat)) {
			//if (right is float) {
				if (!character.ContainsKey(id)) {
					character.Add(id, right);
				} else {
					character[id] = right;
				}
			} else if (bool.TryParse((string)right, out rightBool)) {
				if (!character.ContainsKey(id)) {
					character.Add(id, rightBool);
				} else {
					character[id] = rightBool;
				}
			} else {
				if (!character.ContainsKey(id)) {
					character.Add(id, (string)right);
				} else {
					character[id] = (string)right;
				}
			}
		}
	}
}
