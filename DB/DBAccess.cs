using ReadingEmailsGraphAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEmailsGraphAPI.DB
{
    public static class DBAccess
    {
        public static int AddToEmailReceived(Email email)
        {
            int res = 0;

            using (SqlConnection conn = new SqlConnection(DBConfiguration.GetConnectionString()))
            {
                using (SqlCommand cmdInsertEmail = new SqlCommand("INSERT INTO EmailReceived (name,from_email,received_datetime,subject,message,transferred_to_hubspot) " +
                                                                  "VALUES (@Name,@FromEmail,@ReceivedDateTime,@Subject,@Message,@TransferredToHubspot)",conn))
                {
                    conn.Open();
                    try
                    {
                        cmdInsertEmail.Parameters.AddWithValue("@Name", email.Name);
                        cmdInsertEmail.Parameters.AddWithValue("@FromEmail", email.From);
                        cmdInsertEmail.Parameters.AddWithValue("@ReceivedDateTime", email.ReceivedDateTime);
                        cmdInsertEmail.Parameters.AddWithValue("@Subject", email.Subject);
                        cmdInsertEmail.Parameters.AddWithValue("@Message", email.Message);
                        cmdInsertEmail.Parameters.AddWithValue("@TransferredToHubspot", email.TransferredToHubSpot);
                        res = cmdInsertEmail.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return res;
        }
    }
}
