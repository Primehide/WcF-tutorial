using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.BL.Domain;
using System.Configuration; //nodig om de config file te kunnen aanspreken
using System.Data.SqlClient; //nodig om onze databank aan te spreken

namespace SC.DAL.SqlClient
{
    public class TicketRepository : ITicketRepository
    {
        private SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SupportCenterDB_SqlClient"].ConnectionString; //connectie string uit App.config ophalen
            return new SqlConnection(connectionString);
        }

        public Ticket CreateTicket(Ticket ticketToCreate)
        {
            string insertStatement = "INSERT INTO Ticket(AccountId, [Text], DateOpened,)"
                                      + ",State, DeviceName) VALUES (@AccountId, @Text, @DateOpened, @State, @DeviceName)";
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(insertStatement, connection);
                command.Parameters.AddWithValue("@AccountId", ticketToCreate.AccountId);
                command.Parameters.AddWithValue("@Text", ticketToCreate.Text);
                command.Parameters.AddWithValue("@DateOpened", ticketToCreate.DateOpened.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@state", (byte)ticketToCreate.State);

                if (ticketToCreate is HardwareTicket)//'is' kijkt na of ticketToCreate een HardwareTicket klasse is
                    command.Parameters.AddWithValue("@DeviceName", ((HardwareTicket)ticketToCreate).DeviceName);
                else
                    command.Parameters.AddWithValue("@DeviceName", DBNull.Value);
                command.CommandText += "; SELECT SCOPE_IDENTITY();"; //gaat de primary key ophalen voor ticketNumber

                connection.Open();

                ticketToCreate.TicketNumber = Convert.ToInt32(command.ExecuteScalar());
                connection.Close(); 
            }
            return ticketToCreate;
        }

        public TicketResponse CreateTicketResponse(TicketResponse response)
        {
            if (response.Ticket != null)
            {
                string insertStatement = "INSERT INTO TicketResponse([TEXT], [Date], isClientResponse, Ticket_TicketNumber)"
                                    + " VALUES (@Text, @Date, @isClientResponse, @TicketNumber)";

                using (var connection = GetConnection())
                {
                    var insertCommand = new SqlCommand(insertStatement, connection);
                    insertCommand.Parameters.AddWithValue("@Text", response.Text);
                    insertCommand.Parameters.AddWithValue("@Date", response.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    insertCommand.Parameters.AddWithValue("@isClientResponse", response.IsClientResponse);
                    insertCommand.Parameters.AddWithValue("@TicketNumber", response.Ticket.TicketNumber);

                    insertCommand.CommandText += "; SELECT SCOPE_IDENTITY()"; //gaat primary key ophalen

                    connection.Open();
                    response.Id = Convert.ToInt32(insertCommand.ExecuteScalar());
                    connection.Close();
                }
                return response;
            }
            else
                throw new ArgumentException("Ticketresponse has no ticket attachted to it!");  
        }

        public void DeleteTicket(int ticketNumber)
        {
            string deleteTicketStatement = "DELETE FROM Ticket WHERE TicketNumber = @TicketNumber";
            string deleteResponseStatement = "DELETE FROM TicketResponse WHERE Ticket_Number = @TicketNumber";

            using (var connection = GetConnection())
            {
                var ticketCommand = new SqlCommand(deleteTicketStatement, connection);
                ticketCommand.Parameters.AddWithValue("@TicketNumber", ticketNumber);

                var responseCommand = new SqlCommand(deleteResponseStatement, connection);
                responseCommand.Parameters.AddWithValue("@TicketNumber", ticketNumber);

                connection.Open();

                using (var transaction = connection.BeginTransaction()) //transactie beginnen
                {
                     //statements die tot transactie behoren in transactie object steken
                    responseCommand.Transaction = transaction;
                    ticketCommand.Transaction = transaction;
                    //
                    responseCommand.ExecuteNonQuery();
                    ticketCommand.ExecuteNonQuery();

                    transaction.Commit(); //als alle statements uit transactie zijn uitgevoert, committen
                }
                connection.Close();
            }
        }

        public Ticket ReadTicket(int ticketNumber)
        {
            Ticket requestedTicket = null;

            string statement = "SELECT TicketNumber, AccountId, [Text], DateOpened"
                                 + ", State, DeviceName FROM Ticket"
                                 + " WHERE TicketNumber = @ticketNumber";
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(statement, connection);
                command.Parameters.AddWithValue("@ticketNumber", ticketNumber); //parameter in statement invullen

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int ticketNumberOrdinal = reader.GetOrdinal("TicketNumber");
                    int accountIdOrdinal = reader.GetOrdinal("AccountId");
                    int textOrdinal = reader.GetOrdinal("Text");
                    int dateOpenedOrdinal = reader.GetOrdinal("DateOpened");
                    int stateOrdinal = reader.GetOrdinal("State");
                    int deviceNameOrdinal = reader.GetOrdinal("DeviceName");

                    if(reader.Read())
                    {
                        string deviceName = reader.IsDBNull(deviceNameOrdinal) ? null 
                                            : reader.GetString(deviceNameOrdinal);
                        if (deviceName == null)
                            requestedTicket = new Ticket();
                        else
                            requestedTicket = new HardwareTicket() { DeviceName = deviceName };
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return requestedTicket;
        }

        public IEnumerable<TicketResponse> ReadTicketResponsesOfTicket(int ticketNumber)
        {
            List<TicketResponse> RequestedTicketResponse = new List<TicketResponse>(); //hierin gaan we de opgevraagde ticketresponses insteken en teruggeven
            string readStatement = "SELECT TicketResponse AS rId, TicketResponse.[TEXT] AS rText, [Date], isClientResponse"
                                + " FROM TicketResponse"
                                + " INNER JOIN Ticket ON Ticket.TicketNumber = TicketResponse.Ticket_Ticket_number"
                                + " WHERE Ticket.TicketNumber = @TicketNumber";
            

            using (var connection = GetConnection())
            {
                

                var readCommand = new SqlCommand(readStatement, connection);
                readCommand.Parameters.AddWithValue("@TicketNumber", ticketNumber);
                connection.Open();

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    int rIdOrdinal = reader.GetOrdinal("rId");
                    int rTextOrdinal = reader.GetOrdinal("rText");
                    int DateOrdinal = reader.GetOrdinal("Date");
                    int isClientResponseOrdinal = reader.GetOrdinal("isClientResponse");

                    while(reader.Read())
                    {
                        RequestedTicketResponse.Add(new TicketResponse()
                        {
                            Id = reader.GetInt32(rIdOrdinal),
                            Text = reader.GetString(rTextOrdinal),
                            Date = reader.GetDateTime(DateOrdinal),
                            IsClientResponse = reader.GetBoolean(isClientResponseOrdinal)
                        });
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return RequestedTicketResponse;
        }

        public IEnumerable<Ticket> ReadTickets()
        {
            List<Ticket> tickets = new List<Ticket>();

            string selectStatement = "SELECT TicketNumber, AccountId, [Text], DateOpened"
                                      + ", State, DeviceName FROM Ticket";
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(selectStatement, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int ticketNumberOrdinal = reader.GetOrdinal("TicketNumber"); //gaat index van kolom Ticketnumber uit ons teruggegeven resultaat van select geven
                    int accountIdOrdinal = reader.GetOrdinal("AccountId");
                    int textOrdinal = reader.GetOrdinal("Text");
                    int dateOpenedOrdinal = reader.GetOrdinal("DateOpened");
                    int stateOrdinal = reader.GetOrdinal("State");
                    int deviceOrdinal = reader.GetOrdinal("DeviceName");

                    while(reader.Read())
                    {
                        Ticket ticket;
                        string deviceName = reader.IsDBNull(deviceOrdinal) ? null : reader.GetString(deviceOrdinal);//controle of de waarde in DB null is

                        if (deviceName == null) //als devicename null is dan is het een normaal ticket
                            ticket = new Ticket();
                        else //anders een hardware ticket
                            ticket = new HardwareTicket() { DeviceName = deviceName };
                        ticket.TicketNumber = reader.GetInt32(ticketNumberOrdinal);
                        ticket.AccountId = reader.GetInt32(accountIdOrdinal);
                        ticket.Text = reader.GetString(textOrdinal);
                        ticket.DateOpened = reader.GetDateTime(dateOpenedOrdinal);
                        ticket.State = (TicketState)reader.GetByte(stateOrdinal); //ticketstate is een byte, kunnen casten naar TicketState

                        tickets.Add(ticket);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return tickets;
        }

        public void UpdateTicket(Ticket ticket)
        {
            string updateStatement = "UPDATE Ticket SET AccountId = @AccountId, [Text]= @Text"
                                      + ", DateOpened = @DateOpened, State = @State"
                                      + ", DeviceName = @DeviceName"
                                      + " WHERE TicketNumber = @TicketNumber";
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand(updateStatement, connection);
                command.Parameters.AddWithValue("@AccountId", ticket.AccountId);
                command.Parameters.AddWithValue("@Text", ticket.Text);
                command.Parameters.AddWithValue("@DateOpened", ticket.DateOpened.ToString("yyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@State", (byte)ticket.State);

                if (ticket is HardwareTicket)
                    command.Parameters.AddWithValue("@DeviceName", ((HardwareTicket)ticket).DeviceName);
                else
                    command.Parameters.AddWithValue("@DeviceName", DBNull.Value);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateTicketStateToClosed(int ticketnumber)
        {
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand("sp_CloseTicket", connection);

                command.CommandType = System.Data.CommandType.StoredProcedure; //gaan we aanduiden dat er gebruikt wordt gemaakt van een stored procedure in de databank
                //stored procedure sp_CloseTicket heeft dit statement in onze databank
                //"UPDATE Ticket SET State = 4 WHERE TicketNumber = @ticketNumber"
                //Parameter opvullen
                command.Parameters.AddWithValue("@ticketNumber", ticketnumber);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<HardwareTicket> ReadHwTickets()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> ReadNormalTickets()
        {
            throw new NotImplementedException();
        }
    }
}
