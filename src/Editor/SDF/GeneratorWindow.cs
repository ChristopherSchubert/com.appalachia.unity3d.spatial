﻿#if UNITY_EDITOR
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Spatial.SDF
{
    [CallStaticConstructorInEditor]
    public class GeneratorWindow : UnityEditor.EditorWindow
    {
        private static readonly Generator Generator;

        static GeneratorWindow()
        {
            Generator = new Generator();
        }

        private void CreateSDF()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Save As",
                Generator.Mesh.name + "_SDF",
                "asset",
                ""
            );

            if ((path == null) || path.Equals(""))
            {
                return;
            }

            var voxels = Generator.Generate();

            AssetDatabaseManager.CreateAsset(voxels, path);
            AssetDatabaseManager.SaveAssets();
            AssetDatabaseManager.Refresh();

            Close();

            Selection.activeObject = AssetDatabaseManager.LoadMainAssetAtPath(path);
        }

        private void OnGUI()
        {
            if (!SystemInfo.supportsComputeShaders)
            {
                EditorGUILayout.HelpBox(
                    "This tool requires a GPU that supports compute shaders.",
                    MessageType.Error
                );

                if (GUILayout.Button("Close"))
                {
                    Close();
                }

                return;
            }

            Generator.Mesh = EditorGUILayout.ObjectField("Mesh", Generator.Mesh, typeof(Mesh), false) as Mesh;

            if (Generator.Mesh == null)
            {
                if (GUILayout.Button("Close"))
                {
                    Close();
                }

                return;
            }

            if (Generator.Mesh.subMeshCount > 1)
            {
                Generator.SubMeshIndex = (int) Mathf.Max(
                    EditorGUILayout.IntField("Submesh Index", Generator.SubMeshIndex),
                    0f
                );
            }

            Generator.Padding = EditorGUILayout.Slider("Padding", Generator.Padding, 0f, 1f);

            Generator.Resolution = (int) Mathf.Max(
                EditorGUILayout.IntField("Resolution", Generator.Resolution),
                1f
            );

            if (GUILayout.Button("Create"))
            {
                CreateSDF();
            }

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Windows.Base + "Generator")]
        private static void Window()
        {
            var window = (GeneratorWindow) GetWindow(typeof(GeneratorWindow), true, "Generate SDF");
            window.ShowUtility();
        }
    }
}

#endif
