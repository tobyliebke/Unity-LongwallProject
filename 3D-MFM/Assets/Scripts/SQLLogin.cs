﻿﻿using UnityEngine;
using System.Collections;
using System.Data;
using System.Data.OleDb;
//using System.Data.SqlClient;
//using System.Data.Odbc;
//using Mono.Data.Sqlite;
//using Mono.Data;

public class SQLLogin : MonoBehaviour
{

    public Terrain TerrainMain;
    string server = "localhost";
    string database = "Longwall";
    string connStatus = "";

    // Use this for initialization
    void ChangeTerrain()
    {

        //Mono.Data.Sqlite.SqliteConnection conn = new Mono.Data.Sqlite.SqliteConnection();
        bool connection;
        //CODE FOR Odbc 
        //System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection();



        //CODE FOR Odbc
        //string connectionString = "Driver={SQL Server};Server=" + server + ";Database=" + database + ";Uid=longwall_ro;Pwd=longwall_ro;";
        //CODE FOR SQL
        //string connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";User id=longwall_ro;Password=longwall_ro;";
        //SqlConnection conn = new SqlConnection(connectionString);
        string connectionString = "Provider=sqloledb;Data Source=(local);Initial Catalog=Longwall;User Id=longwall_ro;Password=longwall_ro;";
        OleDbConnection conn = new OleDbConnection(connectionString);

        //CODE FOR Odbc
        //conn.ConnectionString = connectionString;
        conn.Open();

        //IDbConnection conn = new SqlConnection();
        //conn.ConnectionString = "Data Source=localhost;Initial Catalog=Longwall;User id=longwall_ro;Password=longwall_ro;";
        //conn.Open();
        Debug.Log("Much success!");


        connection = (conn.State == ConnectionState.Open);

        if (connection)
            connStatus = "Connected";
        else
            connStatus = "Failed to connect";
        //CODE FOR Odbc
        //OdbcCommand comm2;
        //CODE FOR SQL
        //SqlCommand comm2;
        OleDbCommand comm2;
        comm2 = conn.CreateCommand();

        comm2.CommandText = "SELECT MAX(XAxis) - MIN(XAxis) AS TotalXWidth, MAX(YAxis) - MIN(YAxis) AS TotalYWidth, MIN(XAxis) AS XAxisOffset, MIN(YAxis) AS YAxisOffset FROM vwLongWall";
        //CODE FOR Odbc
        //OdbcDataReader sql_dr2;
        //CODE FOR SQL 
        //SqlDataReader sql_dr2;
        OleDbDataReader sql_dr2;
        sql_dr2 = comm2.ExecuteReader();


        int totalXWidth = 0;
        int totalYWidth = 0;

        //if (sql_dr2.HasRows)
        //{
        while (sql_dr2.Read())
        {
            totalXWidth = int.Parse(sql_dr2["TotalXWidth"].ToString());
            totalYWidth = int.Parse(sql_dr2["TotalYWidth"].ToString());
        }
        //}

        Vector3 worldSize = new Vector3(totalYWidth, 10000, totalXWidth);

        sql_dr2.Close();

        //CODE FOR Odbc
        //OdbcCommand comm;
        //CODE FOR SQL
        // SqlCommand comm;
        OleDbCommand comm;
         comm = conn.CreateCommand();

        int xRes = TerrainMain.terrainData.heightmapWidth;
        int yRes = TerrainMain.terrainData.heightmapHeight;

        Vector3 s = TerrainMain.terrainData.heightmapScale;
        Vector3 sc = new Vector3(1, 10000, 1);

        comm.CommandText = "SELECT  RowLabel, [XAxis], [YAxis], FloorHeight, RowColor FROM vwLongwall ORDER BY XAxis, YAxis";

        //CODE FOR Odbc
        //OdbcDataReader sql_dr;
        //CODE FOR SQL
        //SqlDataReader sql_dr;
        OleDbDataReader sql_dr;

        sql_dr = comm.ExecuteReader();

        string RowColor;

        float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, xRes, yRes);
        float[, ,] alphas = TerrainMain.terrainData.GetAlphamaps(0, 0, TerrainMain.terrainData.alphamapWidth, TerrainMain.terrainData.alphamapHeight);

        for (int x = 0; x < TerrainMain.terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < TerrainMain.terrainData.alphamapHeight; y++)
            {
                alphas[x, y, 0] = 0;
                alphas[x, y, 1] = 0;
                alphas[x, y, 2] = 0;
                alphas[x, y, 3] = 0;
            }
        }

        int xAxis;
        int yAxis;
        float floorHeight;


        //if (sql_dr.HasRows)
        //{

        while (sql_dr.Read())
        {
            xAxis = int.Parse(sql_dr["XAxis"].ToString());
            yAxis = int.Parse(sql_dr["YAxis"].ToString());
            
            floorHeight = float.Parse(sql_dr["FloorHeight"].ToString());


            heights[xAxis, yAxis] = floorHeight; //0 - 1. 1 being the maximum possible height

            RowColor = sql_dr["RowColor"].ToString().Trim();


            switch (RowColor)
            {
                case "Red":
                    alphas[xAxis, yAxis, 0] = 0;
                    alphas[xAxis, yAxis, 1] = 0;
                    alphas[xAxis, yAxis, 2] = 1;
                    alphas[xAxis, yAxis, 3] = 0;
                    break;
                case "Yellow":
                    alphas[xAxis, yAxis, 0] = 0;
                    alphas[xAxis, yAxis, 1] = 1;
                    alphas[xAxis, yAxis, 2] = 0;
                    alphas[xAxis, yAxis, 3] = 0;
                    break;
                case "Green":
                    alphas[xAxis, yAxis, 0] = 1;
                    alphas[xAxis, yAxis, 1] = 0;
                    alphas[xAxis, yAxis, 2] = 0;
                    alphas[xAxis, yAxis, 3] = 0;
                    break;
                case "Grey":
                    alphas[xAxis, yAxis, 0] = 0;
                    alphas[xAxis, yAxis, 1] = 0;
                    alphas[xAxis, yAxis, 2] = 0;
                    alphas[xAxis, yAxis, 3] = 1;
                    break;

            }

        }

        //}

        TerrainMain.terrainData.SetHeights(0, 0, heights);
        TerrainMain.terrainData.SetAlphamaps(0, 0, alphas);

        sql_dr.Close();


        conn.Close();


    }
    
}