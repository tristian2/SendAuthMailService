using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SendAuthMailService.Models;
using System.Net.Mail;
using System.IO;

namespace SendAuthMailService.Controllers
{
    public class EmailController : ApiController
    {
       

        [HttpPost]
        [ActionName("sendmail")]
        //[Route("sendmail")]
        public IHttpActionResult processAuthEmail(SendMailRequest mailModel)
        {
           
            // Send the email
            System.Net.Mail.MailMessage msg = new MailMessage();

            // Separate the recipient array
            string[] emailAddress = mailModel.recipient.Split(',');

            foreach (string currentEmailAddress in emailAddress)
            {
                msg.To.Add(new MailAddress(currentEmailAddress.Trim()));
            }


            // Separate the cc array , if not null
            string[] ccAddress = null;

            if (mailModel.cc != null)
            {
                ccAddress = mailModel.cc.Split(',');
                foreach (string currentCCAddress in ccAddress)
                {
                    msg.CC.Add(new MailAddress(currentCCAddress.Trim()));
                }
            }


            // Include the reply to if not null
            if (mailModel.replyto != null)
            {
                msg.ReplyToList.Add(new MailAddress(mailModel.replyto));
            }


            // Include the file attachment if the filename is not null
            if (mailModel.filename != null)
            {
                // Declare a temp file path where we can assemble our file
                string tempPath = Properties.Settings.Default["TempFile"].ToString();

                string filePath = Path.Combine(tempPath, mailModel.filename);

                using (System.IO.FileStream reader = System.IO.File.Create(filePath))
                {
                    byte[] buffer = Convert.FromBase64String(mailModel.filecontent);
                    reader.Write(buffer, 0, buffer.Length);
                    reader.Dispose();
                }

                msg.Attachments.Add(new Attachment(filePath));

            }

            string sendFromEmail = Properties.Settings.Default["SendFromEmail"].ToString();
            string sendFromName = Properties.Settings.Default["SendFromName"].ToString();
            string sendFromPassword = Properties.Settings.Default["SendFromPassword"].ToString();

            msg.From = new MailAddress(sendFromEmail, sendFromName);
            msg.Subject = mailModel.subject;
            msg.Body = mailModel.body;
            //msg.IsBodyHtml = true;
            msg.IsBodyHtml = false;
            msg.Headers.Add("Content-Type", "text/plain"); //tris comment - for supportworks, set this!


            //SmtpClient client = new SmtpClient("smtp.office365.com");
            //client.Port = 587;
            SmtpClient client = new SmtpClient("localhost");  //for testing, we run smtp4dev
            //client.Port = 25;
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //NetworkCredential cred = new System.Net.NetworkCredential(sendFromEmail, sendFromPassword);
            //client.Credentials = cred;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;




            try
            {
                client.Send(msg);
                msg.Dispose();

                // Clean up the temp directory if used
                if (mailModel.filename != null)
                {
                    string tempPath = Properties.Settings.Default["TempFile"].ToString();
                    string filePath = Path.Combine(tempPath, mailModel.filename);
                    File.Delete(filePath);
                }

                return Ok("Mail Sent");
            }
            catch (SmtpException smtpe)
            {
                //Console. smtpe.InnerException.ToString();
                //add noLog here sowe can capture message http://www.codeproject.com/Articles/475723/Logging-Setup-in-Minutes-with-NLog
                return NotFound(); //todo

            }
            catch (Exception e)
            {
                return NotFound();
            }
            
        } 
    }
}
