using System;
using System.Net.Mail;

namespace CycleCountSystem__CSS_.Helper
{
    public class SendAlarm
    {
        string smtp = "mailrelay-internal.infineon.com";
        string fromEmail = "BTHGreenlineR@infineon.com";
        string EvriskaMail = "evriska.dayanti@infineon.com";
        string bcc5 = "Ravena.Auliya@infineon.com";

        public void SendAlarmToSuperior(string emailTemplate, string Name, string submitemail)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtp);
                mail.From = new MailAddress(fromEmail);
                mail.Subject = "Reminder!! Cyclecount Schedule Has Arrived";

                if (emailTemplate != null)
                {
                    mail.To.Add(submitemail);
                }
                else
                {
                    mail.To.Add(EvriskaMail);
                    mail.Subject = "Failed to send";
                }

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
