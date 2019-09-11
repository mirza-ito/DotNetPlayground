﻿using Cleaners.Web.Constants;
using Cleaners.Web.Infrastructure.Files;
using Cleaners.Web.Services;
using Corvo.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Cleaners.Web.Controllers
{
    [Authorize]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ICsvFileService _csvFileService;
        private readonly ICorvoFileProvider _fileProvider;
        private readonly ISelectListProviderService _selectListProviderService;

        public HomeController(ICsvFileService csvFileService, ICorvoFileProvider fileProvider, ISelectListProviderService selectListProviderService)
        {
            _csvFileService = csvFileService ?? throw new ArgumentNullException(nameof(csvFileService));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _selectListProviderService = selectListProviderService ?? throw new ArgumentNullException(nameof(selectListProviderService));
        }

        [Route("", Name = HomeRoutes.Index)]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Submit(string value)
        {
            return Json($"Action: {nameof(Submit)}, with param {value}");
        }

        [HttpPost]
        [ActionName(nameof(Submit))]
        //[FormValueRequired()]
        [FormValueRequired("submit2")]
        public IActionResult Submit2(string value)
        {
            return Json($"Action: {nameof(Submit2)}, with param {value}");
        }

        [Route("test")]
        public IActionResult Test()
        {
            try
            {
                throw new FileNotFoundException();
            }
            catch (FileNotFoundException)
            {
                return Conflict($"FileNotFound");
            }
            catch (Exception ex)
            {
                return Conflict($"{(ex.StackTrace)}  /  {ex.Message}");
            }
        }

        [Route("about", Name = HomeRoutes.About)]
        public IActionResult About() => View();
    }
}