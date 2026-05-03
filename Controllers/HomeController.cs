using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication11.Models;
using Microsoft.Data.Sqlite;

namespace WebApplication11.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            List<Ticket> list = new List<Ticket>();

            using (SqliteConnection con = new SqliteConnection(@"Data Source=C:\Users\Memmedov\Documents\university\c#\tapsirig10\bilet.db"))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Code, Flight, Time FROM biletler";
               using (var reader = cmd.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       list.Add(new Ticket
                       {
                           Code = reader["Code"].ToString(),
                           Flight = reader["Flight"].ToString(),
                           Time = reader["Time"].ToString()
                       });
                   }
               }
            }
            return View(list);
        }

        [HttpPost]
        public IActionResult AddTicket(string flight, string time)
        {
            string code = "FLY" + new Random().Next(100, 999);

            using (SqliteConnection con = new SqliteConnection(@"Data Source=C:\Users\Memmedov\Documents\university\c#\tapsirig10\bilet.db"))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO biletler (Code, Flight, Time) VALUES (@code, @flight, @time)";
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@flight", flight);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Scan(string code)
        {
            Ticket ticket = null;

            using (SqliteConnection con = new SqliteConnection(@"Data Source=C:\Users\Memmedov\Documents\university\c#\tapsirig10\bilet.db"))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Code, Flight, Time, IsUsed FROM biletler WHERE Code = @code";
                cmd.Parameters.AddWithValue("@code", code);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ticket = new Ticket
                        {
                            Code = reader["Code"].ToString(),
                            Flight = reader["Flight"].ToString(),
                            Time = reader["Time"].ToString(),
                            IsUsed = Convert.ToInt32(reader["IsUsed"])
                        };
                    }
                }
                if (ticket == null)
                {
                    ViewBag.Status = "BAGLI";
                    ViewBag.Message = "XƏTA: Etibarsız bilet kodu!";
                }
                else if (ticket.IsUsed == 1)
                {
                    ViewBag.Status = "BAGLI";
                    ViewBag.Message = "Bu bilet artıq istifadə olunub!";
                }
                else
                {
                    ViewBag.Status = "ACIQ";
                    ViewBag.Message = "Xoş gəlmisiniz! Keçid açıqdır.";
                    var updateCmd = con.CreateCommand();
                    updateCmd.CommandText = "UPDATE biletler SET IsUsed = 1 WHERE Code = @code";
                    updateCmd.Parameters.AddWithValue("@code", code);
                    updateCmd.ExecuteNonQuery();
                }
            }

            List<Ticket> list = GetTickets();
            return View("Index", list);
        }
        private List<Ticket> GetTickets()
        {
            List<Ticket> list = new List<Ticket>();
            using (SqliteConnection con = new SqliteConnection(@"Data Source=C:\Users\Memmedov\Documents\university\c#\tapsirig10\bilet.db"))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Code, Flight, Time FROM biletler";
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                {
                    list.Add(new Ticket
                    {
                        Code = reader["Code"].ToString(),
                        Flight = reader["Flight"].ToString(),
                        Time = reader["Time"].ToString()
                    });
                }
            }
            return list;
        }
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}