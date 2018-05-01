using System;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;
using DotNetMail = System.Net.Mail;

namespace SmtpAndImapClient
{
    class Program
    {
        private static DotNetMail.MailAddress from = new DotNetMail.MailAddress("example@example.com", "example");
        private static string password = "mqxfqibxryatleks";
        private static DotNetMail.MailAddress to = new DotNetMail.MailAddress("example2@example.com", "example2");

        public static void Main(string[] args)
        {
            //dotNetMailSendMessage("123", "123");
            sendMessage("123", "123");
            readMessage();
        }

        /// <summary>
        /// Reads all the messages.
        /// </summary>
        private static void readMessage()
        {
            using (ImapClient client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(from.Address, password);

                IMailFolder inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    MimeMessage message = inbox.GetMessage(i);
                    Console.WriteLine("\n-------------------- messsage " + i + 1 + ", size=" + inbox.Fetch(0, -1, MessageSummaryItems.Size).ElementAt(i).Size + " --------------------");
                    Console.WriteLine("From: {0}", message.From[0]);
                    Console.WriteLine("Subject: {0}", message.Subject);
                    Console.WriteLine("Text: {0}", message.TextBody);
                    Console.WriteLine("-------------------- end of message " + i + 1 + " --------------------");
                }

                Console.WriteLine("Press a key to continue");
                Console.ReadKey();
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Sends a message to an SMTP server for delivery.
        /// </summary>
        /// <param name="subject">The subject line for this e-mail message.</param>
        /// <param name="body">The message body for this e-mail message.</param>
        private static void sendMessage(string subject, string body)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(from.DisplayName, from.Address));
            message.To.Add(new MailboxAddress(to.DisplayName, to.Address));
            message.Subject = "How you doin'?";
            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
            };

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(from.Address, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Sends a message to an SMTP server for delivery.
        /// </summary>
        /// <param name="subject">The subject line for this e-mail message.</param>
        /// <param name="body">The message body for this e-mail message.</param>
        private static void dotNetMailSendMessage(string subject, string body)
        {
            using (DotNetMail.SmtpClient client = new DotNetMail.SmtpClient())
            {
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from.Address, password);

                using (DotNetMail.MailMessage message = new DotNetMail.MailMessage(from, to) { Subject = subject, Body = body })
                {
                    client.Send(message);
                }
            }
        }
    }
}
