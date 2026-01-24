using ImGuiNET;
using Project.Editor.Data;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime;
using Runtime.Calc;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Logging;

namespace Project.Editor.UI.FileSystem
{
    internal class ProjectWindow : GuiWindow
    {
        public override string GetName()
        {
            return "Project";
        }

        string[] hiddenFiles = new string[]
        {
            ".meta",
            ".mtl"
        };
        Texture folderTexture;
        public ProjectWindow()
        {
            folderTexture = Texture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/folder.png"));
        }

        string selectedFolder = "";
        string selectedFile = "";

        string browsePath = "assets";
        public override void Render()
        {
            string currentPath = Path.Combine(EditorUtils.projectPath, browsePath);
            DrawBackButton(currentPath);

            // Get all files we need to draw
            string[] directories;
            string[] files;
            try
            {
                directories = Directory.GetDirectories(Path.Combine(EditorUtils.projectPath, browsePath));
                files = Directory.GetFiles(Path.Combine(EditorUtils.projectPath, browsePath));
            }
            catch (Exception e)
            {
                Debug.Error("Could not open directory: " + e.Message);
                return;
            }



            // Calculate the columns count
            float buttonWidth = 100;
            float windowWidth = ImGui.GetContentRegionAvail().X;
            ImGui.Columns((int)Math.Max(1, windowWidth / buttonWidth), "?", false);

            // Draw all directies
            foreach (string directory in directories)
            {
                // Draw the image
                string folderName = Path.GetFileName(directory);


                Vector2 uv = default(Vector2);
                Vector2 uv2 = new Vector2(1f, 1f);

                MetaData metaData = MetaData.GetAssetMeta(Game.GetAssetDatabase().GetAsset(directory));

                Vector4 color = metaData.GetVector4("color", new Vector4(1, 1, 1, 1));
                ImGui.Image(folderTexture.Handle, new Vector2(100, 100).ToNumerics(), uv.ToNumerics(), uv2.ToNumerics(), color.ToNumerics());

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left) && selectedFolder == directory)
                {
                    browsePath = Path.GetRelativePath(EditorUtils.projectPath, directory);
                    Debug.Log("Opening folder: " + browsePath);
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    selectedFolder = directory;

                    FolderAssetInspection inspection = new FolderAssetInspection();
                    inspection.SetAsset(Game.GetAssetDatabase().GetAsset(directory));
                    InspectorWindow.GetActive().SetInspection(inspection);
                }

                // Draw the file name
                ImGui.Text(folderName);
                ImGui.NextColumn();
            }

            // Draw all files
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                if (hiddenFiles.Contains(Path.GetExtension(fileName)))
                    continue;

                AssetManager? assetManager = AssetManager.GetAssetManager(Asset.FromSystemPath(Game.GetAssetDatabase(), file));

                if (assetManager == null)
                {
                    assetManager = new DefaultAssetManager();
                }

                Vector4 borderColor = Vector4.Zero;
                if (selectedFile == file)
                {
                    borderColor = new Vector4(0, 0, 1, 1);
                }
                ImGui.Image(assetManager.GetIcon().Handle, new Vector2(100, 100).ToNumerics(), default(Vector2).ToNumerics(), Vector2.One.ToNumerics(), Vector4.One.ToNumerics(), borderColor.ToNumerics());

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    if (assetManager.GetInspection() is AssetInspection assetInspection)
                    {
                        assetInspection.SetAsset(assetManager.GetAsset());
                    }

                    InspectorWindow.GetActive().SetInspection(assetManager.GetInspection());
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left) && selectedFile == file)
                {
                    assetManager.OnOpen();
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    selectedFile = file;
                }

                ImGui.Text(fileName);
                ImGui.NextColumn();
            }

            DrawRightClickMenu(currentPath);

            ImGui.Columns(1); // Reset
        }

        public void DrawBackButton(string currentPath)
        {
            if (!ImGui.Button("..."))
                return;

            try
            {
                string fullCurrent = Path.GetFullPath(currentPath);
                string fullRoot = Path.GetFullPath(EditorUtils.projectPath);

                DirectoryInfo? parent = Directory.GetParent(fullCurrent);
                if (parent == null)
                    return;

                string fullParent = parent.FullName;

                if (!fullParent.StartsWith(fullRoot + Path.DirectorySeparatorChar))
                    return;

                browsePath = Path.GetRelativePath(fullRoot, fullParent);
                if (browsePath == ".")
                    browsePath = "";

                selectedFolder = "";
                selectedFile = "";
            }
            catch
            {

            }
        }


        public void DrawRightClickMenu(string currentPath)
        {
            if (ImGui.BeginPopupContextWindow($"FolderContext_", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverItems))
            {
                if (ImGui.BeginMenu("Create"))
                {
                    AssetCreator.GUI(Asset.FromSystemPath(Game.GetAssetDatabase(), currentPath));
                    // Action
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Open Folder in Explorer"))
                {
                    Debug.Log($"Opening file explorer at {currentPath}");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = currentPath,
                        UseShellExecute = true
                    });
                }

                ImGui.EndPopup();
            }
        }
    }
}
