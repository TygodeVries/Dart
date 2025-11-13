using ImGuiNET;
using Project.Editor.Data;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem
{
    internal class ProjectWindow : GuiWindow
    {
        Texture folderTexture;
        public ProjectWindow()
        {
            folderTexture = Texture.LoadFromPng("assets/textures/icons/folder.png");
        }

         string browsePath = "assets";
        public override void Render()
        {
            // Get all files we need to draw
            string[] directories = Directory.GetDirectories(Path.Combine(Editor.projectPath, browsePath));
            string[] files = Directory.GetFiles(Path.Combine(Editor.projectPath, browsePath));

            // Add a back button
            string currentPath = Path.Combine(Editor.projectPath, browsePath);
           
            ImGui.Begin("Project");

            if (ImGui.Button("..."))
            {
                string? parent = Directory.GetParent(currentPath)?.FullName;

                if (parent != null && parent.StartsWith(Editor.projectPath))
                {
                    browsePath = Path.GetRelativePath(Editor.projectPath, parent);
                }
            }


            // Calculate the columns count
            float buttonWidth = 100;
            float windowWidth = ImGui.GetContentRegionAvail().X;
            ImGui.Columns((int) Math.Max(1, windowWidth / buttonWidth), "?", false);
            
            // Draw all directies
            foreach (string directory in directories)
            {
                // Draw the image
                string folderName = Path.GetFileName(directory);


                Vector2 uv = default(Vector2);
                Vector2 uv2 = new Vector2(1f, 1f);

                MetaData metaData = MetaData.Get(directory);

                Vector4 color = (Vector4)metaData.GetVector4("color", new Vector4(1, 1, 1, 1));

                if (ImGui.ImageButton(folderName, folderTexture.Handle, new System.Numerics.Vector2(100, 100), uv, uv2, default(Vector4), color))
                {
                    InspectorWindow.GetActive().SetInspection(new FolderAssetInspection(metaData));
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    browsePath = Path.GetRelativePath(Editor.projectPath, directory);
                }

                // Draw the file name
                ImGui.Text(folderName);
                ImGui.NextColumn();
            }

            // Draw all files
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                AssetManager assetManager = AssetManager.GetAssetManager(file);
                if (ImGui.ImageButton(fileName, assetManager.GetIcon(file).Handle, new System.Numerics.Vector2(100, 100)))
                {
                    InspectorWindow.GetActive().SetInspection(assetManager.GetInspection());
                }
                ImGui.Text(fileName);
                ImGui.NextColumn();
            }

            if (ImGui.BeginPopupContextWindow("FolderContext"))
            {
                if (ImGui.BeginMenu("Create"))
                {
                    if(ImGui.MenuItem("Test"))
                    {

                    }
                    // Action
                    ImGui.EndMenu();
                }

                if(ImGui.MenuItem("Open Folder in Explorer"))
                {
                    Process.Start("explorer.exe", currentPath);
                }

                ImGui.EndPopup();
            }

            ImGui.Columns(1); // Reset
            ImGui.End();
        }
    }
}
