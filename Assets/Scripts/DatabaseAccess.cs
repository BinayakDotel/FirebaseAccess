using System;
using System.Collections.Generic;
using UnityEngine;

using MySql.Data.MySqlClient;

public class DatabaseAccess: MonoBehaviour
{
    private const string SERVER = "localhost";
    private const string DATABASE = "words";
    private const string USER_ID = "root";
    private const string PASSWORD = "";
    private const int PORT = 3306;

    private string basePath;

    private UIManager uIManager;

    MySqlConnection connection;
    MySqlCommand cmd;
    private MySqlDataReader rdr;

    private void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
    }
    public void ConnectToDatabase()
    {
        basePath = BuildConnectionString();
        try
        {
            connection = new MySqlConnection(basePath);
            connection.Open();
            print($"STATUS: {connection}");
            print("CONNECTION ESTABLISHED");
        }
        catch
        {
            print($"ERROR...Check Internet Connection");
        }
    }
    public void RetreiveWords(string category)
    {
        int wordCount = 0;
        try
        {
            string sql = $"SELECT * FROM {category}";
            cmd = new MySqlCommand(sql, connection);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                PlayerPrefs.SetString($"{category}_{rdr[0]}", rdr[1].ToString().ToUpper());
                PlayerPrefs.SetInt($"{category}_wordCount", ++wordCount);
            }

            rdr.Close();
            connection.Close();
        }
        catch
        {
            print($"COULDNOT FIND WORD");
        }
    }
    private string BuildConnectionString()
    {
        return $" SERVER = {SERVER}; DATABASE = {DATABASE}; PORT = {PORT}; " +
               $" USER ID = {USER_ID}; PASSWORD = {PASSWORD}; ";
    }
}
