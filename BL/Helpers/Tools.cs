using System.Collections;
using System.Net.Mail;
using System.Net;
namespace Helpers;

public static class Tools
{
    /// <summary>
    /// Given the generic value T, this method will generate a stirng containing every field and its value for the given "val" instance
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    /// <param name="val">The instance which is needed to be converted to a string, containing its values</param>
    /// <returns>a string containig all the fields of the T class and the values of the val instance</returns>
    /// <exception cref="Exception">If the instance is null the program will throw an error</exception>
    public static string ToStringProperty<T>(this T val)
    {
        string msg = "";
        if (val == null)
            throw new BO.BlUnWantedNullValueException("Internal Error: Cann't represent string for null values");
        else
        {
            Type currentVarType = val.GetType();
            msg += $"Entity Type: {currentVarType.Name}\n";
            foreach (System.Reflection.PropertyInfo? field in currentVarType.GetProperties())
            {
                var fieldValue = field.GetValue(val);
                msg += $"{field.Name}:";

                //Type of field (IEnumerable / Simple field)
                if (fieldValue is IEnumerable && fieldValue is not string)
                {
                    msg += " [";
                    foreach (var element in (IEnumerable)fieldValue)
                    {
                        msg += $" {element}";
                    }
                    msg += "]\n";
                }
                else
                {
                    msg += $" {fieldValue}\n";
                }
            }
        }
        return msg;
    }
    /// <summary>
    /// I kept the synchronous function because it is the original implementation.  
    /// The function below is the same, but it works asynchronously.
    /// </summary>
    /// <param name="toEmail">Recipient's email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    //public static void SendEmail(string toEmail, string subject, string body)
    //{
    //    try
    //    {
    //        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)) 
    //        {

    //            smtpClient.Credentials = new NetworkCredential("meircrombie@gmail.com", "syby zvun cxab gokx");
    //            smtpClient.EnableSsl = true; 


    //            MailMessage mailMessage = new MailMessage
    //            {
    //                From = new MailAddress("meircrombie@gmail.com"),
    //                Subject = subject,
    //                Body = body,
    //                IsBodyHtml = true
    //            };

    //            mailMessage.To.Add(toEmail);

    //            smtpClient.Send(mailMessage);
    //            Console.WriteLine($"Email sent successfully to {toEmail}");
    //        }
    //    }
    //    catch (SmtpException smtpEx)
    //    {
    //        Console.WriteLine($"SMTP Error: {smtpEx.Message}");
    //        if (smtpEx.InnerException != null)
    //        {
    //            Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"General Error: {ex.Message}");
    //    }
    //}
    public static async Task SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("meircrombie@gmail.com", "syby zvun cxab gokx");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("meircrombie@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // Asynchronously send the email
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {toEmail}");
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"SMTP Error: {smtpEx.Message}");
            if (smtpEx.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
    }


}



