# Minigame2-Library
Mini  game 2 for game design 2 class. 
Assets/
├── Scenes/
│   ├── Start.unity
│   ├── Library.unity
│   ├── Door.unity
│   ├── BadEnd.unity
│   └── GoodEnd.unity
├── Scripts/
│   ├── GameManager.cs         // Global manager, stores variables across loops
│   ├── ChoiceHandler.cs       // Input logic (short/long space press)
│   └── StoryController.cs     // Story branch controller
├── UI/
│   ├── ChoicePanel.prefab     // Option panel (TextMeshPro + progress bar)
│   ├── EndPanel.prefab        // Ending screen
│   └── ... (other UI assets)
├── Art/
│   ├── Backgrounds/           // Background images
│   ├── Characters/            // Character illustrations
│   └── Endings/               // BAD END illustrations
├── Audio/
│   ├── SFX/                   // Sound effects
│   └── BGM/                   // Background music
└── Resources/
    └── ... (optional, for dynamically loaded assets)