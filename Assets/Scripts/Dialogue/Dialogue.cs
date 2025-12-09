using System;
using System.Collections.Generic;
using Core;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();

        [NonSerialized] Vector2 newNodeOffset = new Vector2(250, 0);
        [NonSerialized] Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        //to display next button in game
        
        public void Awake()
        {
            OnValidate();
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        //old code for one root node
       /* public DialogueNode GetRootNode()
        {
            return nodes[0];
        }*/
       
       public IEnumerable<DialogueNode> GetRootNodes()
       {
           // Return all nodes without parents as potential root nodes
           foreach (var node in nodes)
           {
               bool hasParent = false;
               foreach (var otherNode in nodes)
               {
                   if (otherNode.GetChildren().Contains(node.name))
                   {
                       hasParent = true;
                       break;
                   }
               }
               if (!hasParent)
               {
                   yield return node;
               }
           }
       }
       
       //return node without condition if no conditions in other rot nodes are specified
       
      public DialogueNode GetFirstValidRootNode(IEnumerable<IPredicateEvaluator> evaluators)
      {
          DialogueNode fallbackNode = null;

          foreach (var rootNode in GetRootNodes())
          {
              // Check if the node has no condition
              if (rootNode.CheckCondition(evaluators))
              {
                  return rootNode; // Return the first valid node with a condition
              }

              // If no conditions are attached, consider it as a fallback
              if (rootNode.CheckCondition(new List<IPredicateEvaluator>())) 
              {
                  fallbackNode = rootNode;
              }
          }

          // Return the fallback node if no valid node with conditions is found
          return fallbackNode;
      }
      
        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }


        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            //if deleted node has children reconnect it to new parent
            TransferChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerIsSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void TransferChildren(DialogueNode deletedNode)
        {
            DialogueNode newParent = GetNewParent(deletedNode);
            if (newParent != null)
            {
                foreach (DialogueNode node in nodes)
                {
                    node.RemoveChild(deletedNode.name);
                }

                newParent.GetChildren().AddRange(deletedNode.GetChildren());
            }
        }

        private DialogueNode GetNewParent(DialogueNode deletedNode)
        {
            foreach (DialogueNode node in nodes)
            {
                if (node.GetChildren().Contains(deletedNode.name))
                {
                    return node;
                }
            }

            return null;
        }

        //used as workaround because of unity bug. not exist on original course. work in combination with 
        //DialogueModificationProcessor.cs
        public void CreateRootNode()
        {
            if (nodes.Count == 0)
            {
                //old code for one root node
                //nodes.Add(ScriptableObject.CreateInstance<DialogueNode>());
                
                DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
                newNode.name = Guid.NewGuid().ToString();
                nodes.Add(newNode);
                OnValidate();
            }
        }
#endif
    }
}