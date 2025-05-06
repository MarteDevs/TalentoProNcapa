using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TalentoPro.Domain.Repositories;

namespace TalentoPro.Web.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly IDepartamentoRepository _departamentoRepository;

        public DepartamentoController(IDepartamentoRepository departamentoRepository)
        {
            _departamentoRepository = departamentoRepository ?? throw new ArgumentNullException(nameof(departamentoRepository));
        }

        // GET: Departamento
        public async Task<IActionResult> Index()
        {
            try
            {
                var departamentos = await _departamentoRepository.Listar();
                return View(departamentos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Departamento/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var departamento = await _departamentoRepository.ObtenerPorId(id);
                if (departamento == null)
                {
                    return NotFound();
                }
                return View(departamento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Departamento/Empleados/5
        public IActionResult Empleados(int id)
        {
            return RedirectToAction("ByDepartment", "Empleado", new { id });
        }
    }
} 