using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Reflection.Emit;
using PDF_Generator;
using UnityEngine.UI;

public class Sender : MonoBehaviour
{  
	[SerializeField] private Text _logText;
    [SerializeField] private InputField _emailFromSend;
    [SerializeField] private InputField _passwordField;

    [SerializeField] private InputField _emailToSend;
    [SerializeField] private InputField _subjectField;
    [SerializeField] private InputField _bodyField;

    [SerializeField] private Button _generatePdfButton;
    [SerializeField] private Button _sendEmailButton;

    private PdfCreator _pdfCreator;
    private string _pdfPath;

    private void Start()
    {
        _generatePdfButton.onClick.AddListener(GeneratePdf);
        _sendEmailButton.onClick.AddListener(SendEmail);

        _pdfCreator = new PdfCreator();
        _pdfCreator.CreatePdf(OnPdfCreate);
    }

    private void OnDestroy()
    {
        _generatePdfButton.onClick.RemoveAllListeners();
        _sendEmailButton.onClick.RemoveAllListeners();
    }

    private void GeneratePdf()
    {
        _pdfCreator.CreatePdf(OnPdfCreate);
    }

    private void OnPdfCreate(string pdfPath)
    {
        _pdfPath = pdfPath;
    }
	
	private void SendEmail ()
	{
		MailMessage mail = new MailMessage();

		mail.From = new MailAddress(_emailFromSend.text);
		mail.To.Add(_emailToSend.text);
		mail.Subject = _subjectField.text;
		mail.Body = _bodyField.text;

        if(!string.IsNullOrEmpty(_pdfPath))
		    mail.Attachments.Add (new Attachment (_pdfPath));

		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com")
		{
			Port = 587,
			Credentials = new NetworkCredential(_emailFromSend.text, _passwordField.text) as ICredentialsByHost,
			EnableSsl = true
		};
		ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

		try
		{
			smtpServer.Send(mail);

			_logText.text = "Email Was send " + " subject " + _subjectField.text + " body " + _bodyField.text;
		}
		catch (Exception e)
		{
			Debug.Log(e.GetBaseException());
			_logText.text = "Error " + e.GetBaseException ().ToString();
		}
	}
}
