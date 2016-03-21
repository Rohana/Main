using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using System.IO;
using Microsoft.AspNet.Http;
using System.Net.Mail;
using System.Net.Mime;
using SendGrid;

namespace TrackManager.Models
{
    public class PdfForm
    {

        public int Id { get; set; }


        public string First { get; set; }
    
        public string Last { get; set; }

        public void AutoFill(string webRoot, PdfForm pdfForm,out string error)
        {
            error = "NO Errors";
            String pdfTemplate = webRoot+ @"/forms/fw4.pdf";
            String newFile = webRoot + @"/forms/Final_fw4.pdf";
            error = "Web root: " + webRoot;

            try {

                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(
                            newFile, FileMode.Create));

                AcroFields pdfFormFields = pdfStamper.AcroFields;

                // ' set form pdfFormFields

                // ' The first worksheet and W-4 form
                pdfFormFields.SetField("f1_01(0)", "1");
                pdfFormFields.SetField("f1_02(0)", "1");
                pdfFormFields.SetField("f1_03(0)", "1");
                pdfFormFields.SetField("f1_04(0)", "8");
                pdfFormFields.SetField("f1_05(0)", "0");
                pdfFormFields.SetField("f1_06(0)", "1");
                pdfFormFields.SetField("f1_07(0)", "16");
                pdfFormFields.SetField("f1_08(0)", "28");
                pdfFormFields.SetField("f1_09(0)", pdfForm.First);
                pdfFormFields.SetField("f1_10(0)", pdfForm.Last);
                pdfFormFields.SetField("f1_11(0)", "532");
                pdfFormFields.SetField("f1_12(0)", "12");
                pdfFormFields.SetField("f1_13(0)", "1234");

                //' The form's checkboxes
                pdfFormFields.SetField("c1_01(0)", "0");
                pdfFormFields.SetField("c1_02(0)", "Yes");
                pdfFormFields.SetField("c1_03(0)", "0");
                pdfFormFields.SetField("c1_04(0)", "Yes");

                // ' The rest of the form pdfFormFields
                pdfFormFields.SetField("f1_14(0)", "100 North Cujo Street");
                pdfFormFields.SetField("f1_15(0)", "Nome, AK  67201");
                pdfFormFields.SetField("f1_16(0)", "9");
                pdfFormFields.SetField("f1_17(0)", "10");
                pdfFormFields.SetField("f1_18(0)", "11");
                pdfFormFields.SetField("f1_19(0)", "Walmart, Nome, AK");
                pdfFormFields.SetField("f1_20(0)", "WAL666");
                pdfFormFields.SetField("f1_21(0)", "AB");
                pdfFormFields.SetField("f1_22(0)", "4321");

                // ' Second Worksheets pdfFormFields
                // ' In order to map the fields, I just pass them a sequential
                // ' number to mark them once I know which field is which, I 
                // ' can pass the appropriate value
                pdfFormFields.SetField("f2_01(0)", "1");
                pdfFormFields.SetField("f2_02(0)", "2");
                pdfFormFields.SetField("f2_03(0)", "3");
                pdfFormFields.SetField("f2_04(0)", "4");
                pdfFormFields.SetField("f2_05(0)", "5");
                pdfFormFields.SetField("f2_06(0)", "6");
                pdfFormFields.SetField("f2_07(0)", "7");
                pdfFormFields.SetField("f2_08(0)", "8");
                pdfFormFields.SetField("f2_09(0)", "9");
                pdfFormFields.SetField("f2_10(0)", "10");
                pdfFormFields.SetField("f2_11(0)", "11");
                pdfFormFields.SetField("f2_12(0)", "12");
                pdfFormFields.SetField("f2_13(0)", "13");
                pdfFormFields.SetField("f2_14(0)", "14");
                pdfFormFields.SetField("f2_15(0)", "15");
                pdfFormFields.SetField("f2_16(0)", "16");
                pdfFormFields.SetField("f2_17(0)", "17");
                pdfFormFields.SetField("f2_18(0)", "18");
                pdfFormFields.SetField("f2_19(0)", "19");

                //' report by reading values from completed PDF
                String sTmp = "W-4 Completed for " + pdfFormFields.GetField("f1_09(0)") + " " +
            pdfFormFields.GetField("f1_10(0)");
                // MessageBox.Show(sTmp, "Finished");

                //  ' flatten the form to remove editting options, set it to false
                // ' to leave the form open to subsequent manual edits
                pdfStamper.FormFlattening = true;

                //' close the pdf
                pdfStamper.Close();
                SendGridMail(newFile, pdfForm.First+" "+ pdfForm.Last);
                //SendMail(newFile);
            }
            catch(Exception e)
            {
                error = "Exception" + e.Message;
            }
        }

        public void SendMail(string fileAttachment)
        {
            string from = "rohana100@gmail.com";
            string to = "rohana100@gmail.com";
            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = "Birthday Greetings";
                mm.Body = string.Format("<b>Happy Birthday </b>{0}<br /><br />Many happy returns of the day.", "RO");

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                credentials.UserName = from;
                credentials.Password = "";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
                smtp.Port = 587;

                // Create  the file attachment for this e-mail message.
                Attachment data = new Attachment(fileAttachment, MediaTypeNames.Application.Octet);
                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(fileAttachment);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(fileAttachment);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(fileAttachment);
                // Add the file attachment to this e-mail message.
                mm.Attachments.Add(data);
               
                smtp.Send(mm);
                
            }
        }

        public void SendGridMail(string fileAttachment,string name)
        {
            // Create the email object first, then add the properties.
            SendGridMessage myMessage = new SendGridMessage();
            myMessage.AddTo("rohana100@gmail.com");
            myMessage.From = new MailAddress("rohana100@gmail.com", "Rohana Liyanarachchi");
            myMessage.Subject = "Medical Form of " + name;
            myMessage.Text = "Please find the completed form attached";
            myMessage.AddAttachment(fileAttachment);

            // Create credentials, specifying your user name and password.
            var credentials = new System.Net.NetworkCredential("azure_666bb7ea2a5107b85db9d93cf5ff36c6@azure.com", "123liyanarachchi");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email, which returns an awaitable task.
            transportWeb.DeliverAsync(myMessage);

            // If developing a Console Application, use the following
            // transportWeb.DeliverAsync(mail).Wait();
        }
    }
}
