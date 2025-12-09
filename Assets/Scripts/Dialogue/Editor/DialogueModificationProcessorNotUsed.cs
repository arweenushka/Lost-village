using System.IO;
using UnityEditor;

namespace Dialogue.Editor
{
    //used to avoid unity bug when all nodes are deleted while dialogue is renamed
    //Fixed in 2021.1.X https://issuetracker.unity3d.com/issues/parent-and-child-nested-scriptable-object-assets-switch-places-when-parent-scriptable-object-asset-is-renamed
   // if useing previous unity version then uncomment code
    public class DialogueModificationProcessorNotUsed : AssetModificationProcessor
    {
       /* private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            var dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as global::Dialogue.Dialogue;
            if (dialogue == null)
            {
                return AssetMoveResult.DidNotMove;
            }
            if (MovingDirectory(sourcePath, destinationPath))
            {
                return AssetMoveResult.DidNotMove;
            }
            dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);
            return AssetMoveResult.DidNotMove;
        }
        private static bool MovingDirectory(string sourcePath, string destinationPath)
        {
            return Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath);
        }*/
    }
}