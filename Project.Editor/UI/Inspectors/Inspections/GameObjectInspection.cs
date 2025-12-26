using ImGuiNET;
using Project.Editor.UI.Generic;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Objects.Prefabs;
using System.Reflection;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public class GameObjectInspection : Inspection
    {
        GameObject gameObject;
        Asset asset;
        public GameObjectInspection(GameObject gameObject, Asset asset)
        {
            this.gameObject = gameObject;
            this.asset = asset;
        }

        public override void Render()
        {
            foreach (IComponent component in gameObject.GetComponents())
            {
                if (ImGui.CollapsingHeader(component.GetType().Name))
                {
                    if (ImGui.Button("Remove"))
                    {
                        Type componentType = component.GetType();
                        gameObject.RemoveComponent(componentType);
                    }

                    FieldInfo[] fieldInfos = component.GetType().GetFields();
                    foreach (FieldInfo info in fieldInfos)
                    {
                        DrawInspectableMember(info, info.FieldType, () => info.GetValue(component), v => info.SetValue(component, v));
                    }

                    PropertyInfo[] propertyInfos = component.GetType().GetProperties();
                    foreach (PropertyInfo info in propertyInfos)
                    {
                        DrawInspectableMember(info, info.PropertyType, () => info.GetValue(component), v => info.SetValue(component, v));
                    }
                }
            }

            if (ImGui.Button("Add Component"))
            {
                ComponentSelectorWindow guiWindow = new ComponentSelectorWindow();
                GuiWindow.Enable(guiWindow);

                guiWindow.OnComponentPicked += (Type type) =>
                {
                    gameObject.AddComponent((IComponent)Activator.CreateInstance(type));
                    GuiWindow.Disable(guiWindow);
                    Save();
                };
            }
        }

        public void Save()
        {
            Debug.Log("Saving prefab...");
            PrefabGameObject prefab = PrefabGameObject.FromGameObject(gameObject);
            File.WriteAllText(asset.GetSystemPath(), prefab.ToJson());
        }
        void DrawInspectableMember(MemberInfo member, Type valueType, Func<object?> getter, Action<object?> setter)
        {
            InspectableAttribute? attribute =
                member.GetCustomAttribute<InspectableAttribute>();

            if (attribute == null)
                return;

            ImGui.Text(member.Name);

            var record = ValueRecord.ValueRecordTypeFromType(valueType);
            if (record == null)
                return;

            if (record == ValueRecord.ValueRecordType.Asset)
            {
                object? value = getter();

                if (value == null)
                {
                    ImGui.Text("No value selected.");
                }
                else
                {
                    AssetReference assetReference = (AssetReference)value;
                    Asset? asset = assetReference.GetAsset();
                    ImGui.Text(asset != null ? asset.GetPath() : "[Instance]");
                }

                if (ImGui.Button("Select##" + member.Name))
                {
                    AssetReferenceAttribute? att =
                        valueType.GetCustomAttribute<AssetReferenceAttribute>();

                    if (att == null)
                        return;

                    AssetSelectorWindow window =
                        new AssetSelectorWindow(att.filetype.First(), Game.GetAssetDatabase());

                    window.OnSelect += selected =>
                    {
                        setter(selected.CreateInstance(valueType));
                        Save();
                    };

                    GuiWindow.Enable(window);
                }
            }
        }
    }
}
