using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

public class UIManager : MonoBehaviour
{
    public Text Status;
    public Text WordStatus;

    public GameObject CategoryListUI;
    public GameObject WordListUI;
    public GameObject NumberOfWordsUI;

    //Generate buttons as per the firebase category list
    public GameObject content;
    public GameObject categoryButton;

    [SerializeField]
    private List<GameObject> categoryButtonList = new List<GameObject>();

    //Generate WordList as per the database
    public GameObject wordContent;
    public GameObject wordText;

    /*Buttons to set limits to words
     * button[0] for 4 Words
     * button[0] for 6 Words
     * button[0] for 8 Words
     */
    string currentCategory;
    public Button[] button;

    [SerializeField]
    private List<GameObject> wordList = new List<GameObject>();
    public List<int> displayWord = new List<int>();

    private List<string> loadedCategories = new List<string>();
    private List<string> loadedWords = new List<string>();

    public void Start()
    {
        currentCategory = "";
        Status.text = "";
        CategoryListUI.SetActive(true);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);
    }
    public void AddCategory()
    {
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);
    }
    public void BackToList()
    {
        CategoryListUI.SetActive(true);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);
    }
    public void LoadNumberOfWordsUI()
    {
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(true);
        ClearWord();
    }
    public void LoadWordListUI()
    {
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(true);
        NumberOfWordsUI.SetActive(false);
    }
    public void ClearButton()
    {
        for(int i = 0; i < categoryButtonList.Count; i++)
        {
            Destroy(categoryButtonList[i]);
        }
        categoryButtonList.Clear();
    }
    public void ClearLocalData()
    {
        PlayerPrefs.DeleteAll();
        ClearButton();
    }
    public void ClearWord()
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            Destroy(wordList[i]);
        }
        wordList.Clear();
        WordStatus.text = "";
    }
    public void DrawWords(int limit)
    {
        LoadWordListUI();
        GenerateWordList(limit);
    }
    //GENERATE BUTTONS AS PER THE DATA STORE LOCALLY and ALSO UPDATES IF REFRESHED
    public void GenerateButton()
    {
        ClearButton();
        loadedCategories.Clear();

        string filePath = $"{Application.persistentDataPath}/wordSearch.json";

        if (!File.Exists(filePath))
        {
            print("FILE NOT FOUND!");
            return;
        }
        string data = File.ReadAllText(filePath);
        JSONObject jsonData = (JSONObject)JSON.Parse(data);

        var categories = jsonData["Catergories"];

        //int word = 0;
        foreach (var category in categories)
        {
            loadedCategories.Add(category.Key);
        }
        int count = loadedCategories.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject newButton = Instantiate(categoryButton) as GameObject;

            newButton.name = loadedCategories[i];

            //newButton.name = PlayerPrefs.GetString($"Category_{i}".ToString());
            Text text = newButton.GetComponentInChildren<Text>();
            text.text = newButton.name;
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentCategory = newButton.name;
                LoadNumberOfWordsUI();
            });
            newButton.transform.SetParent(content.transform, true);
            categoryButtonList.Add(newButton);
        }
        Status.text = "";
    }
    public void GenerateWordList(int limit)
    {
        print("GENERATE WORD");
        int totalwords = PlayerPrefs.GetInt($"{currentCategory}_wordCount");

        string filePath = $"{Application.persistentDataPath}/wordSearch.json";

        if (!File.Exists(filePath))
        {
            print("FILE NOT FOUND!");
            return;
        }
        string data = File.ReadAllText(filePath);
        JSONObject jsonData = (JSONObject)JSON.Parse(data);

        var categories = jsonData["Catergories"];

        int wordIndex = 0;
        int totalCount = PlayerPrefs.GetInt($"{currentCategory}_wordCount");
        while (wordIndex < totalCount)
        {
            var name = categories[currentCategory][wordIndex]["name"];
            loadedWords.Add(name);
            ++wordIndex;
        }

        for (int i = 1; i <= limit; i++)
        {
            GameObject wordListText = Instantiate(wordText) as GameObject;

            int index = Random.Range(1, loadedWords.Count);
            while(displayWord.Contains(index))
            {
                index = Random.Range(1, totalwords);
            }
            displayWord.Add(index);
            wordListText.GetComponent<Text>().text = loadedWords[index];
            wordListText.transform.SetParent(wordContent.transform, true);
            wordList.Add(wordListText);
        }
        displayWord.Clear();
        loadedWords.Clear();
    }
}
