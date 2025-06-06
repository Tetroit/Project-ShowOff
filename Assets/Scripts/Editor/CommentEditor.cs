using UnityEngine;
using System.Collections;
using UnityEditor;

/**
 * Defines the editor for the Note component which is 
 * a simple sticky note like component through which you
 * can document your scene.
 */
[CustomEditor(typeof(Comment))]
public class CommentEditor : Editor {

    private static bool _backgroundTexturesInitialized = false;
    private static string[] _descriptions = { "Documentation", "Todo", "Nice to have", "Minor bug", "Critical bug" };
    private static Color[] _colors = {
        new Color(1, 1, 0.4f),
        new Color(1, 0.75f, 0.3f),
        new Color(1, 0.5f, 0.2f),
        new Color(1, 0.25f, 0.1f),
        new Color(1, 0, 0)

    };

    private static GUIStyle[] _noteStyles = null;

    private static GUILayoutOption[] TEXT_AREA_OPTIONS = {
        GUILayout.ExpandWidth (true), 
        GUILayout.ExpandHeight(false)
    };

    static void generateBackgroundTexturesForDescriptions ()
    {
        if (_backgroundTexturesInitialized) return;

        _noteStyles = new GUIStyle[_descriptions.Length];
        _noteStyles = new GUIStyle[_descriptions.Length];

        for (int i = 0; i < _descriptions.Length; i++)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, _colors[i]);
            texture.Apply();

            GUIStyle style = new GUIStyle();
            style.normal.background = texture;
            style.wordWrap = true;
            style.fontSize = 12;
            style.padding = new RectOffset(5, 5, 5, 5);
            style.margin = new RectOffset(5, 5, 5, 5);

            _noteStyles[i] = style;
        }
    }

    private Comment _note;

    private void OnEnable ()
	{
        generateBackgroundTexturesForDescriptions();
        _note = target as Comment;
    }

    public override void OnInspectorGUI()
    {
        _note.noteType = EditorGUILayout.Popup(_note.noteType, _descriptions);
        _note.noteText = EditorGUILayout.TextArea(_note.noteText, _noteStyles[_note.noteType], TEXT_AREA_OPTIONS);
    }


}

