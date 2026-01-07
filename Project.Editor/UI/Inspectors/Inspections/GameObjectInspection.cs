using ImGuiNET;
using Project.Editor.UI.Generic;
using Runtime;
using Runtime.Calc;
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

                    if (ImGui.Button($"Remove##{component.GetType().Name}"))
                    {
                        Type componentType = component.GetType();
                        gameObject.RemoveComponent(componentType);
                        Save();
                        return;
                    }
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                    FieldInfo[] fieldInfos = component.GetType().GetFields(flags);
                    foreach (FieldInfo info in fieldInfos)
                    {
                        DrawInspectableMember(info, info.FieldType, () => info.GetValue(component), v => info.SetValue(component, v));
                    }

                    PropertyInfo[] propertyInfos = component.GetType().GetProperties(flags);
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
            ImGui.SameLine();
            var record = ValueRecord.ValueRecordTypeFromType(valueType);
            if (record == null)
                return;


            if (record == ValueRecord.ValueRecordType.Bool)
            {
                object? value = getter();

                bool currentValue = value is bool b && b;

                if (ImGui.Checkbox($"##{member.Name}", ref currentValue))
                {
                    setter(currentValue);
                    Save();
                }

                return;
            }

            if (record == ValueRecord.ValueRecordType.Vector3)
            {
                System.Numerics.Vector3 value = ((Vector3)getter()).ToNumerics();

                if (ImGui.InputFloat3($"##{member.Name}", ref value))
                {
                    setter(new Vector3(value));
                    Save();
                }

                return;
            }

            if (record == ValueRecord.ValueRecordType.Float)
            {
                float value = (float)getter();

                if (ImGui.InputFloat($"##{member.Name}", ref value))
                {
                    setter(value);
                    Save();
                }

                return;
            }

            if (record == ValueRecord.ValueRecordType.Asset)
            {
                object? value = getter();

                string text = "";
                if (value == null)
                {
                    text = "No value selected";
                }
                else
                {
                    AssetReference assetReference = (AssetReference)value;
                    Asset? asset = assetReference.GetAsset();
                    text = asset != null ? asset.GetPath() : "[Instance]";
                }

                if (ImGui.Button($"{text}##" + member.Name))
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
