using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BrainData))]
public class BrainDataEditor : Editor
{
    private ReorderableList list;

    private List<Type> types = new List<Type>();

    private void OnEnable()
    {
        this.list = new ReorderableList(this.serializedObject,
                                        this.serializedObject.FindProperty("SteerNeurons"),
                                        true,  //draggable
                                        true,  //display header
                                        true,  //has add button
                                        true); //has delete button

        //cache all the types that derive from Neuron
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var t in assembly.GetTypes())
            {
                if (typeof(NeuronSteer).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                {
                    types.Add(t);
                }
            }
        }

        this.list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "SteerNeurons"); };

        this.list.drawElementCallback = ListDrawElement;
        this.list.onAddDropdownCallback = ListAddDropDown;
        this.list.onRemoveCallback = ListRemove;
    }

    private void ListAddDropDown(Rect buttonRect, ReorderableList list)
    {
        var menu = new GenericMenu();

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.ToString()), false, ListAddItemClicked, type);
        }

        menu.ShowAsContext();
    }

    private void ListAddItemClicked(object targetItem)
    {
        var brainData = this.target as BrainData;
        var targetType = targetItem as Type;

        var newScriptableObject = ScriptableObject.CreateInstance(targetType);

        //we'll name the object based on the type - it'd be nice to be able to edit this later
        //but it's a property not a field...
        newScriptableObject.name = targetType.ToString();

        brainData.SteerNeurons.Add(newScriptableObject as NeuronSteer);

        //add to the asset database so that the object saves
        AssetDatabase.AddObjectToAsset(newScriptableObject, this.target);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(base.target));
    }

    private void ListRemove(ReorderableList list)
    {
        var brainData = this.target as BrainData;

        var neuron = this.list.serializedProperty.GetArrayElementAtIndex(this.list.index).objectReferenceValue as NeuronSteer;

        brainData.SteerNeurons.Remove(neuron);

        //destroy the asset in the databse as well
        UnityEngine.Object.DestroyImmediate(neuron, true);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(base.target));
    }

    private void ListDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var serializedPropertyForIndex = this.list.serializedProperty.GetArrayElementAtIndex(index);

        var item = serializedPropertyForIndex.objectReferenceValue as NeuronSteer;

        if (item == null)
            return;

        //show the name of the neuron at the same y as the list item
        EditorGUI.LabelField(rect, serializedPropertyForIndex.objectReferenceValue.name);

        //we want to show the inspector for the selected object 
        if (isActive)
        {
            var editor = Editor.CreateEditor(item, typeof(NeuronEditor));
            editor.OnInspectorGUI();
        }
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        this.list.DoLayoutList();

        this.serializedObject.ApplyModifiedProperties();
    }
}