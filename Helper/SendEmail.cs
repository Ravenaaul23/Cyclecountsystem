using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using CycleCountSystem__CSS_.Models;

namespace CycleCountSystem__CSS_.Helper
{
     public class SendMail
    {
        string smtp = "mailrelay-internal.infineon.com";
        string fromEmail = "BTHGreenlineR@infineon.com";
        string EvriskaMail = "evriska.dayanti@infineon.com";
        //string cc1 = "eko.pribadi@infineon.com";
        //string cc2 = "pipi.aminah@infineon.com";
        //string cc3 = "malida.sinaga@infineon.com";
        //string cc4 = "wiwiek.purwanti@infineon.com";
        //string cc5 = "BTH-IFBT-RWD@infineon.com";
        string bcc5 = "Ravena.Auliya@infineon.com";

        public void SendMailToSuperior(string emailTemplate, string Name, string submitemail)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtp);
                mail.From = new MailAddress(fromEmail);
                mail.Subject = "Report: Result Cycle Count Failed";

                if (emailTemplate != null)
                {
                    mail.To.Add(submitemail);
                }
                else
                {
                    mail.To.Add(EvriskaMail);
                    mail.Subject = "Failed to send";
                }


                //}
                //mail.To.Add(EvriskaMail);
                //mail.To.Add(submitemail);
                //mail.CC.Add(cc1);
                //mail.CC.Add(cc2);
                //mail.CC.Add(cc3);
                //mail.CC.Add(cc4);
                //mail.CC.Add(cc5);
                //mail.Bcc.Add(EvriskaMail);
                mail.Bcc.Add(bcc5);


                mail.Body = emailTemplate;
                mail.IsBodyHtml = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential("OJT TEAM", "1234");

                try
                {
                    SmtpServer.Send(mail);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.StatusCode;
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    SmtpStatusCode status = ex.StatusCode;
                }
            }
            catch (Exception)
            {

            }
        }

        private bool CheckEmail(string email)
        {
            return !string.IsNullOrEmpty(email);
        }

    }
   
 }
  