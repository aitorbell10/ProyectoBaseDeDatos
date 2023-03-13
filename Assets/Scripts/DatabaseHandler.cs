using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Librerías que necesitamos
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System.Security.Cryptography;

/// <summary>
/// Script creado para hacer una base de datos y el json, donde también se encripta y desencripta el código y la información.
/// </summary>

public class DatabaseHandler : MonoBehaviour
{
    //Variable para controlar la ruta de la base de datos, constructor de la ruta, y el nombre de la base de datos
    string rutaDB;
    string strConexion;
    string DBFileName = "UserData.db";

    //Variable para trabajar con las conexiones
    IDbConnection dbConnection;

    //Para poder ejecutar comandos
    IDbCommand dbCommand;

    //Variable para leer
    IDataReader reader;

    // Start is called before the first frame update
    void Start()
    {
        SetupDB();

    }

    //Método para abrir la DB
    void AbrirDB()
    {
        // Crear y abrir la conexión
        // Comprobar en que plataforma estamos
        // Si es el Editor de Unity mantenemos la ruta
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si estamos en PC
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si es Android
        else if (Application.platform == RuntimePlatform.Android)
        {
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            // Comprobar si el archivo se encuentra almecenado en persistant data
            if (!File.Exists(rutaDB))
            {
                // Almaceno el archivo en load db
                WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);
                while (!loadDB.isDone)
                {
                }

                // Copio el archivo a persistant data
                File.WriteAllBytes(rutaDB, loadDB.bytes);
            }
        }

        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    void SetupDB()
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = @"CREATE TABLE IF NOT EXISTS User(  
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            nickname VARCHAR(255) UNIQUE NOT NULL,
            password VARCHAR(255) NOT NULL,
            score INTEGER NOT NULL DEFAULT '0',
            levels INTEGER NOT NULL DEFAULT '0'
            )";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
        //Cerramos la DB
        CerrarDB();
    }

    public User IniciarSesion(string nickname, string password)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = string.Format("SELECT * FROM User WHERE nickname = \"{0}\" AND password = \"{1}\"", nickname,
            password);
        dbCommand.CommandText = sqlQuery;

        // Leer la base de datos
        reader = dbCommand.ExecuteReader();
        // Si no se ha encontrado un usuario con el nick y contraseña dados devolver null
        if (!reader.Read()) return null;
        // Creamos el objeto usuario si se ha recuperado de la base de datos
        var user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
        reader.Close();
        reader = null;
        //Cerramos la DB
        CerrarDB();
        return user;
    }

    public bool Registrar(string nickname, string password)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO User(nickname, password) values(\"{0}\",\"{1}\")", nickname,
            password);
        dbCommand.CommandText = sqlQuery;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }

        //Cerramos la DB
        CerrarDB();

        return true;
    }

    public bool GuardarDatosDB(User user)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("UPDATE User SET score = \"{0}\" WHERE id = \"{1}\"",
            user.Puntuacion,
            user.ID);
        //string sqlQuery2 = String.Format("UPDATE User SET levels = \"{0}\" WHERE id = \"{1}\"",
        //user.Levels,
        //user.ID);

        dbCommand.CommandText = sqlQuery;
        //dbCommand.CommandText = sqlQuery2;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }

        //Cerramos la DB
        CerrarDB();

        return true;
    }



    public void GuardarJSON(User user)
    {
        string json = JsonUtility.ToJson(user, true);
        byte[] JasonEcriptado = Encrypt(json);

        StreamWriter writer = new StreamWriter(Application.dataPath + "/JsonGuardado/" + user.Nombre, false);
        writer.BaseStream.Write(JasonEcriptado, 0, JasonEcriptado.Length);
        writer.Close();
    }

    byte[] _key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    byte[] _inicializationVector = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

    byte[] Encrypt(string message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform encryptor = aes.CreateEncryptor(_key, _inicializationVector);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);

        streamWriter.WriteLine(message);

        streamWriter.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }

    string Decrypt(byte[] message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform decrypter = aes.CreateDecryptor(_key, _inicializationVector);

        MemoryStream memoryStream = new MemoryStream(message);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        string decryptedMessage = streamReader.ReadToEnd();

        memoryStream.Close();
        cryptoStream.Close();
        streamReader.Close();

        return decryptedMessage;
    }



    //Método para cerrar la DB
    void CerrarDB()
    {
        // Cerrar las conexiones
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }




}