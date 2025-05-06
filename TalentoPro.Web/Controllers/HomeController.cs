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

        public async Task<IActionResult> Index()
        {
            try
            {
                var empleados = await _empleadoServicio.ListarEmpleados();
                var departamentos = await _departamentoRepository.Listar();
                
                ViewBag.CantidadEmpleados = empleados.Count();
                ViewBag.CantidadDepartamentos = departamentos.Count();
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página principal");
                ViewBag.Error = "Ocurrió un error al cargar los datos. Por favor, inténtelo de nuevo más tarde.";
                return View();
            }
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
