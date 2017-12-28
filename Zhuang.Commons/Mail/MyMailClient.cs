using System.Net.Mail;

namespace Zhuang.Commons.Mail
{
    public class MyMailClient
    {
        private SmtpClient _smtpClient;
        private MailMessage _mailMessage;

        public SmtpClient SmtpClient
        {
            get { return _smtpClient; }
            set { _smtpClient = value; }
        }

        public MailMessage MailMessage
        {
            get { return _mailMessage; }
            set { _mailMessage = value; }
        }

        public MyMailClient()
        {
            _smtpClient = new SmtpClient();
            _mailMessage = new MailMessage();

            Init();
        }

        public MyMailClient(string host,int? port, string from, string to,string fromUserName,string fromPassword)
        {
            if (port.HasValue)
            {
                _smtpClient = new SmtpClient(host,port.Value);
            }
            else
            {
                _smtpClient = new SmtpClient(host);
            }
            _mailMessage = new MailMessage(from, to);

            Init();

            System.Net.NetworkCredential nc = new System.Net.NetworkCredential(fromUserName, fromPassword);
            _smtpClient.Credentials = new System.Net.NetworkCredential(fromUserName, fromPassword);
            //NTLM: Secure Password Authentication in Microsoft Outlook Express
            //_sc.Credentials = nc.GetCredential(_sc.Host, _sc.Port, "NTLM");


        }

        private void Init()
        {
            _mailMessage.IsBodyHtml = true;
            _smtpClient.UseDefaultCredentials = true;
            //_sc.EnableSsl = true;
            _smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        }

        public void Send()
        {
            _smtpClient.Send(_mailMessage);
        }

        public void Send(string subject, string body)
        {
            _mailMessage.Subject = subject;
            _mailMessage.Body = body;
            _smtpClient.Send(_mailMessage);
        }

        public void Send(string from, string recipients, string subject, string body)
        {
            _smtpClient.Send(from, recipients, subject, body);
        }

    }
}
