using System;
using System.IO;

namespace SuperUnityBuild.BuildActions
{
    public class FileOperation
    {
        private void Copy(string inputPath, string outputPath, bool overwrite = true)
        {
            CopyOrMove(true, inputPath, outputPath, overwrite);
        }

        public static void Move(string inputPath, string outputPath, bool overwrite = true)
        {
            CopyOrMove(false, inputPath, outputPath, overwrite);
        }

        private static void CopyOrMove(bool isCopy, string inputPath, string outputPath, bool overwrite = true)
        {
            Action<string, string> fileOperation = FileUtility.GetCopyOrMoveAction(isCopy);
            bool success = ValidatePath(inputPath, FileUtility.PathType.Input, true, out string errorString);
            if (success)
                success = ValidatePath(outputPath, FileUtility.PathType.Output, false, out errorString);

            if (success)
            {
                // Make sure that all parent directories in path are already created.
                string parentPath = Path.GetDirectoryName(outputPath);

                if (!Directory.Exists(parentPath))
                    _ = Directory.CreateDirectory(parentPath);

                if (overwrite && Directory.Exists(outputPath))
                    success = FileUtility.Delete(outputPath, $"Could not overwrite existing folder \"{outputPath}\".", out errorString);

                if (success)
                    fileOperation(inputPath, outputPath);
            }

            FileUtility.OperationComplete(success, $"Folder {(isCopy ? "Copy" : "Move")} Failed.", errorString);
        }

        public static void Delete(string inputPath)
        {
            bool success = ValidatePath(inputPath, FileUtility.PathType.Input, true, out string errorString);
            if (success)
                success = FileUtility.Delete(inputPath, $"Could not delete folder \"{inputPath}\".", out errorString);

            FileUtility.OperationComplete(success, "Folder Delete Failed.", errorString);
        }

        private static bool ValidatePath(string path, FileUtility.PathType pathType, bool checkForExistence, out string errorString)
        {
            return FileUtility.ValidatePath(path, pathType, checkForExistence, false, out errorString);
        }
    }
}
