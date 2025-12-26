# Unity-IAassistant
Un assistant de débogage intelligent pour Unity intégrant n8n et Docker | Intelligent AI-powered debugging assistant for Unity using n8n and Docker.


# 🛡️ Ange Gardien IA - Assistant de Débogage Temps Réel pour Unity

[![Unity](https://img.shields.io/badge/Unity-2022.3+-black?logo=unity)](https://unity.com/)
[![n8n](https://img.shields.io/badge/n8n-Workflow_Automation-orange?logo=n8n)](https://n8n.io/)


[![Docker](https://img.shields.io/badge/Docker-Container-blue?logo=docker)](https://www.docker.com/)

**Ange Gardien IA** est un outil de productivité pour développeurs Unity. Il capture les erreurs de la console en temps réel et utilise un agent IA (via n8n) pour fournir des analyses contextuelles et des solutions immédiates directement dans l'éditeur.

---

## 🚀 Fonctionnalités
- **Capture Automatique** : Intercepte les `Exceptions` et `Errors` dès qu'elles surviennent.
- **Analyse Zen** : Fournit une explication concise (Satori) et une solution technique propre.
- **Architecture Découplée** : Utilise n8n comme moteur de workflow pour une flexibilité totale sur le modèle d'IA utilisé (Grok, OpenAI, local LLM).
- **Interface Intégrée** : Fenêtre dédiée dans l'éditeur Unity pour centraliser les conseils de l'IA.

---

## 🏗️ Architecture Technique
Le projet repose sur une communication triangulaire :
1. **Unity Editor** : Script C# (EditorWindow) qui surveille les logs et envoie des requêtes POST JSON.
2. **n8n (Docker)** : Reçoit le Webhook, traite la stacktrace et interroge l'IA avec un prompt système optimisé.
3. **Agent IA** : Analyse l'erreur et renvoie une réponse structurée au format texte/markdown.

---

## 🛠️ Installation

### 1. Backend (n8n & Docker)
1. Lancez n8n via Docker :
   ```bash
   docker run -it --rm --name n8n -p 5678:5678 n8nio/n8n

   Importez le workflow fourni (workflow_n8n.json) dans votre instance n8n.

Activez le Webhook de production.

2. Frontend (Unity)
Copiez le dossier AIAssistant dans le répertoire Assets/Editor/ de votre projet.

Ouvrez la fenêtre via le menu : Mon Assistant > Activer l'Ange Gardien.

Renseignez l'URL de votre Webhook n8n.

📖 Utilisation
Lorsqu'une erreur survient, l'assistant affiche :

Analyse : Pourquoi l'erreur est apparue.

Solution : Un bloc de code prêt à être copié-collé.

Prévention : Un conseil d'expert pour améliorer la stabilité du code.

🛠️ Roadmap / Évolutions futures
[ ] Scene Context : Envoyer la hiérarchie de la scène pour un diagnostic plus précis.

[ ] Quick Fix : Application automatique des corrections de code en un clic.

[ ] History Log : Archivage des solutions proposées durant la session.

---

👨‍💻 Auteur:
aminata-sane

[Mon LinkedIn](https://www.linkedin.com/in/aminata-constance-san%C3%A9-82897a33a/?originalSubdomain=fr) - [Mon Portfolio](https://aminata-constance-sane.students-laplateforme.io/)

---
