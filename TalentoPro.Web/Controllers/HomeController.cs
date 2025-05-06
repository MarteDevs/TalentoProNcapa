using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TalentoPro.Application.Servicios;
using TalentoPro.Domain.Repositories;
using TalentoPro.Web.Models;

namespace TalentoPro.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmpleadoServicio _empleadoServicio;
        private readonly IDepartamentoRepository _departamentoRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IEmpleadoServicio empleadoServicio,
            IDepartamentoRepository departamentoRepository)
        {
            _logger = logger;
            _empleadoServicio = empleadoServicio ?? throw new ArgumentNullException(nameof(empleadoServicio));
            _departamentoRepository = departamentoRepository ?? throw new ArgumentNullException(nameof(departamentoRepository));
        }

        public IActionResult Index()
        {
            // Redirigir a la página principal de empleados
            return RedirectToAction("Index", "Empleado");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
