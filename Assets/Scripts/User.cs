using System;

[Serializable]
public class User
{
    public int ID;
    public string Nombre;
    public string Contrase�a;
    public int Puntuacion;
    //public int Levels;

    public User(int id, string nombre, string contrase�a, int puntuacion)
    {
        this.ID = id;
        this.Nombre = nombre;
        this.Contrase�a = contrase�a;
        this.Puntuacion = puntuacion;
        //this.Levels = levels;
    }
}
