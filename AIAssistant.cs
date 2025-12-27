using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class AIAssistant : EditorWindow
{
    public string n8nUrl = "http://localhost:5678/webhook-test/aide-unity"; 
    private string lastAiResponse = "En attente d'une erreur pour analyse...";
    private Vector2 scrollPos;
    private bool isProcessing = false;

    [MenuItem("Mon Assistant/Ange Gardien IA")]
    public static void ShowWindow() => GetWindow<AIAssistant>("Ange Gardien IA");

    void OnEnable() { 
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog; 
    }

    void OnDisable() { Application.logMessageReceived -= HandleLog; }

    void OnGUI() {
        // --- HEADER ---
        EditorGUILayout.Space(10);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15, alignment = TextAnchor.MiddleCenter };
        headerStyle.normal.textColor = new Color(0.0f, 0.9f, 1.0f);
        GUILayout.Label("🛡️ ANGE GARDIEN : MAÎTRE ZEN", headerStyle);
        EditorGUILayout.Space(10);

        // --- CONFIGURATION ---
        EditorGUILayout.BeginVertical("box");
        n8nUrl = EditorGUILayout.TextField("URL Webhook :", n8nUrl);
        if (isProcessing) EditorGUILayout.HelpBox("L'IA réfléchit...", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // --- AFFICHAGE DE LA RÉPONSE ---
        GUILayout.Label("Conseil de l'IA :", EditorStyles.boldLabel);
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        
        GUIStyle responseStyle = new GUIStyle(EditorStyles.helpBox);
        responseStyle.fontSize = 13;
        responseStyle.richText = true;
        responseStyle.padding = new RectOffset(10, 10, 10, 10);

        // Affichage de la réponse avec une couleur différente pour le texte
        EditorGUILayout.TextArea(lastAiResponse, responseStyle);
        
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        // --- BOUTON DE TEST ---
        if (GUILayout.Button("Forcer une erreur de test", GUILayout.Height(30))) {
            Debug.LogError("MissingComponentException: Rigidbody manquant sur le Player.");
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        if (type != LogType.Error && type != LogType.Exception) return;
        if (logString.Contains("CONTREMAÎTRE") || logString.Contains("n8n")) return;

        lastAiResponse = "🧘 Le Maître Zen analyse votre erreur...";
        StaticEditorCoroutine.Start(SendErrorToN8N(logString, stackTrace));
    }

    IEnumerator SendErrorToN8N(string error, string trace) {
        isProcessing = true;
        ErrorData data = new ErrorData { message = error, details = trace };
        string json = JsonUtility.ToJson(data);
        
        UnityWebRequest request = new UnityWebRequest(n8nUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            lastAiResponse = request.downloadHandler.text;
            Repaint(); // Force la mise à jour de la fenêtre
        } else {
            lastAiResponse = "<color=red>Erreur de connexion avec n8n.</color>\n" + request.error;
        }
        
        isProcessing = false;
        request.Dispose();
    }

    [System.Serializable] 
    public class ErrorData { public string message; public string details; }
}

// Classe Coroutine inchangée