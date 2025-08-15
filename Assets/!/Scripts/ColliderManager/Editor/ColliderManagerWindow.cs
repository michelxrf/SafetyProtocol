using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ColliderTools
{
    /// <summary>
    /// Estrutura para armazenar informações do collider
    /// </summary>
    [Serializable]
    public class ColliderInfo
    {
        public GameObject gameObject;
        public Collider collider;
        public string colliderType;
        public bool isActive;
        public bool isTrigger;

        public ColliderInfo(Collider collider)
        {
            this.collider = collider;
            this.gameObject = collider.gameObject;
            this.colliderType = collider.GetType().Name;
            this.isActive = collider.enabled;
            this.isTrigger = collider.isTrigger;
        }
    }

    /// <summary>
    /// Classe para categorizar colliders por tipo
    /// </summary>
    [Serializable]
    public class ColliderCollection
    {
        public List<ColliderInfo> boxColliders = new List<ColliderInfo>();
        public List<ColliderInfo> sphereColliders = new List<ColliderInfo>();
        public List<ColliderInfo> capsuleColliders = new List<ColliderInfo>();
        public List<ColliderInfo> meshColliders = new List<ColliderInfo>();
        public List<ColliderInfo> wheelColliders = new List<ColliderInfo>();
        public List<ColliderInfo> characterControllers = new List<ColliderInfo>();
        public List<ColliderInfo> terrainColliders = new List<ColliderInfo>();
        public List<ColliderInfo> otherColliders = new List<ColliderInfo>();

        public int TotalCount => boxColliders.Count + sphereColliders.Count + capsuleColliders.Count +
                                meshColliders.Count + wheelColliders.Count + characterControllers.Count +
                                terrainColliders.Count + otherColliders.Count;
    }

    /// <summary>
    /// Editor Window para gerenciar colliders na cena
    /// </summary>
    public class ColliderManagerWindow : EditorWindow
    {
        private ColliderCollection colliderCollection;
        private Vector2 scrollPosition;
        private bool showBoxColliders = true;
        private bool showSphereColliders = true;
        private bool showCapsuleColliders = true;
        private bool showMeshColliders = true;
        private bool showWheelColliders = true;
        private bool showCharacterControllers = true;
        private bool showTerrainColliders = true;
        private bool showOtherColliders = true;

        private GUIStyle headerStyle;
        private GUIStyle buttonStyle;
        private bool stylesInitialized = false;

        [MenuItem("Tools/Collider Manager")]
        public static void ShowWindow()
        {
            ColliderManagerWindow window = GetWindow<ColliderManagerWindow>("Collider Manager");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshColliderList();
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                padding = new RectOffset(8, 8, 4, 4)
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitializeStyles();

            EditorGUILayout.BeginVertical("box");

            // Header
            EditorGUILayout.LabelField("Collider Manager", headerStyle);
            EditorGUILayout.Space(5);

            // Botões de ação
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("🔄 Refresh Colliders", buttonStyle, GUILayout.Height(30)))
            {
                RefreshColliderList();
            }

            if (GUILayout.Button("📊 Show Statistics", buttonStyle, GUILayout.Height(30)))
            {
                ShowStatistics();
            }

            EditorGUILayout.EndHorizontal();

            if (colliderCollection != null && colliderCollection.TotalCount > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"Total Colliders Found: {colliderCollection.TotalCount}", EditorStyles.helpBox);
                EditorGUILayout.Space(5);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                DrawColliderSection("Box Colliders", colliderCollection.boxColliders, ref showBoxColliders, Color.green);
                DrawColliderSection("Sphere Colliders", colliderCollection.sphereColliders, ref showSphereColliders, Color.blue);
                DrawColliderSection("Capsule Colliders", colliderCollection.capsuleColliders, ref showCapsuleColliders, Color.cyan);
                DrawColliderSection("Mesh Colliders", colliderCollection.meshColliders, ref showMeshColliders, Color.red);
                DrawColliderSection("Wheel Colliders", colliderCollection.wheelColliders, ref showWheelColliders, Color.yellow);
                DrawColliderSection("Character Controllers", colliderCollection.characterControllers, ref showCharacterControllers, Color.magenta);
                DrawColliderSection("Terrain Colliders", colliderCollection.terrainColliders, ref showTerrainColliders, Color.gray);
                DrawColliderSection("Other Colliders", colliderCollection.otherColliders, ref showOtherColliders, Color.white);

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox("No colliders found in the current scene. Click 'Refresh Colliders' to scan.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawColliderSection(string title, List<ColliderInfo> colliders, ref bool showSection, Color sectionColor)
        {
            if (colliders.Count == 0) return;

            EditorGUILayout.Space(5);

            // Header da seção com cor
            GUI.color = sectionColor;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            showSection = EditorGUILayout.Foldout(showSection, $"{title} ({colliders.Count})", true, EditorStyles.foldoutHeader);

            if (colliders.Count > 0)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("❌ Remove All", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    if (EditorUtility.DisplayDialog("Confirm Deletion",
                        $"Are you sure you want to remove all {colliders.Count} {title}?",
                        "Yes", "Cancel"))
                    {
                        RemoveAllCollidersOfType(colliders);
                    }
                }
                GUI.color = Color.white;
            }

            EditorGUILayout.EndHorizontal();

            if (showSection)
            {
                EditorGUI.indentLevel++;

                foreach (var colliderInfo in colliders.ToList()) // ToList para evitar modificação durante iteração
                {
                    if (colliderInfo.collider == null || colliderInfo.gameObject == null)
                    {
                        colliders.Remove(colliderInfo);
                        continue;
                    }

                    DrawColliderItem(colliderInfo, colliders);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawColliderItem(ColliderInfo colliderInfo, List<ColliderInfo> parentList)
        {
            EditorGUILayout.BeginHorizontal("box");

            // Ícone de status
            string statusIcon = colliderInfo.isActive ? "✅" : "⏸️";
            string triggerIcon = colliderInfo.isTrigger ? "🎯" : "🛡️";

            EditorGUILayout.LabelField($"{statusIcon} {triggerIcon}", GUILayout.Width(40));

            // Nome do GameObject (clicável)
            if (GUILayout.Button(colliderInfo.gameObject.name, EditorStyles.linkLabel))
            {
                Selection.activeGameObject = colliderInfo.gameObject;
                EditorGUIUtility.PingObject(colliderInfo.gameObject);
            }

            // Tipo do collider
            EditorGUILayout.LabelField(colliderInfo.colliderType, EditorStyles.miniLabel, GUILayout.Width(120));

            // Botões de ação
            if (GUILayout.Button("📍", GUILayout.Width(30)))
            {
                Selection.activeGameObject = colliderInfo.gameObject;
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button(colliderInfo.isActive ? "⏸️" : "▶️", GUILayout.Width(30)))
            {
                Undo.RecordObject(colliderInfo.collider, "Toggle Collider");
                colliderInfo.collider.enabled = !colliderInfo.collider.enabled;
                colliderInfo.isActive = colliderInfo.collider.enabled;
                EditorUtility.SetDirty(colliderInfo.gameObject);
            }

            GUI.color = Color.red;
            if (GUILayout.Button("🗑️", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm Deletion",
                    $"Remove {colliderInfo.colliderType} from '{colliderInfo.gameObject.name}'?",
                    "Yes", "Cancel"))
                {
                    RemoveCollider(colliderInfo, parentList);
                }
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Coleta todos os colliders da cena e os categoriza
        /// </summary>
        public void RefreshColliderList()
        {
            colliderCollection = new ColliderCollection();

            // Busca todos os colliders na cena ativa
            Collider[] allColliders = FindObjectsOfType<Collider>(true);
            CharacterController[] characterControllers = FindObjectsOfType<CharacterController>(true);

            // Categoriza colliders
            foreach (var collider in allColliders)
            {
                ColliderInfo info = new ColliderInfo(collider);
                CategorizeCollider(info);
            }

            // Adiciona Character Controllers separadamente
            foreach (var characterController in characterControllers)
            {
                ColliderInfo info = new ColliderInfo(characterController);
                colliderCollection.characterControllers.Add(info);
            }

            Debug.Log($"[ColliderManager] Found {colliderCollection.TotalCount} colliders in the scene.");
        }

        /// <summary>
        /// Categoriza o collider no tipo apropriado
        /// </summary>
        private void CategorizeCollider(ColliderInfo colliderInfo)
        {
            switch (colliderInfo.collider)
            {
                case BoxCollider _:
                    colliderCollection.boxColliders.Add(colliderInfo);
                    break;
                case SphereCollider _:
                    colliderCollection.sphereColliders.Add(colliderInfo);
                    break;
                case CapsuleCollider _:
                    colliderCollection.capsuleColliders.Add(colliderInfo);
                    break;
                case MeshCollider _:
                    colliderCollection.meshColliders.Add(colliderInfo);
                    break;
                case WheelCollider _:
                    colliderCollection.wheelColliders.Add(colliderInfo);
                    break;
                case TerrainCollider _:
                    colliderCollection.terrainColliders.Add(colliderInfo);
                    break;
                default:
                    colliderCollection.otherColliders.Add(colliderInfo);
                    break;
            }
        }

        /// <summary>
        /// Remove um collider específico
        /// </summary>
        private void RemoveCollider(ColliderInfo colliderInfo, List<ColliderInfo> parentList)
        {
            if (colliderInfo.collider != null)
            {
                Undo.DestroyObjectImmediate(colliderInfo.collider);
                parentList.Remove(colliderInfo);
                EditorUtility.SetDirty(colliderInfo.gameObject);
                Debug.Log($"[ColliderManager] Removed {colliderInfo.colliderType} from '{colliderInfo.gameObject.name}'");
            }
        }

        /// <summary>
        /// Remove todos os colliders de um tipo específico
        /// </summary>
        private void RemoveAllCollidersOfType(List<ColliderInfo> colliders)
        {
            int removedCount = 0;
            var collidersToRemove = colliders.ToList();

            foreach (var colliderInfo in collidersToRemove)
            {
                if (colliderInfo.collider != null)
                {
                    Undo.DestroyObjectImmediate(colliderInfo.collider);
                    EditorUtility.SetDirty(colliderInfo.gameObject);
                    removedCount++;
                }
            }

            colliders.Clear();
            Debug.Log($"[ColliderManager] Removed {removedCount} colliders.");
        }

        /// <summary>
        /// Mostra estatísticas detalhadas dos colliders
        /// </summary>
        private void ShowStatistics()
        {
            if (colliderCollection == null)
            {
                RefreshColliderList();
                return;
            }

            string stats = "=== COLLIDER STATISTICS ===\n\n";
            stats += $"📦 Box Colliders: {colliderCollection.boxColliders.Count}\n";
            stats += $"⚪ Sphere Colliders: {colliderCollection.sphereColliders.Count}\n";
            stats += $"💊 Capsule Colliders: {colliderCollection.capsuleColliders.Count}\n";
            stats += $"🔺 Mesh Colliders: {colliderCollection.meshColliders.Count}\n";
            stats += $"🛞 Wheel Colliders: {colliderCollection.wheelColliders.Count}\n";
            stats += $"🚶 Character Controllers: {colliderCollection.characterControllers.Count}\n";
            stats += $"🏔️ Terrain Colliders: {colliderCollection.terrainColliders.Count}\n";
            stats += $"❓ Other Colliders: {colliderCollection.otherColliders.Count}\n";
            stats += $"\n📊 TOTAL: {colliderCollection.TotalCount} colliders\n";

            EditorUtility.DisplayDialog("Collider Statistics", stats, "OK");
        }

        /// <summary>
        /// Retorna a coleção de colliders categorizados
        /// </summary>
        public ColliderCollection GetColliderCollection()
        {
            if (colliderCollection == null)
                RefreshColliderList();
            return colliderCollection;
        }

        /// <summary>
        /// API pública para obter colliders por tipo
        /// </summary>
        public List<T> GetCollidersByType<T>() where T : Collider
        {
            if (colliderCollection == null)
                RefreshColliderList();

            List<T> result = new List<T>();

            var allLists = new List<List<ColliderInfo>>
            {
                colliderCollection.boxColliders,
                colliderCollection.sphereColliders,
                colliderCollection.capsuleColliders,
                colliderCollection.meshColliders,
                colliderCollection.wheelColliders,
                colliderCollection.terrainColliders,
                colliderCollection.otherColliders
            };

            foreach (var list in allLists)
            {
                foreach (var info in list)
                {
                    if (info.collider is T collider)
                        result.Add(collider);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Classe utilitária para acesso via script
    /// </summary>
    public static class ColliderManagerUtility
    {
        /// <summary>
        /// Coleta todos os colliders da cena e retorna categorizado
        /// </summary>
        public static ColliderCollection GetAllCollidersInScene()
        {
            var window = EditorWindow.GetWindow<ColliderManagerWindow>();
            window.RefreshColliderList();
            return window.GetColliderCollection();
        }

        /// <summary>
        /// Obtém colliders por tipo específico
        /// </summary>
        public static List<T> GetCollidersByType<T>() where T : Collider
        {
            var window = EditorWindow.GetWindow<ColliderManagerWindow>();
            return window.GetCollidersByType<T>();
        }
    }
}