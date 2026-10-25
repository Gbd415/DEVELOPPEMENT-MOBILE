MONGO BIDJOKA GABRIEL FRANKLIN-24G01114
# EduCam — Quiz scolaire hors ligne

Application mobile .NET MAUI permettant aux élèves de s’auto-évaluer **sans connexion Internet** et aux enseignants de créer leurs propres questionnaires.

## Fonctionnalités

- **Mode Élève**
  - Questionnaires préchargés (Maths, Français, Histoire)
  - Passage du quiz avec score final
  - Messages d’encouragement selon le résultat
  - Historique local des scores

- **Mode Enseignant**
  - Création de nouveaux questionnaires
  - Ajout de questions (jusqu’à 4 options)
  - Suppression des quiz personnalisés (les quiz par défaut sont protégés)

- **100 % hors ligne**
  - Stockage SQLite local
  - Aucune connexion Internet requise

## Comment ouvrir et compiler

### Prérequis
- Visual Studio 2022 (17.8+) ou Visual Studio Code + .NET 9 SDK
- Workload **.NET Multi-platform App UI development** installé

### Étapes
1. Ouvre le fichier `EduCam.sln` avec Visual Studio
2. Restaure les packages NuGet (automatique)
3. Choisis une cible (Android Emulator, Windows Machine, iOS Simulator…)
4. Appuie sur **F5** ou clique sur le bouton Play

### Première exécution
Au premier lancement, 3 questionnaires par défaut sont automatiquement créés :
- Mathématiques - Niveau 1
- Français - Grammaire
- Histoire - Afrique

## Structure du projet

```
EduCam/
├── Models/           → Question, Quiz, QuizResult
├── Services/         → DatabaseService (SQLite)
├── ViewModels/       → Home, Quiz, Teacher, History
├── Views/            → Pages XAML
├── Converters/       → Convertisseurs de valeur
└── Resources/        → Styles, icônes, splash
```

## Technologies
- .NET 9 + MAUI
- SQLite (sqlite-net-pcl)
- MVVM simple (sans CommunityToolkit pour rester léger)


