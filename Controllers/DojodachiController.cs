using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DojoDachi.Controllers
{
    public class DojoDachiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObjectFromJson<GameState>("GS") == null)
            {
                HttpContext.Session.SetObjectAsJson("GS", new GameState());
            }

            ViewBag.GameState = HttpContext.Session.GetObjectFromJson<GameState>("GS");
            ViewBag.Message = "Feed, Play, Work, or Sleep!";
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = "";

            return View();
        }

        [HttpPost]
        [Route("performAction")]
        public IActionResult PerformAction(string action)
        {
            GameState EditDachi = HttpContext.Session.GetObjectFromJson<GameState>("GS");
            Random RandObject = new Random();
            ViewBag.GameStatus = "running";
            switch(action)
            {
                case "feed":
                    if(EditDachi.Meals > 0){
                        EditDachi.Meals -= 1;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            EditDachi.Fullness += RandObject.Next(5, 11);
                            ViewBag.Reaction = ":)";
                            ViewBag.Message = "YUM!";
                        }
                        else
                        {
                            ViewBag.Reaction = ":(";
                            ViewBag.Message = "YUCK!";
                        }
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "Out of food!!";
                    }
                    break;
                case "play":
                    if(EditDachi.Energy > 4)
                    {
                        EditDachi.Energy -= 5;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            EditDachi.Happiness += RandObject.Next(5, 11);
                            ViewBag.Reaction = ":)";
                            ViewBag.Message = "Weee! FUN!";
                        }
                        else
                        {
                            ViewBag.Reaction = ":(";
                            ViewBag.Message = "...meh, not in the mood for playing.";
                        }
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "Famished...";
                    }

                    break;
                case "work":
                    if(EditDachi.Energy > 4)
                    {
                        EditDachi.Energy -= 5;
                        EditDachi.Meals += RandObject.Next(1, 4);
                        ViewBag.Reaction = ":)";
                        ViewBag.Message = "Hard Work Pays Off!!";
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "...Famished";
                    }
                    break;
                case "sleep":
                    EditDachi.Energy += 15;
                    EditDachi.Fullness -= 5;
                    EditDachi.Happiness -= 5;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = "Yawn.";
                    break;
                default:
                    // Handle unxpected values submitted from form
                    ViewBag.Reaction = "XXXXXXXXXXXXXX";
                    ViewBag.Message = "Eagle One, Fox Two - MAN DOWN! (Something went wrong)";
                    break;

            }
            if(EditDachi.Fullness < 1 || EditDachi.Happiness < 1)
            {
                ViewBag.Reaction = "X(";
                ViewBag.Message = "Dojodachi died...";
                ViewBag.GameStatus = "over";
            }
            else if(EditDachi.Fullness > 99 && EditDachi.Happiness > 99)
            {
                ViewBag.Reaction = "!METAMORPHOSIS!";
                ViewBag.Message = "LEVEL UP!!";
                EditDachi.Fullness = 20;
                EditDachi.Happiness = 20;
                EditDachi.Meals = 3;
                EditDachi.Energy = 50;
                EditDachi.Level++;
            }
            else if(EditDachi.Level == 6 && EditDachi.Fullness > 99 && EditDachi.Happiness > 99)
            {
                ViewBag.Reaction = "RAWR!";
                ViewBag.Message = "YOU WIN!";
                ViewBag.GameStatus = "over";
            }
            HttpContext.Session.SetObjectAsJson("GS", EditDachi);
            ViewBag.GameState = EditDachi;
            return View("Index");
        }

        [HttpGet]
        [Route("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }

    public static class SessionExtensions
{
    // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        // This helper function simply serializes theobject to JSON and stores it as a string in session
        session.SetString(key, JsonConvert.SerializeObject(value));
    }
       
    // generic type T is a stand-in indicating that we need to specify the type on retrieval
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        // Upone retrieval the object is deserialized based on the type we specified
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
}