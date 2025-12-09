using UnityEditor;

namespace Dialogue.Editor
{
    //class is used to avoid unity bug where dialogue is creating but then after clicking on any place in the editor root
    //node is dissapearing 
    public class DialogueModificationProcessor  : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                Dialogue dialogue = AssetDatabase.LoadAssetAtPath(importedAsset, typeof(Dialogue)) as Dialogue;
            
                if(dialogue != null)
                    dialogue.CreateRootNode();
            }
        }
    }
}