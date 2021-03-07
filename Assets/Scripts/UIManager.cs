using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        print("BUTTONS GENERATED");
        ClearButton();
        int count = PlayerPrefs.GetInt("totalCategory");
        for (int i = 1; i <= count; i++)
        {
                GameObject newButton = Instantiate(categoryButton) as GameObject;
                newButton.name = PlayerPrefs.GetString($"Category_{i}".ToString());
                Text text = newButton.GetComponentInChildren<Text>();
                text.text = newButton.name;
                newButton.GetComponent<Button>().onClick.AddListener(() => {
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
        int totalwords = PlayerPrefs.GetInt($"{currentCategory}_wordCount");
        for(int i = 1; i <= limit; i++)
        {
            GameObject wordListText = Instantiate(wordText) as GameObject;

            int index = Random.Range(1, totalwords);
            while(displayWord.Contains(index))
            {
                index = Random.Range(1, totalwords);
            }
            displayWord.Add(index);
            wordListText.GetComponent<Text>().text = PlayerPrefs.GetString($"{currentCategory}_word_{index}");
            //wordListText.GetComponent<Text>().text = index.ToString();
            wordListText.transform.SetParent(wordContent.transform, true);
            wordList.Add(wordListText);
        }
        displayWord.Clear();
    }
}
