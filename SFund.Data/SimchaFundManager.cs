using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SFund.Data
{
  
    public class SimchaFundManager
    {
        private readonly string _connectionString;

        public SimchaFundManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT s.Id, s.Name,s.Date,SUM(c.Amount) AS 'Amount',COUNT(c.ContributorId)AS 'Amount of Contributors' FROM Simchas s LEFT JOIN Contributions c on s.Id=c.SimchaId GROUP BY s.Id,s.Name,s.Date";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Simcha> simchas = new();
            while (reader.Read())
            {
                simchas.Add(new Simcha()
                {

                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Amount = reader.GetOrNull<decimal>("Amount"),
                    AmountOfContributors = (int)reader["Amount of contributors"]



                });


            };

            return simchas;

        }

        public int GetTotalAmountOfContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(Id) FROM Contributors";
            connection.Open();
            int amount = (int)cmd.ExecuteScalar();
            return amount;


        }

        public void AddNewSimcha(Simcha simcha)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas (Name,Date) VALUES(@name,@date)";
            cmd.Parameters.AddWithValue("@name", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            var reader = cmd.ExecuteNonQuery();

        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT c.*, ISNULL(d.TotalDeposits, 0) - ISNULL(con.TotalContributions, 0)" +
                "AS 'Balance' FROM Contributors c LEFT JOIN " +
                "(SELECT ContributorId, SUM(Amount) AS TotalDeposits FROM Deposits " +
                "GROUP BY ContributorId) d ON c.Id = d.ContributorId " +
                "LEFT JOIN (SELECT ContributorId, SUM(Amount) AS TotalContributions " +
                "FROM Contributions GROUP BY ContributorId) con ON c.Id = con.ContributorId;";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();
            while (reader.Read())
            {
                contributors.Add(new Contributor()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Date = (DateTime)reader["Date"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Balance"]

                });


            };

            return contributors;

        }

        public int AddContributor(Contributor contributor)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors (FirstName,LastName,CellNumber,Date,AlwaysInclude) VALUES(@firstName,@lastName,@cellNumber,@date,@alwaysInclude); SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);

            connection.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();
            return id;
        }

        public void InsertDeposit(Deposit deposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits (ContributorId,Amount,Date) VALUES(@contributorId,@Amount,@date)";
            cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            var reader = cmd.ExecuteNonQuery();
        }

        public void UpdateContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors set AlwaysInclude = @AlwaysInclude, FirstName = @firstName, LastName = @LastName ,CellNumber = @Cell
Where id = @id";
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.Parameters.AddWithValue("@FirstName", c.FirstName);
            cmd.Parameters.AddWithValue("@LastName", c.LastName);
            cmd.Parameters.AddWithValue("@Cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@AlwaysInclude", c.AlwaysInclude);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<TransactionHistory> GetDepositHistoryById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT d.* FROM Contributors c JOIN Deposits d ON c.Id=d.ContributorId WHERE c.Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<TransactionHistory> deposits = new();
            while (reader.Read())
            {
                deposits.Add(new TransactionHistory()
                {

                    TransactionName = "Deposit",
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]

                });

            }
            return deposits;
        }

        public List<TransactionHistory> GetContributionsHistoryById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT s.Name,s.Date,cc.Amount FROM Contributors c JOIN Contributions cc ON c.Id=cc.ContributorId LEFT JOIN Simchas s ON cc.SimchaId=s.Id WHERE c.Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<TransactionHistory> contributions = new();
            while (reader.Read())
            {
                contributions.Add(new TransactionHistory()
                {

                    TransactionName = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]


                });

            };
            return contributions;


        }

        public decimal GetBalanceById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"SELECT (SELECT SUM(Amount) FROM Deposits WHERE ContributorId = @id) AS 'Deposits', 
            (SELECT SUM(Amount) FROM Contributions WHERE ContributorId = @id) AS 'Contributions'";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            var deposits = reader.GetOrNull<decimal>("Deposits");
            var contributions = reader.GetOrNull<decimal>("Contributions");
            return deposits - contributions;

        }

        public string GetContributorNameById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT c.FirstName +' '+c.LastName AS 'NAME' FROM Contributors c WHERE c.Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            string name = (string)cmd.ExecuteScalar();
            return name;

        }

        public string GetSimchaNameById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT s.Name FROM Simchas s WHERE s.Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            string name = (string)cmd.ExecuteScalar();
            return name;

        }

        public decimal GetTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT (SELECT SUM(Amount) FROM Deposits) AS 'Deposits', (SELECT SUM(Amount) FROM Contributions) AS 'Contributions'";
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            var deposits = reader.GetOrNull<decimal>("Deposits");
            var contributions = reader.GetOrNull<decimal>("Contributions");
            return deposits - contributions;

        }

        public List<Contributor> GetContributionsForASimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT c.Id, c.FirstName, c.LastName, c.AlwaysInclude, SUM(cc.Amount) AS 'Contributions' FROM Contributors c LEFT JOIN Contributions cc ON c.Id = cc.ContributorId AND cc.SimchaId = @id GROUP BY c.Id, c.FirstName, c.LastName, c.AlwaysInclude";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Contributor> contributions = new();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new Contributor()
                {
                    Id = (int)reader["id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Contribution = reader.GetOrNull<decimal>("Contributions")

                });


            }
            foreach (var c in contributions)
            {
                c.Balance = GetBalanceById(c.Id);

            }
            return contributions;


        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }


        public void UpdateSimcha(int simchaId, List<ContributionInclusion> contributions)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions (ContributorId, SimchaId, Amount) Values (@contribId, @simchaId, @amount)";
            connection.Open();
            foreach (var c in contributions)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@contribId", c.ContributorId);
                cmd.Parameters.AddWithValue("@simchaId", simchaId);       
                cmd.Parameters.AddWithValue("@amount", c.Amount);
                cmd.ExecuteNonQuery();
            }
        }



    }

    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}

