using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEditor;
using UnityEngine.UI;
using FullSerializer;

public class FirebaseAccess : MonoBehaviour
{
    private readonly string basePath = "https://unityprojects-bd0f1-default-rtdb.firebaseio.com/Category/";

	Category category;
	private List<int> id = new List<int>();
	private static fsSerializer serializer = new fsSerializer();

	private UIManager uIManager;

	//Checking data update in firebase
	private float actualTime;
	private float timeInterval;
	private bool updatedOnce = false;

    public void Start()
    {
		timeInterval = 10.0f;
		actualTime = timeInterval;
		uIManager = FindObjectOfType<UIManager>();
		GetAllCategory();
	}
    /*public string Post(string category_name)
	{
		//Checking internet connection
        if (!CheckInternet())
        {
			return "ERROR...CHECK INTERNET";
        }
		category = new Category(category_name);
		category.AddWord("Nepal".ToUpper());

		//id.Add(category.getID());
		RestClient.Put<Category>($"{basePath}{category.GetCategoryName()}.json", category)
			.Then(
			response =>
            {
				return "DATA SUBMITTED";
			});
		return "DATA SUBMITTED";
	}*/
    private void Update()
    {
		if (CheckInternet())
		{
            if (!updatedOnce)
            {
				GetAllCategory();
				updatedOnce = true;
            }
            else
            {
				actualTime = actualTime - Time.deltaTime;

				if (actualTime <= 0.0f)
				{
					GetAllCategory();
					actualTime = timeInterval;
				}
			}
		}
		else print("NOT WORKING");
	}
    public void GetAllCategory()
    {
		uIManager.Status.text = "LOADING...";
        if (!CheckInternet())
        {
			//uIManager.Status.text = "ERROR...CHECK INTERNET";
			StartCoroutine(NoInternetMessage(2));
			return;
		}
		StartCoroutine(NoInternetMessage(0));
		RestClient.Get($"{basePath}.json").Then(response =>
		{
			var responseJson = response.Text;
			var data = fsJsonParser.Parse(responseJson);
			object deserialized = null;
			serializer.TryDeserialize(data, typeof(Dictionary<string, Category>), ref deserialized);

			var categories = deserialized as Dictionary<string, Category>;

			int categoryCount = 0;
			foreach (var category in categories)
            {
				PlayerPrefs.SetString($"Category_{categoryCount}", category.Value.name);
				PlayerPrefs.SetInt($"categoryCount", ++categoryCount);
				PlayerPrefs.SetInt($"{category.Value.name}_wordCount", category.Value.wordCount);

				for (int i = 0; i < category.Value.wordCount; i++)
				{
					PlayerPrefs.SetString($"{category.Value.name}_{i}", category.Value.words[i]);
				}
			}
			//Create buttons on runtime as per the categories
			uIManager.GenerateButton(PlayerPrefs.GetInt($"categoryCount"));
		});
    }
	/*
	 * Check whether internet is connected or not 
	 * returns true if connected
	 */
	public bool CheckInternet()
    {
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Debug.Log("ERROR...CHECK INTERNET");
			return false;
		}
		return true;
	}
	IEnumerator NoInternetMessage(int time)
    {
		yield return new WaitForSeconds(time);
		if (PlayerPrefs.GetInt($"categoryCount") == 0) uIManager.Status.text = "ERROR! CHECK INTERNET CONNECTION";
		else uIManager.Status.text = "";
    }
}
