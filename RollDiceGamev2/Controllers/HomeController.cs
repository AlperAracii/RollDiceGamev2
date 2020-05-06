using RollDiceGamev2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web.Mvc;

namespace RollDiceGamev2.Controllers
{
    public class HomeController : Controller
    {
        OleDbCommand cmd;
        OleDbConnection conn;
        OleDbDataReader rd;

        public HomeController()
        {
            cmd = new OleDbCommand();
            conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\RollDiceGameDb.mdb;Persist Security Info=True");
        }

        public ActionResult Index()
        {
            if (Session["Permission"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var model = new IndexViewModel
                {
                    Player1Name = Session["Player1"].ToString(),
                    Player2Name = Session["Player2"].ToString()
                };
                return View(model);
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartGame(string player1Text, string player2Text)
        {
           
            var value = "";
            var score1 = "";

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MAX(VersusID) FROM VS";
                conn.Open();
                rd = cmd.ExecuteReader();
                while (rd.Read())
                    value = rd.GetValue(0).ToString();
                rd.Close();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Score1 FROM SCORE WHERE VersusID = ?";
                cmd.Parameters.AddWithValue("VersusID", Int32.Parse(value));
                rd = cmd.ExecuteReader();
                while (rd.Read())
                    score1 = rd.GetValue(0).ToString();
                rd.Close();

                conn.Close();
                if (!score1.Equals(""))
                {
                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO VS (Player1, Player2) VALUES (?, ?)";
                    cmd.Parameters.AddWithValue("Player1", player1Text);
                    cmd.Parameters.AddWithValue("Player2", player2Text);

                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();


                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT MAX(VersusID) FROM VS";

                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                        value = rd.GetValue(0).ToString();
                    rd.Close();
                    conn.Close();
                }

                else
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE VS SET Player1 = ? , Player2 = ? WHERE VersusID = ? ";
                    cmd.Parameters.AddWithValue("Player1", player1Text);
                    cmd.Parameters.AddWithValue("Player2", player2Text);
                    cmd.Parameters.AddWithValue("VersusID", Int32.Parse(value));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                Session.Add("VersusID", value);
                Session.Add("Permission", "Access");
                Session.Add("Player1", player1Text);
                Session.Add("Player2", player2Text);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveResult(string score0, string score1)
        {
            var versusId = Int32.Parse(Session["VersusID"].ToString());
            cmd = new OleDbCommand();
            conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\RollDiceGameDb.mdb;Persist Security Info=True");

            if (Int32.Parse(score0) >= 10 || Int32.Parse(score1) >= 10)
            {
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO SCORE (VersusID, Score1, Score2) VALUES (?, ?, ?)";
                    cmd.Parameters.AddWithValue("VersusID", Int32.Parse(Session["VersusID"].ToString()));
                    cmd.Parameters.AddWithValue("Score1", Int32.Parse(score0));
                    cmd.Parameters.AddWithValue("Score2", Int32.Parse(score1));

                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
            return null;
        }

        public ActionResult Scores()
        {
            var scores = new List<Scores>();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM Score as sc INNER JOIN VS as vs ON vs.VersusID = sc.VersusID";
            cmd.Connection = conn;
            conn.Open();
            cmd.ExecuteNonQuery();

            rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var model = new Scores()
                {
                    Player1 = rd.GetValue(5).ToString(),
                    Player2 = rd.GetValue(6).ToString(),
                    VersusId = Int32.Parse(rd.GetValue(1).ToString()),
                    Score1 = rd.GetValue(2).ToString(),
                    Score2 = rd.GetValue(3).ToString()
                };
                scores.Add(model);
            }
              
            rd.Close();
            conn.Close();
            return View(scores);
        }

        public ActionResult newGame()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}