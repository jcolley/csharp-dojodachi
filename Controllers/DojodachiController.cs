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
            GameState UpdateDachi = HttpContext.Session.GetObjectFromJson<GameState>("GS");
            Random RandObject = new Random();
            ViewBag.GameStatus = "running";
            switch(action)
            {
                case "feed":
                    if(UpdateDachi.Meals > 0){
                        UpdateDachi.Meals -= 1;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            UpdateDachi.Fullness += RandObject.Next(5, 11);
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
                    if(UpdateDachi.Energy > 4)
                    {
                        UpdateDachi.Energy -= 5;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            UpdateDachi.Happiness += RandObject.Next(5, 11);
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
                    if(UpdateDachi.Energy > 4)
                    {
                        UpdateDachi.Energy -= 5;
                        UpdateDachi.Meals += RandObject.Next(1, 4);
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
                    UpdateDachi.Energy += 15;
                    UpdateDachi.Fullness -= 5;
                    UpdateDachi.Happiness -= 5;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = "Yawn.";
                    break;
                default:
                    // Handle unxpected values submitted from form
                    ViewBag.Reaction = "XXXXXXXXXXXXXX";
                    ViewBag.Message = "Eagle One, Fox Two - MAN DOWN! (Something went wrong)";
                    break;

            }
            if(UpdateDachi.Fullness < 1 || UpdateDachi.Happiness < 1)
            {
                ViewBag.Reaction = "X(";
                ViewBag.Message = "Dojodachi died...";
                ViewBag.GameStatus = "over";
            }
            else if(UpdateDachi.Fullness > 99 && UpdateDachi.Happiness > 99)
            {
                ViewBag.Reaction = "!METAMORPHOSIS!";
                ViewBag.Message = "!!LEVEL UP!!";
                UpdateDachi.Fullness = 20;
                UpdateDachi.Happiness = 20;
                UpdateDachi.Meals = 3;
                UpdateDachi.Energy = 50;
                UpdateDachi.Level++;
            }
            else if(UpdateDachi.Level == 6 && UpdateDachi.Fullness > 99 && UpdateDachi.Happiness > 99)
            {
                ViewBag.Reaction = "RAWR!";
                ViewBag.Message = "YOU WIN!";
                ViewBag.GameStatus = "over";
            }
            HttpContext.Session.SetObjectAsJson("GS", UpdateDachi);
            ViewBag.GameState = UpdateDachi;
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