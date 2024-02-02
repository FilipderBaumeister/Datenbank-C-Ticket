using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Xml.Linq;
using SystemConfiguration = System.Configuration.ConfigurationManager;

namespace Programm_mit_Datenbank_verbinden
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Fügen Sie den folgenden Code hier ein
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("configuration",
                    new XElement("connectionStrings",
                        new XElement("add",
                            new XAttribute("name", "TicketSystem.Properties.Settings.TicketDatabaseConnectionString"),
                            new XAttribute("connectionString", "Data Source=(local);Initial Catalog=YourDatabaseName;Integrated Security=True"),
                            new XAttribute("providerName", "System.Data.SqlClient")
                        )
                    )
                )
            );
        }

            private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class Ticket
        {
            public int TicketNumber { get; set; }
            public string TicketName { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connetionString;
            SqlConnection cnn;
            connetionString = SystemConfiguration.ConnectionStrings["TicketSystem.Properties.Settings.TicketDatabaseConnectionString"].ConnectionString;
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            MessageBox.Show("Connection Open  !");
            cnn.Close();

        }

        public List<Ticket> GetTickets()
        {
            string connectionString = SystemConfiguration.ConnectionStrings["TicketSystem.Properties.Settings.TicketDatabaseConnectionString"].ConnectionString; List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Tickets", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ticket ticket = new Ticket
                            {
                                TicketNumber = Convert.ToInt32(reader["TicketNumber"]),
                                TicketName = reader["TicketName"].ToString()
                            };

                            tickets.Add(ticket);
                        }
                    }
                }
            }

            return tickets;
        }

        public void AddTicket(Ticket ticket)
        {
            string connectionString = SystemConfiguration.ConnectionStrings["TicketSystem.Properties.Settings.TicketDatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("INSERT INTO Tickets (TicketNumber, TicketName) VALUES (@TicketNumber, @TicketName)", connection))
                {
                    command.Parameters.AddWithValue("@TicketNumber", ticket.TicketNumber);
                    command.Parameters.AddWithValue("@TicketName", ticket.TicketName);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTicket(int ticketNumber)
        {
            string connectionString = SystemConfiguration.ConnectionStrings["TicketSystem.Properties.Settings.TicketDatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM Tickets WHERE TicketNumber = @TicketNumber", connection))
                {
                    command.Parameters.AddWithValue("@TicketNumber", ticketNumber);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
