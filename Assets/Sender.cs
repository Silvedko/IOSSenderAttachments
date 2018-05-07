using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using UnityEngine.UI;

public class Sender : MonoBehaviour 
{
	[SerializeField] private Text _logText;
	[SerializeField] 

	private void Awake ()
	{
		Application.logMessageReceived += HandleLog;

	}

	private void HandleLog (string condition, string stacktrace, LogType logType)
	{

	}

	private void OnGUI () 
	{
		if (GUI.Button (new Rect (10, 10, 150, 100), "Send")) 
		{
			SendEmail();
		}
	}
	
	private void SendEmail ()
	{
		MailMessage mail = new MailMessage();

		mail.From = new MailAddress("silvedko@gmail.com");
		mail.To.Add("o.silvestrov@ameria.de");
		mail.Subject = "Test";
		mail.Body = "Body";
		//		mail.Attachments.Add (new Attachment (FilePath));

		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com")
		{
			Port = 587,
			Credentials = new NetworkCredential("silvedko@gmail.com", "Si1vedk0!") as ICredentialsByHost,
			EnableSsl = true
		};
		ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
		{
			return true;
		};

		try
		{
			smtpServer.Send(mail);

			_logText.text = "Email Was send";
		}
		catch (Exception e)
		{
			Debug.Log(e.GetBaseException());
			_logText.text = "Error " + e.GetBaseException ().ToString();
		}
	}
}
