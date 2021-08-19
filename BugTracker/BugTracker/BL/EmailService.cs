using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace BugTracker.BL
{
    public static class EmailManager
    {
        public static void SendEmail(string userName, string email, string subject, string text)
        {
            //if (!reciver.EmailConfirmed)
            //    return Task.CompletedTask;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bug Tracker", "bugtracker@guerrillamail.net"));
            message.To.Add(new MailboxAddress(userName, email));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp-relay.sendinblue.com", 587, false);

                client.Authenticate("adam.isaak.dev@gmail.com", "2RWqj9gY73sGE6pf");

                client.Send(message);
                client.Disconnect(true);
            }

            return;
        }
    }
}