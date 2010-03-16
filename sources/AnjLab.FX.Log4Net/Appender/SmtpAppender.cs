using System.IO;
using System.Net.Mail;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace AnjLab.FX.Log4Net.Appender
{
    public class SmtpAppender : log4net.Appender.SmtpAppender
    {
        public ILayout SubjectLayout { get; set; }
        public string EnableSsl { get; set; }
        public string SubjectMaxLength { get; set; }

        protected override void  SendBuffer(LoggingEvent[] events)
        {
            //  Choose the latest event to format subject
            FormatSubject(events[events.Length-1]);
            base.SendBuffer(events);
        }

        /// <summary>
        /// Formats email's subject using <see cref="SubjectLayout"/>.
        /// 
        /// <see cref="AppenderSkeleton.RenderLoggingEvent(TextWriter, log4net.Core.LoggingEvent)"/>
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected void FormatSubject(LoggingEvent loggingEvent)
        {
            var layout = SubjectLayout;

            if (layout == null)
            {
                LogLog.Warn("SmtpAppender: No subjectLayout configured for appender [" + Name + "]");
                return;
            }

            var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);

            if (layout.IgnoresException) 
			{
				string exceptionStr = loggingEvent.GetExceptionString();
				if (!string.IsNullOrEmpty(exceptionStr)) 
				{
					// render the event and the exception
					layout.Format(writer, loggingEvent);
					writer.WriteLine(exceptionStr);
				}
				else 
				{
					// there is no exception to render
                    layout.Format(writer, loggingEvent);
				}
			}
			else 
			{
				// The layout will render the exception
                layout.Format(writer, loggingEvent);
			}

            string subject = writer.ToString();

            //  Take first line only (Subject may not contain any control chars)
            var idx = subject.IndexOf("\r\n");
            if (idx != -1)
            {
                subject = subject.Substring(0, idx);
            }
            
            //  Cut subject to specified length
            if (!string.IsNullOrEmpty(SubjectMaxLength))
            {
                int maxLength;
                if (int.TryParse(SubjectMaxLength, out maxLength))
                {
                    if (maxLength > 0 && subject.Length > maxLength)
                    {
                        subject = subject.Substring(0, maxLength);
                    }
                }
            }

            Subject = subject;
        }

        /// <summary>
        ///  This override supports SSL (which is required by gmail).
        /// 
        /// <inheritdoc/>
        /// </summary>
        /// <param name="messageBody"></param>
        override protected void SendEmail(string messageBody)
        {
			// .NET 2.0 has a new API for SMTP email System.Net.Mail
			// This API supports credentials and multiple hosts correctly.
			// The old API is deprecated.

			// Create and configure the smtp client
			var smtpClient = new SmtpClient();
			if (!string.IsNullOrEmpty(SmtpHost))
			{
				smtpClient.Host = SmtpHost;
			}
			smtpClient.Port = Port;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

			if (Authentication == SmtpAuthentication.Basic)
			{
				// Perform basic authentication
				smtpClient.Credentials = new System.Net.NetworkCredential(Username, Password);
			}
            else if (Authentication == SmtpAuthentication.Ntlm)
			{
				// Perform integrated authentication (NTLM)
				smtpClient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
			}

            if (!string.IsNullOrEmpty(EnableSsl))
            {
                smtpClient.EnableSsl = "true".Equals(EnableSsl.ToLower());
            }

			var mailMessage = new MailMessage
			                      {
			                          Body = messageBody,
			                          From = new MailAddress(From),
			                          Subject = Subject,
			                          Priority = Priority
			                      };
            mailMessage.To.Add(To);

            //  TODO: Send async?
			smtpClient.Send(mailMessage);
        }
    }
}