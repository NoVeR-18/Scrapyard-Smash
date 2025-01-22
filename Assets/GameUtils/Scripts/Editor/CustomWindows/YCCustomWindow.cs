using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCCustomWindow : EditorWindow {

            public enum WindowPosition {
                UpperLeft,
                UpperCenter,
                UpperRight,
                MiddleLeft,
                MiddleCenter,
                MiddleRight,
                LowerLeft,
                LowerCenter,
                LowerRight
            }

            public virtual void OnGUI() {
                this.ShortcutHandling();
            }

            private Rect GetMainWindowRect() {
#if UNITY_2020_1_OR_NEWER
                return EditorGUIUtility.GetMainWindowPosition();
#else
                return new Rect(0, 0, 1920, 1080);
#endif
            }

            #region Size / Position
            public void SetMinSize(float width, float height) {
                width = Mathf.Min(width, this.GetMainWindowRect().width * 0.9f);
                height = Mathf.Min(height, this.GetMainWindowRect().height * 0.9f);
                this.minSize = new Vector2(width, height);
            }

            public void SetMaxSize(float width, float height) {
                width = Mathf.Min(width, this.GetMainWindowRect().width * 0.9f);
                height = Mathf.Min(height, this.GetMainWindowRect().height * 0.9f);
                this.maxSize = new Vector2(width, height);
            }

            public void SetSize(float width, float height) {
                this.SetMinSize(width, height);
                this.SetMaxSize(width, height);
            }

            public void SetPosition(int posX, int posY) {
                Rect r = this.position;
                r.x = posX;
                r.y = posY;
                this.position = r;
            }

            public void SetPosition(WindowPosition windowPosition) {
                Rect r = this.position;
                float centerX = this.GetMainWindowRect().center.x - this.position.width / 2;
                float centerY = this.GetMainWindowRect().center.y - this.position.height / 2;
                float rightX = this.GetMainWindowRect().width - this.position.width;
                float downY = this.GetMainWindowRect().height - this.position.height;
                switch (windowPosition) {
                    case WindowPosition.UpperLeft:
                        r.x = 0;
                        r.y = 0;
                        break;

                    case WindowPosition.UpperCenter:
                        r.x = centerX;
                        r.y = 0;
                        break;

                    case WindowPosition.UpperRight:
                        r.x = rightX;
                        r.y = 0;
                        break;

                    case WindowPosition.MiddleLeft:
                        r.x = 0;
                        r.y = centerY;
                        break;

                    case WindowPosition.MiddleCenter:
                        r.x = centerX;
                        r.y = centerY;
                        break;

                    case WindowPosition.MiddleRight:
                        r.x = rightX;
                        r.y = centerY;
                        break;

                    case WindowPosition.LowerLeft:
                        r.x = 0;
                        r.y = downY;
                        break;

                    case WindowPosition.LowerCenter:
                        r.x = centerX;
                        r.y = downY;
                        break;

                    case WindowPosition.LowerRight:
                        r.x = rightX;
                        r.y = downY;
                        break;
                }
                this.position = r;
            }
            #endregion

            protected void AddEmptyLine(int amount = 0) {
                string spaces = "";
                for (int i = 0; i < amount; i++) {
                    spaces += "\n";
                }
                GUILayout.Label(spaces);
            }

            protected void AddLabel(GUIContent content, TextAnchor alignment = TextAnchor.MiddleLeft, GUIStyle style = null) {
                GUIStyle newstyle = new GUIStyle(style == null ? GUI.skin.label : style);
                newstyle.alignment = alignment;
                GUILayout.Label(content, newstyle);
            }
            protected void AddLabel(string text, TextAnchor alignment = TextAnchor.MiddleLeft, GUIStyle style = null) {
                this.AddLabel(new GUIContent(text), alignment, style);
            }

            protected void AddSelectableLabel(string text, TextAnchor alignment = TextAnchor.MiddleLeft, GUIStyle style = null) {
                GUIStyle newstyle = new GUIStyle(style == null ? GUI.skin.label : style);
                newstyle.alignment = alignment;
                GUILayout.BeginVertical("box");
                EditorGUILayout.SelectableLabel(text, newstyle);
                GUILayout.EndVertical();
            }

            protected string AddTextInputField(GUIContent title, string defaultValue, GUIStyle style = null) {
                GUIStyle newstyle = new GUIStyle(style == null ? GUI.skin.textField : style);
                return EditorGUILayout.TextField(title, defaultValue, newstyle);
            }
            protected string AddTextInputField(string title, string defaultValue, GUIStyle style = null) {
                return this.AddTextInputField(new GUIContent(title), defaultValue, style);
            }

            protected void AddTextAreaField(GUIContent title, ref string defaultValue) {
                GUILayout.BeginHorizontal();
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fixedWidth = 147;
                this.AddLabel(title, TextAnchor.MiddleLeft, labelStyle);
                defaultValue = EditorGUILayout.TextArea(defaultValue);
                GUILayout.EndHorizontal();
            }
            protected void AddTextAreaField(string title, ref string defaultValue) {
                this.AddTextAreaField(new GUIContent(title), ref defaultValue);
            }

            protected bool AddToggle(GUIContent title = null, bool defaultValue = false, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                style.alignment = alignment;
                return GUILayout.Toggle(defaultValue, title, style);
            }
            protected bool AddToggle(string title, bool defaultValue = false, TextAnchor alignment = TextAnchor.MiddleLeft) {
                return this.AddToggle(new GUIContent(title), defaultValue, alignment);
            }

            protected bool AddBoolField(GUIContent title = null, bool defaultValue = false, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                style.alignment = alignment;
                return EditorGUILayout.Toggle(title, defaultValue, style);
            }
            protected bool AddBoolField(string title, bool defaultValue = false, TextAnchor alignment = TextAnchor.MiddleLeft) {
                return this.AddBoolField(new GUIContent(title), defaultValue, alignment);
            }

            protected void AddButton(GUIContent text, Action action, GUIStyle style = null) {
                GUIStyle newstyle = new GUIStyle(style == null ? GUI.skin.button : style);
                if (GUILayout.Button(text, newstyle)) {
                    action();
                }
            }
            protected void AddButton(string text, Action action, GUIStyle style = null) {
                this.AddButton(new GUIContent(text), action, style);
            }

            protected void AddButtonValidation(GUIContent text, Action action) {
                if (GUILayout.Button(text) || (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))) {
                    action();
                }
            }
            protected void AddButtonValidation(string text, Action action) {
                this.AddButtonValidation(new GUIContent(text), action);
            }

            protected void AddButtonClose(GUIContent text = null, Action closeAction = null) {
                if (text == null) {
                    text = new GUIContent("Close");
                }
                if (GUILayout.Button(text) || (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Escape))) {
                    closeAction?.Invoke();
                    this.Close();
                }
            }
            protected void AddButtonClose(string text = "", Action closeAction = null) {
                if (text == "") {
                    GUIContent c = null;
                    this.AddButtonClose(c, closeAction);
                } else {
                    this.AddButtonClose(new GUIContent(text), closeAction);
                }
            }

            protected void AddCancelOk(GUIContent cancel, GUIContent ok, Action cancelAction, Action okAction) {
                GUILayout.BeginHorizontal();
                this.AddButtonClose(cancel, cancelAction);
                this.AddButtonValidation(ok, okAction);
                GUILayout.EndHorizontal();
            }
            protected void AddCancelOk(string cancel, string ok, Action cancelAction, Action okAction) {
                this.AddCancelOk(new GUIContent(cancel), new GUIContent(ok), cancelAction, okAction);
            }

            protected void AddFoldout(GUIContent title, ref bool status, Action inside) {
                GUILayout.BeginVertical("box");
                status = EditorGUILayout.Foldout(status, title, true);
                if (status) {
                    GUIStyle style = new GUIStyle(GUI.skin.box);
                    style.margin.left = 25;
                    GUILayout.BeginVertical(style);
                    inside.Invoke();
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            protected void AddFoldout(string text, ref bool status, Action inside) {
                this.AddFoldout(new GUIContent(text), ref status, inside);
            }

            protected void AddScrollView(ref Vector2 scrollPosition, Action inside, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false) {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical);
                inside.Invoke();
                EditorGUILayout.EndScrollView();
            }

            protected Enum AddEnumPopup(GUIContent title, Enum enumerator) {
                return EditorGUILayout.EnumPopup(title, enumerator);
            }
            protected Enum AddEnumPopup(string title, Enum enumerator) {
                return this.AddEnumPopup(new GUIContent(title), enumerator);
            }

            protected Enum AddEnumFlagPopup(GUIContent title, Enum enumerator) {
                return EditorGUILayout.EnumFlagsField(title, enumerator);
            }
            protected Enum AddEnumFlagPopup(string title, Enum enumerator) {
                return this.AddEnumFlagPopup(new GUIContent(title), enumerator);
            }

            protected void AddTable(int column, int row, Action[] actions) {
                if (actions.Length <= column * row) {
                    GUILayout.BeginVertical("box");
                    for (int i = 0; i < row; i++) {
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < column; j++) {
                            GUILayout.BeginHorizontal("box");
                            actions[i * column + j]();
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                } else {
                    Debug.LogError("Actions Array too large for the set size");
                }
            }

            protected T AddObjectField<T>(GUIContent text, T obj, bool allowSceneObjects) where T : UnityEngine.Object {
                return EditorGUILayout.ObjectField(text, obj, typeof(T), allowSceneObjects) as T;
            }
            protected T AddObjectField<T>(string text, T obj, bool allowSceneObjects) where T : UnityEngine.Object {
                return this.AddObjectField(new GUIContent(text), obj, allowSceneObjects);
            }

            protected void AddHelpBox(string content, MessageType messageType) {
                EditorGUILayout.HelpBox(content, messageType);
            }

            protected virtual void ShortcutHandling() {
                Event e = Event.current;
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.S && (e.control || e.command)) { // Ctrl + S 
                    this.SaveChanges();
                }
            }
        }
    }
}
