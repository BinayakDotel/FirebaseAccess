using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputField category_name;
    public Text Status;
    public Text SubmitStatus;
    public Text WordStatus;

    public GameObject AddCategoryUI;
    public GameObject CategoryListUI;
    public GameObject WordListUI;
    public GameObject NumberOfWordsUI;

    FirebaseAccess firebaseAccess;
    DatabaseAccess databaseAccess;

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

    public void Start()
    {
        currentCategory = "";
        Status.text = "";
        firebaseAccess = FindObjectOfType<FirebaseAccess>();
        databaseAccess = FindObjectOfType<DatabaseAccess>();
        AddCategoryUI.SetActive(false);
        CategoryListUI.SetActive(true);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);

        GenerateButton(PlayerPrefs.GetInt("IdCount"));
    }
    public void AddCategory()
    {
        AddCategoryUI.SetActive(true);
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);
    }
    public void BackToList()
    {
        AddCategoryUI.SetActive(false);
        CategoryListUI.SetActive(true);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(false);
        SubmitStatus.text = "";
        category_name.text = "";
    }
    public void LoadNumberOfWordsUI()
    {
        AddCategoryUI.SetActive(false);
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(false);
        NumberOfWordsUI.SetActive(true);
        ClearWord();
    }
    public void LoadWordListUI()
    {
        AddCategoryUI.SetActive(false);
        CategoryListUI.SetActive(false);
        WordListUI.SetActive(true);
        NumberOfWordsUI.SetActive(false);
    }
    /* Adding category to the firebase database */
    public void SaveData()
    {
        if (category_name.text.Equals(""))
        {
            SubmitStatus.text = "Enter Something";
            return;
        }
        SubmitStatus.text=firebaseAccess.Post(category_name.text.ToUpper());
    }
    public void ClearButton()
    {
        for(int i = 0; i < categoryButtonList.Count; i++)
        {
            Destroy(categoryButtonList[i]);
        }
    }
    public void ClearLocalData()
    {
        PlayerPrefs.DeleteAll();
    }
    public void ClearWord()
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            Destroy(wordList[i]);
        }
        WordStatus.text = "";
    }
    public void DrawWords(int limit)
    {
        LoadWordListUI();
        /*databaseAccess.ConnectToDatabase();
        databaseAccess.RetreiveWords(currentCategory);*/
        GenerateWordList(currentCategory, limit);

    }
    //GENERATE BUTTONS AS PER THE DATA STORE LOCALLY and ALSO UPDATES IF REFRESHED
    public void GenerateButton(int count)
    {
        ClearButton();
        Status.text = "SUCCESS";
        for (int i = 0; i < count; i++)
        {
                GameObject newButton = Instantiate(categoryButton) as GameObject;
                newButton.name = PlayerPrefs.GetString($"ID_{i}".ToString());
                Text text = newButton.GetComponentInChildren<Text>();
                text.text = newButton.name;
                newButton.GetComponent<Button>().onClick.AddListener(() => {
                    currentCategory = newButton.name;
                    LoadNumberOfWordsUI();
                });
                newButton.transform.SetParent(content.transform, true);
                categoryButtonList.Add(newButton);

                databaseAccess.ConnectToDatabase();
                databaseAccess.RetreiveWords(newButton.name);
           
        }

        Status.text = "";
    }
    public void GenerateWordList(string word, int limit)
    {
        int totalwords = PlayerPrefs.GetInt($"{currentCategory}_wordCount");
        for(int i = 1; i <= limit; i++)
        {
            GameObject wordListText = Instantiate(wordText) as GameObject;
            wordListText.GetComponent<Text>().text = PlayerPrefs.GetString($"{currentCategory}_{Random.Range(1, totalwords)}");
            wordListText.transform.SetParent(wordContent.transform, true);
            wordList.Add(wordListText);
        }   
    }
}
