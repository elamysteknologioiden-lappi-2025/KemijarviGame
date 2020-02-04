/******************************************************************************
* File         : SceneSwitcherWindow.cs
* Lisence      : BSD 3-Clause License
* Copyright    : Lapland University of Applied Sciences
* Authors      : Arto Söderström
* BSD 3-Clause License
*
* Copyright (c) 2019, Lapland University of Applied Sciences
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
* 
* 1. Redistributions of source code must retain the above copyright notice, this
*  list of conditions and the following disclaimer.
*
* 2. Redistributions in binary form must reproduce the above copyright notice,
*  this list of conditions and the following disclaimer in the documentation
*  and/or other materials provided with the distribution.
*
* 3. Neither the name of the copyright holder nor the names of its
*  contributors may be used to endorse or promote products derived from
*  this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************/

using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;

/// <summary>
/// SceneSwitcherWindow opens a window which lists all the scenes in the build settings and project to quickly change between scenes
/// </summary>
public class pLab_SceneSwitcherWindow : EditorWindow
{
    [System.Serializable]
    public class FavoriteScenes {
        public List<string> scenes = new List<string>();
    }
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    private bool showFavorites = true;
    private bool showBuildScenes = true;
    private bool showAllScenes = true;

    private const string SHOW_BUILD_SCENES_PREF_KEY = "SceneSwitcherWindow.ShowBuildScenes";
    private const string SHOW_ALL_SCENES_PREF_KEY = "SceneSwitcherWindow.ShowAllScenes";
    private const string SHOW_FAVORITE_SCENES_PREF_KEY = "SceneSwitcherWindow.ShowFavoriteScenes";
    private const string FAVORITE_SCENES_PREF_KEY = "SceneSwitcherWindow.FavoriteScenes";

    private bool focusOnFilter = true;
    private bool isEditing = false;

    private FavoriteScenes favorites = new FavoriteScenes();
    private string filterText = "";

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("Tools/Scene Switcher %q")]
    internal static void Init()
    {
        pLab_SceneSwitcherWindow window = (pLab_SceneSwitcherWindow) GetWindow(typeof(pLab_SceneSwitcherWindow), false, "Scene Switcher");
        window.focusOnFilter = true;
        window.Refresh();
    }

    internal void OnGUI()
    {
        GUILayout.Space(4);
        Event e = Event.current;

        //Move focus to filter-textfield on CTRL + F
        if (e.keyCode == KeyCode.F && e.control) {
            focusOnFilter = true;
        }
        else if (e.keyCode == KeyCode.R && e.control) {
            if (e.type == EventType.KeyUp)  {
                //Refresh on CTRL + R
                Refresh();
            }

            //Use the event so it won't be passed on to anything else
            e.Use();
        }

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft };

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        // GUILayout.Label(EditorGUIUtility.IconContent("ViewToolZoom"), GUILayout.ExpandWidth(false));
        GUI.SetNextControlName("FilterTextField");
        filterText = EditorGUILayout.TextField(filterText, EditorStyles.toolbarSearchField);
        GUIStyle toolbarCancelStyle = GUI.skin.GetStyle("ToolbarSeachCancelButton");
        bool cleared = GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_graph_close_h"), toolbarCancelStyle, GUILayout.ExpandWidth(false));

        if (cleared) {
            filterText = "";
            GUI.FocusControl(null);
            Repaint();
        }

        if (focusOnFilter) {
            EditorGUI.FocusTextInControl("FilterTextField");
            focusOnFilter = false;
        }
        
        bool createNewScene = false;


        if (GUILayout.Button(new GUIContent("Create", "Create a new scene"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            createNewScene = true;
        }
        
        bool refresh = GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "|Refresh"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
        isEditing = GUILayout.Toggle(isEditing, EditorGUIUtility.IconContent("d_editicon.sml", "|Edit mode: Rename or remove scenes"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));

        if (refresh) {
            RemoveDeletedFavorites();
        }

        EditorGUILayout.EndHorizontal();
        filterText = filterText.ToLower();

        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

        GUILayout.Space(4);
        GUIStyle favoritesFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        favoritesFoldoutStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.BeginHorizontal();
        showFavorites = EditorGUILayout.Foldout(showFavorites, "Favorites", true, favoritesFoldoutStyle);


        EditorGUILayout.EndHorizontal();

        if (showFavorites) {
            for(int i = 0; i < favorites.scenes.Count; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(favorites.scenes[i]);
                if (assetPath != null && assetPath != "") {
                    string sceneName = Path.GetFileNameWithoutExtension(assetPath);
                    if (!sceneName.ToLower().Contains(filterText)) continue;
                    EditorGUILayout.BeginHorizontal();
                    bool favorite = GUILayout.Button(EditorGUIUtility.IconContent("Favorite", "|Remove from favorites"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                    bool pressed = false;

                    if (isEditing) {
                        EditorGUILayout.BeginHorizontal();

                        string newSceneName = EditorGUILayout.DelayedTextField(sceneName, GUILayout.Height(20));

                        if (newSceneName != null && newSceneName != "" && newSceneName != sceneName) {
                            string statusMessage = AssetDatabase.RenameAsset(assetPath, newSceneName);
                            AssetDatabase.Refresh();

                            if (statusMessage == "") {
                                //Everything went ok
                                Debug.Log($"Renamed {assetPath} to {newSceneName}");
                            } else {
                                //There was an error
                                Debug.LogError($"Error renaming scene: {statusMessage}");
                            }
                        }

                        bool deleteAsset = GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash", "|Delete scene asset"), EditorStyles.miniButton, GUILayout.ExpandWidth(false));

                        if (deleteAsset) {
                            bool wasRemoved = AssetDatabase.DeleteAsset(assetPath);

                            if (wasRemoved) {
                                Debug.Log($"Deleted scene at {assetPath}");
                                ToggleFavorite(favorites.scenes[i]);
                                AssetDatabase.Refresh();
                            } else {
                                Debug.LogError($"Failed to delete scene at {assetPath}");
                            }

                        }

                        EditorGUILayout.EndHorizontal();
                    } else {
                        pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);
                    }

                    EditorGUILayout.EndHorizontal();

                    //If favorite button was clicked
                    if (favorite) {
                        ToggleFavorite(favorites.scenes[i]);
                    }
                    //If button was pressed
                    if (pressed)
                    {
                        OpenScene(assetPath);
                    }
                }
            }
        }
 
        GUILayout.Space(10);
        showBuildScenes = EditorGUILayout.Foldout(showBuildScenes, "Scenes In Build", true);


        if (showBuildScenes) {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                if (scene.path != null && scene.path != "") {
                    string sceneGuid = scene.guid.ToString();
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    if (!sceneName.ToLower().Contains(filterText)) continue;

                    EditorGUILayout.BeginHorizontal();
                    bool isFavorite = favorites.scenes.Contains(scene.guid.ToString());

                    bool favorite = GUILayout.Button(EditorGUIUtility.IconContent(isFavorite ? "Favorite" : "d_favorite", isFavorite ? "|Remove from favorites" : "|Add to favorites"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

                    bool pressed = false;

                    if (isEditing) {
                        EditorGUILayout.BeginHorizontal();

                        string newSceneName = EditorGUILayout.DelayedTextField(sceneName, GUILayout.Height(20));
                        string assetPath = scene.path;

                        if (newSceneName != null && newSceneName != "" && newSceneName != sceneName) {
                            string statusMessage = AssetDatabase.RenameAsset(assetPath, newSceneName);
                            AssetDatabase.Refresh();

                            if (statusMessage == "") {
                                //Everything went ok
                                Debug.Log($"Renamed {assetPath} to {newSceneName}");
                            } else {
                                //There was an error
                                Debug.LogError($"Error renaming scene: {statusMessage}");
                            }
                        }

                        bool deleteAsset = GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash", "|Delete scene asset"), EditorStyles.miniButton, GUILayout.ExpandWidth(false));

                        if (deleteAsset) {
                            bool wasRemoved = AssetDatabase.DeleteAsset(assetPath);

                            if (wasRemoved) {
                                Debug.Log($"Deleted scene at {assetPath}");
                                if (favorites.scenes.IndexOf(sceneGuid) != -1) {
                                    ToggleFavorite(sceneGuid);
                                }
                                AssetDatabase.Refresh();
                            } else {
                                Debug.LogError($"Failed to delete scene at {assetPath}");
                            }

                        }

                        EditorGUILayout.EndHorizontal();
                    } else {
                        pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);
                    }

                    if (favorite) {
                        ToggleFavorite(sceneGuid);
                    }

                    EditorGUILayout.EndHorizontal();
                    //If button was pressed
                    if (pressed)
                    {
                        OpenScene(scene.path);
                    }
                }
            }
        }

        GUILayout.Space(10);

        showAllScenes = EditorGUILayout.Foldout(showAllScenes, "All Scenes", true);

        if (showAllScenes) {
            string[] guids = AssetDatabase.FindAssets("t:Scene");

            for(int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                string sceneName = Path.GetFileNameWithoutExtension(assetPath);
                if (!sceneName.ToLower().Contains(filterText)) continue;

                EditorGUILayout.BeginHorizontal();

                bool isFavorite = favorites.scenes.Contains(guids[i]);
                bool favorite = GUILayout.Button(EditorGUIUtility.IconContent(isFavorite ? "Favorite" : "d_favorite", isFavorite ? "|Remove from favorites" : "|Add to favorites"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                bool pressed = false;

                if (isEditing) {
                    EditorGUILayout.BeginHorizontal();

                    string newSceneName = EditorGUILayout.DelayedTextField(sceneName, GUILayout.Height(20));

                    if (newSceneName != null && newSceneName != "" && newSceneName != sceneName) {
                        string statusMessage = AssetDatabase.RenameAsset(assetPath, newSceneName);
                        AssetDatabase.Refresh();

                        if (statusMessage == "") {
                            //Everything went ok
                            Debug.Log($"Renamed {assetPath} to {newSceneName}");
                        } else {
                            //There was an error
                            Debug.LogError($"Error renaming scene: {statusMessage}");
                        }
                    }

                    bool deleteAsset = GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash", "|Delete scene asset"), EditorStyles.miniButton, GUILayout.ExpandWidth(false));

                    if (deleteAsset) {
                        bool wasRemoved = AssetDatabase.DeleteAsset(assetPath);

                        if (wasRemoved) {
                            Debug.Log($"Deleted scene at {assetPath}");
                            if (favorites.scenes.IndexOf(guids[i]) != -1) {
                                ToggleFavorite(guids[i]);
                            }

                            AssetDatabase.Refresh();
                        } else {
                            Debug.LogError($"Failed to delete scene at {assetPath}");
                        }

                    }

                    EditorGUILayout.EndHorizontal();

                } else {
                    pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);
                }

                EditorGUILayout.EndHorizontal();

                //If favorite button was clicked
                if (favorite) {
                    ToggleFavorite(guids[i]);
                }

                if (pressed) {
                    OpenScene(assetPath);
                }
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (createNewScene) {
            CreateNewScene();
        }
    }

    /// <summary>
    /// Starts new scene creation process
    /// </summary>
    private void CreateNewScene() {
        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        if (EditorSceneManager.SaveScene(scene)) {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// Refresh. Removes deleted favorites
    /// </summary>
    private void Refresh() {
        RemoveDeletedFavorites();
    }

    /// <summary>
    /// Goes through all the favorites and removes every entry that references a scene file that is deleted
    /// </summary>
    private void RemoveDeletedFavorites() {

        bool saveNeeded = false;

        for(int i = 0; i < favorites.scenes.Count; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(favorites.scenes[i]);
            if (assetPath != null && assetPath != "") {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

                //If the asset was deleted
                if (sceneAsset == null) {
                    ToggleFavorite(favorites.scenes[i]);
                    saveNeeded = true;
                }
            }
        }

        if (saveNeeded) {
            SavePrefs();
        }
    }

    /// <summary>
    /// Open scene from scene path
    /// </summary>
    /// <param name="scenePath"></param>
    private void OpenScene(string scenePath) {
        if (scenePath != "") {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }

    /// <summary>
    /// Toggle favorite status of scene
    /// </summary>
    /// <param name="guid"></param>
    private void ToggleFavorite(string guid) {
        int index = favorites.scenes.IndexOf(guid);

        if (index != -1) {
            favorites.scenes.RemoveAt(index);
        } else {
            favorites.scenes.Add(guid);
        }
    }

    /// <summary>
    /// Save favorites and user preferences
    /// </summary>
    private void SavePrefs() {
        EditorPrefs.SetBool(SHOW_BUILD_SCENES_PREF_KEY, showBuildScenes);
        EditorPrefs.SetBool(SHOW_ALL_SCENES_PREF_KEY, showAllScenes);
        EditorPrefs.SetBool(SHOW_FAVORITE_SCENES_PREF_KEY, showFavorites);

        if (favorites != null && favorites.scenes.Count > 0) {
            string favoritesAsJson = EditorJsonUtility.ToJson(favorites);
            PlayerPrefs.SetString(FAVORITE_SCENES_PREF_KEY, favoritesAsJson);
        } else {
            PlayerPrefs.DeleteKey(FAVORITE_SCENES_PREF_KEY);
        }
    }

    /// <summary>
    /// Load favorites and window preferences
    /// </summary>
    private void LoadPrefs() {
        if (EditorPrefs.HasKey(SHOW_BUILD_SCENES_PREF_KEY))
            showBuildScenes = EditorPrefs.GetBool(SHOW_BUILD_SCENES_PREF_KEY);

        if (EditorPrefs.HasKey(SHOW_ALL_SCENES_PREF_KEY))
            showAllScenes = EditorPrefs.GetBool(SHOW_ALL_SCENES_PREF_KEY);

        if (EditorPrefs.HasKey(SHOW_FAVORITE_SCENES_PREF_KEY))
            showFavorites = EditorPrefs.GetBool(SHOW_FAVORITE_SCENES_PREF_KEY);
        
        if (PlayerPrefs.HasKey(FAVORITE_SCENES_PREF_KEY)) {
            string favoritesAsJson = PlayerPrefs.GetString(FAVORITE_SCENES_PREF_KEY);

            if (favoritesAsJson != "") {
                try {
                    EditorJsonUtility.FromJsonOverwrite(favoritesAsJson, favorites);
                } catch(Exception e) {

                }
            }
        }
    }

    private void OnFocus()
    {
        LoadPrefs();
    }

    private void OnLostFocus()
    {
        SavePrefs();
    }
}