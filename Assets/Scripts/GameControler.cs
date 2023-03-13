using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControler : MonoBehaviour
{

	public GameObject clicker;
	private User usuario;
	private DatabaseHandler db;

	public Text marcador;
	public Text marcador2;
	public Text mensajeExito;

	public Button clikerBtn;
	public Button registerBtn;
	public Button inicioSesionBtn;
	public Button guardarBtn;

	public InputField nicknameField;
	public InputField passwordField;

	public int numeroDragones;

	public GameObject[] huevos;
	public GameObject[] textoHuevos;

	void Start()
	{
		db = GetComponent<DatabaseHandler>();
		clikerBtn.onClick.AddListener(SumarPuntos);
		registerBtn.onClick.AddListener(Registrarse);
		inicioSesionBtn.onClick.AddListener(IniciarSesion);
		guardarBtn.onClick.AddListener(Guardar);
	}

    private void Update()
    {
		SumarLevels();
    }

    void SumarPuntos()
	{
		usuario.Puntuacion++;
		marcador.text = usuario.Puntuacion.ToString();	
	}
	
	void SumarLevels()
    {
        if (usuario.Puntuacion >= 30)
        {
			huevos[0].SetActive(false);
			huevos[1].SetActive(true);
			textoHuevos[0].SetActive(false);
			textoHuevos[1].SetActive(true);
		}
		if (usuario.Puntuacion >= 60)
		{
			huevos[1].SetActive(false);
			huevos[2].SetActive(true);
			textoHuevos[1].SetActive(false);
			textoHuevos[2].SetActive(true);
		}
		if (usuario.Puntuacion >= 90)
		{
			huevos[2].SetActive(false);
			huevos[3].SetActive(true);
			textoHuevos[2].SetActive(false);
			textoHuevos[3].SetActive(true);
		}
		if (usuario.Puntuacion >= 120)
		{
			huevos[3].SetActive(false);
			huevos[4].SetActive(true);
			textoHuevos[3].SetActive(false);
			textoHuevos[4].SetActive(true);
		}
	}

	void Registrarse()
	{
		if (nicknameField.text.Equals("") || passwordField.text.Equals(""))
		{
			mensajeExito.text = "Introduce un usuario y contraseña";
			return;
		}
		bool resultado = db.Registrar(nicknameField.text, passwordField.text);
		if(resultado)
        {
			mensajeExito.text = "Usuario registrado, inicie sesión";
        }
		else
        {
			mensajeExito.text = "Usuario ya registrado";
		}

	}

	void IniciarSesion()
    {
		usuario = db.IniciarSesion(nicknameField.text, passwordField.text);
		if(usuario != null)
        {
			clicker.SetActive(true);
			huevos[1].SetActive(false);
			huevos[2].SetActive(false);
			huevos[3].SetActive(false);
			huevos[4].SetActive(false);
			textoHuevos[1].SetActive(false);
			textoHuevos[2].SetActive(false);
			textoHuevos[3].SetActive(false);
			textoHuevos[4].SetActive(false);
			mensajeExito.text = "Has iniciado sesión como " + usuario.Nombre;
			marcador.text = usuario.Puntuacion.ToString();
			//marcador2.text = usuario.Levels.ToString();
			//numeroDragones = usuario.Levels;
		}
		else
        {
			mensajeExito.text = "Usuario o contraseña incorrecto";
        }

    }

	void Guardar()
    {

		db.GuardarDatosDB(usuario);
		db.GuardarJSON(usuario);
		mensajeExito.text = "Guardado con éxito";
	}
	
}
