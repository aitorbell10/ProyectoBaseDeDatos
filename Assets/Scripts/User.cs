using System;

/// <summary>
/// Script creado para acceder más rápido desde otros scripts, se llama serialize.
/// </summary>

[Serializable]
public class User
{
    public int ID;
    public string Nombre;
    public string Contraseña;
    public int Puntuacion;
    //public int Levels;

    public User(int id, string nombre, string contraseña, int puntuacion)
    {
        this.ID = id;
        this.Nombre = nombre;
        this.Contraseña = contraseña;
        this.Puntuacion = puntuacion;
        //this.Levels = levels;
    }
}
