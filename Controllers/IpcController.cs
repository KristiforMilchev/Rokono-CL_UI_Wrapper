namespace RokonoDbManager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using RokonoDbManager.DatabaseController;
    using RokonoDbManager.Models;

    public class IpcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        internal string CastToString(object obj)
        {
            return obj as string;
        }
    }
}