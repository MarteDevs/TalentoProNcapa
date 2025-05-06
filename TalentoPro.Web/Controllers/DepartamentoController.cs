using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TalentoPro.Application.Excepciones;
using TalentoPro.Application.Models;
using TalentoPro.Application.Servicios;

namespace TalentoPro.Web.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly IDepartamentoServicio _departamentoServicio;

        public DepartamentoController(IDepartamentoServicio departamentoServicio)
        {
            _departamentoServicio = departamentoServicio ?? throw new ArgumentNullException(nameof(departamentoServicio));
        }

        // GET: Departamento
        public async Task<IActionResult> Index()
        {
            try
            {
                var departamentos = await _departamentoServicio.ListarDepartamentos();
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
                var departamento = await _departamentoServicio.ObtenerDepartamentoPorId(id);
                return View(departamento);
            }
            catch (DepartamentoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Departamento/Create
        public IActionResult Create()
        {
            return View(new DepartamentoModel());
        }

        // POST: Departamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartamentoModel departamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _departamentoServicio.RegistrarDepartamento(departamento);
                    if (resultado)
                    {
                        TempData["Success"] = "Departamento creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", "No se pudo crear el departamento.");
                }
                return View(departamento);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(departamento);
            }
        }

        // GET: Departamento/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var departamento = await _departamentoServicio.ObtenerDepartamentoPorId(id);
                return View(departamento);
            }
            catch (DepartamentoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Departamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartamentoModel departamento)
        {
            try
            {
                if (id != departamento.IdDepartamento)
                {
                    return BadRequest("ID de departamento no coincide");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _departamentoServicio.ActualizarDepartamento(departamento);
                    if (resultado)
                    {
                        TempData["Success"] = "Departamento actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", "No se pudo actualizar el departamento.");
                }
                return View(departamento);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(departamento);
            }
        }

        // GET: Departamento/ChangeStatus/5
        public async Task<IActionResult> ChangeStatus(int id)
        {
            try
            {
                var departamento = await _departamentoServicio.ObtenerDepartamentoPorId(id);
                return View(departamento);
            }
            catch (DepartamentoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Departamento/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, char estado)
        {
            try
            {
                if (estado != 'A' && estado != 'I')
                {
                    ModelState.AddModelError("", "Estado no válido. Debe ser A (Activo) o I (Inactivo)");
                    var departamento = await _departamentoServicio.ObtenerDepartamentoPorId(id);
                    return View(departamento);
                }

                var resultado = await _departamentoServicio.CambiarEstadoDepartamento(id, estado);
                if (resultado)
                {
                    TempData["Success"] = "Estado del departamento cambiado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = "No se pudo cambiar el estado del departamento.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Departamento/Empleados/5
        public IActionResult Empleados(int id)
        {
            return RedirectToAction("ByDepartment", "Empleado", new { id });
        }
    }
} 