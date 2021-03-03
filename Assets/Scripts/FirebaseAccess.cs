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

    public void Start()
    {
		uIManager = FindObjectOfType<UIManager>();
	}
    public string Post(string category_name)
	{
		//Checking internet connection
        if (!CheckInternet())
        {
			return "ERROR...CHECK INTERNET";
        }
		category = new Category(Random.Range(0,100), category_name);
		id.Add(category.getID());
		RestClient.Put<Category>($"{basePath}category{category.getID()}.json", category)
			.Then(
			response =>
            {
				return "DATA SUBMITTED";
			});
		return "DATA SUBMITTED";
	}
	public void GetAllCategory()
    {
		uIManager.Status.text = "LOADING...";
        if (!CheckInternet())
        {
			uIManager.Status.text = "ERROR...CHECK INTERNET";
			return;
		}
		RestClient.Get($"{basePath}.json").Then(response =>
		{
			var responseJson = response.Text;
			var data = fsJsonParser.Parse(responseJson);
			object deserialized = null;
			serializer.TryDeserialize(data, typeof(Dictionary<string, Category>), ref deserialized);

			var categories = deserialized as Dictionary<string, Category>;

			//Save Data locally
			uIManager.ClearLocalData();
			int i = 0;
			foreach(var category in categories)
            {
				//Store IDs for offline loading
				PlayerPrefs.SetString($"ID_{i}".ToString(), category.Value.name.ToString());
				PlayerPrefs.SetInt("IdCount", ++i);
			}
			//Create buttons on runtime as per the categories
			uIManager.GenerateButton(PlayerPrefs.GetInt("IdCount"));
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
}
