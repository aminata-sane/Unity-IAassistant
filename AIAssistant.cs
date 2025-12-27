using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class AIAssistant : EditorWindow
{
    // URL DE TEST : Celle que tu utilises actuellement
    public string n8nUrl = "http://localhost:5678/webhook-test/aide-unity"; 

    [MenuItem("Mon Assistant/Activer l'Ange Gardien")]
    public static void ShowWindow() => GetWindow<AIAssistant>("Ange Gardien IA");

    void OnEnable() { 
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog; 
    }

    void OnDisable() { 
        Application.logMessageReceived -= HandleLog; 
    }

    void OnGUI() {
        GUILayout.Label("Statut : Mode Test Actif", EditorStyles.boldLabel);
        n8nUrl = EditorGUILayout.TextField("URL Webhook Test :", n8nUrl);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Forcer une erreur de test", GUILayout.Height(30))) {
            Debug.LogError("Erreur de test : La variable 'vitesse' n'est pas assignée.");
        }
        
        EditorGUILayout.HelpBox("IMPORTANT : Avant de tester, cliquez sur 'Execute Workflow' dans n8n (le bouton orange).", MessageType.Warning);
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        // Sécurité anti-boucle infinie
        if (type != LogType.Error && type != LogType.Exception) return;
        if (logString.Contains("CONTREMAÎTRE IA") || logString.Contains("n8n")) return;

        StaticEditorCoroutine.Start(SendErrorToN8N(logString, stackTrace));
    }

    IEnumerator SendErrorToN8N(string error, string trace) {
        if (string.IsNullOrEmpty(n8nUrl)) {
            Debug.LogWarning("[n8n] URL non définie ! Vérifiez le champ dans la fenêtre de l'Ange Gardien.");
            yield break;
        }
        
        ErrorData data = new ErrorData { message = error, details = trace };
        string json = JsonUtility.ToJson(data);
        
        Debug.Log($"<color=yellow>[n8n] Envoi de l'erreur à {n8nUrl}</color>");
        
        UnityWebRequest request = new UnityWebRequest(n8nUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.SendWebRequest();
        while (!request.isDone) {
            yield return null;
        }

        Debug.Log($"<color=yellow>[n8n] Résultat: {request.result}, Code: {request.responseCode}, Erreur: '{request.error}'</color>");

        // ANALYSE PRÉCISE
        if (request.result == UnityWebRequest.Result.Success) {
            string response = request.downloadHandler.text;
            if (!string.IsNullOrEmpty(response)) {
                Debug.Log("<color=#00e6ff><b>[CONTREMAÎTRE IA] :</b></color> " + response);
            } else {
                Debug.Log("<color=orange>[INFO] n8n a reçu l'erreur mais a répondu VIDE. Vérifiez le texte dans le nœud Respond to Webhook.</color>");
            }
        } else {
            // On affiche le CODE d'erreur pour comprendre
            Debug.LogWarning($"[Alerte n8n] Code: {request.responseCode} | Erreur: {request.error}");
        }
        
        request.Dispose();
    }

    [System.Serializable] 
    public class ErrorData { public string message; public string details; }
}

public static class StaticEditorCoroutine {
    public static void Start(IEnumerator coroutine) {
        EditorApplication.CallbackFunction callback = null;
        callback = () => { 
            if (!coroutine.MoveNext()) {
                EditorApplication.update -= callback; 
            }
        };
        EditorApplication.update += callback;
    }
}