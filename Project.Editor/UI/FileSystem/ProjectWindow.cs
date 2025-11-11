using ImGuiNET;
using Project.Editor.UI.FileSystem.FileInspectors;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem
{
    internal class ProjectWindow : GuiWindow
    {
        Texture folderTexture;
        public ProjectWindow()
        {
            folderTexture = new Texture("assets/textures/icons/folder.png");
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
                if (ImGui.ImageButton(folderName, folderTexture.Handle, new System.Numerics.Vector2(100, 100)))
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
                if (ImGui.ImageButton(fileName, FileInspector.GetInspector(file).GetIcon(file).Handle, new System.Numerics.Vector2(100, 100)))
                {
                    
                }
                ImGui.Text(fileName);
                ImGui.NextColumn();
            }

            ImGui.Columns(1); // Reset
            ImGui.End();
        }
    }
}
