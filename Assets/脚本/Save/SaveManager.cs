using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;
    [SerializeField] private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();

    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
    }


    private void Start()
    {
        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No save data found, creating new game");
            NewGame();
        }
        if (gameData.skillTree == null)
            gameData.skillTree = new SerializableDictionary<string, bool>();
        if (gameData.inventory == null)
            gameData.inventory = new SerializableDictionary<string, int>();
        if (gameData.equipmentId == null)
            gameData.equipmentId = new List<string>();
        if (gameData.checkpoints == null)
            gameData.checkpoints = new SerializableDictionary<string, bool>();
        if (gameData.volumeSettings == null)
            gameData.volumeSettings = new SerializableDictionary<string, float>();
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        saveManagers = FindAllSaveManagers();

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        List<ISaveManager> saveManagers = new List<ISaveManager>();
        foreach (var manager in FindObjectsOfType<MonoBehaviour>())
        {
            if (manager is ISaveManager saveManager)
            {
                saveManagers.Add(saveManager);
            }
        }
        return saveManagers;
    }

    public bool HasSavedData()
    {
        if (dataHandler == null)
            return false;

        if (dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }
}
