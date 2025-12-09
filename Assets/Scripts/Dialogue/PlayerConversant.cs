using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Dialogue;
using Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        bool isChoosing = false;
        AIConversant currentConversant = null;
        [SerializeField] private string playerName;
        private Mover mover;
        
        public event Action onConversationUpdated;

        public void Awake()
        {
            mover = GetComponent<Mover>();
        }
        
        public void MoveToAndStartDialogue(AIConversant conversant, Dialogue dialogue)
        {
            if (mover == null)
            {
                Debug.LogError("Mover component not found on PlayerConversant.");
                return;
            }
            Vector3 directionToConversant = conversant.transform.position - transform.position;
            Vector3 destination = conversant.transform.position - (directionToConversant.normalized * 2f);
            StartCoroutine(MoveToAndStartDialogueCoroutine(destination, conversant, dialogue));
        }



        private IEnumerator MoveToAndStartDialogueCoroutine(Vector3 destination, AIConversant conversant, Dialogue dialogue)
        {
            mover.MoveTo(destination, 2f);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, destination) < 0.2f);
            mover.CancelAction(); 
            StartDialogue(conversant, dialogue); // Start the dialogue
        }
        
        //old code for one root node
        /*public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            if(currentDialogue!=null) QuitDialogue();//to fix bug where speaking with one conversant we could click on another 
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            // Hide the question mark when dialogue starts
            TriggerEnterAction();
            onConversationUpdated();
        }*/
        
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            if (currentDialogue != null) QuitDialogue(); // Quit previous dialogue if active

            currentConversant = newConversant;
            currentDialogue = newDialogue;

            // Get the first valid root node based on conditions
            currentNode = currentDialogue.GetFirstValidRootNode(GetEvaluators());

            if (currentNode == null)
            {
                Debug.LogWarning("No valid root node found for the dialogue.");
                QuitDialogue();
                return;
            }

            TriggerEnterAction();
            onConversationUpdated();
        }
        

        //TODO remove duplicate
        public bool IsActive()
        {
            return currentDialogue != null;
        }
        
        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }
        
        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }
    
        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            if (HasNext())
            {
                int randomIndex = Random.Range(0, children.Count());
                TriggerExitAction();
                currentNode = children[randomIndex];
                TriggerEnterAction();
            }
            else
            {
                QuitDialogue();
                return;
            }
            onConversationUpdated();
        }

        //TODO has next works only for npc nodes. add player
        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }
        
        //hide close button if it is not end of the dialogue
        //public bool HasNext() => currentDialogue != null && currentDialogue.GetAllChildren(currentNode).Any();
        
        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }
        
        public void QuitDialogue()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();
        }
        
        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }
        
        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                foreach (var action in currentNode.GetOnExitActions()) // Iterate through each action
                {
                    TriggerAction(action);
                }
            }
        }
        
        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetAIConversantName();
            }
        }
    }
}