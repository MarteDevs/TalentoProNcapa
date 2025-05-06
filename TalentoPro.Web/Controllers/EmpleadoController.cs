using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentoPro.Application.Excepciones;
using TalentoPro.Application.Models;
using TalentoPro.Application.Servicios;
using TalentoPro.Web.Models;

namespace TalentoPro.Web.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadoServicio _empleadoServicio;
        private readonly IDepartamentoServicio _departamentoServicio;

        public EmpleadoController(IEmpleadoServicio empleadoServicio, IDepartamentoServicio departamentoServicio)
        {
            _empleadoServicio = empleadoServicio ?? throw new ArgumentNullException(nameof(empleadoServicio));
            _departamentoServicio = departamentoServicio ?? throw new ArgumentNullException(nameof(departamentoServicio));
        }

        // Método para cargar la lista de departamentos
        private async Task CargarDepartamentos()
        {
            var departamentos = await _departamentoServicio.ListarDepartamentos();
            ViewBag.Departamentos = new SelectList(departamentos, "IdDepartamento", "Nombre");
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            try
            {
                var empleados = await _empleadoServicio.ListarEmpleados();
                return View(empleados);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Empleado/Details/5
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de empleado no proporcionado");
                }

                var empleado = await _empleadoServicio.ObtenerEmpleadoPorId(id);
                return View(empleado);
            }
            catch (EmpleadoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Empleado/Create
        public async Task<IActionResult> Create()
        {
            await CargarDepartamentos();
            return View();
        }

        // POST: Empleado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoSolicitudModel empleado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _empleadoServicio.RegistrarEmpleado(empleado);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                await CargarDepartamentos();
                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarDepartamentos();
                return View(empleado);
            }
        }

        // GET: Empleado/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de empleado no proporcionado");
                }

                var empleado = await _empleadoServicio.ObtenerEmpleadoPorId(id);
                var empleadoSolicitud = new EmpleadoSolicitudModel
                {
                    IdEmpleado = empleado.IdEmpleado,
                    Nombre = empleado.Nombre,
                    Apellido = empleado.Apellido,
                    FechaNacimiento = empleado.FechaNacimiento,
                    FechaContratacion = empleado.FechaContratacion,
                    IdDepartamento = empleado.IdDepartamento,
                    Cargo = empleado.Cargo,
                    Salario = empleado.Salario,
                    TipoContrato = empleado.TipoContrato,
                    NivelEducacion = empleado.NivelEducacion,
                    EmailCorporativo = empleado.EmailCorporativo
                };
                
                await CargarDepartamentos();
                return View(empleadoSolicitud);
            }
            catch (EmpleadoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Empleado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EmpleadoSolicitudModel empleado)
        {
            try
            {
                if (id != empleado.IdEmpleado)
                {
                    return BadRequest("ID de empleado no coincide");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _empleadoServicio.ActualizarEmpleado(empleado);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                await CargarDepartamentos();
                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarDepartamentos();
                return View(empleado);
            }
        }

        // GET: Empleado/ChangeStatus/5
        public async Task<IActionResult> ChangeStatus(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de empleado no proporcionado");
                }

                var empleado = await _empleadoServicio.ObtenerEmpleadoPorId(id);
                var cambioEstadoViewModel = new CambioEstadoViewModel
                {
                    IdEmpleado = empleado.IdEmpleado,
                    Nombre = empleado.Nombre,
                    Apellido = empleado.Apellido,
                    Estado = empleado.Estado
                };
                
                return View(cambioEstadoViewModel);
            }
            catch (EmpleadoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Empleado/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, CambioEstadoViewModel model)
        {
            try
            {
                if (id != model.IdEmpleado)
                {
                    return BadRequest("ID de empleado no coincide");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _empleadoServicio.CambiarEstadoEmpleado(id, model.NuevoEstado);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: Empleado/ByDepartment/5
        public async Task<IActionResult> ByDepartment(int id)
        {
            try
            {
                var empleados = await _empleadoServicio.ObtenerEmpleadosPorDepartamento(id);
                ViewBag.DepartamentoId = id;
                return View("Index", empleados);
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

        // GET: Empleado/CalculateBenefits/5
        public async Task<IActionResult> CalculateBenefits(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de empleado no proporcionado");
                }

                var empleado = await _empleadoServicio.ObtenerEmpleadoPorId(id);
                var beneficios = await _empleadoServicio.CalcularBeneficiosEmpleado(id);
                
                var beneficiosViewModel = new BeneficiosViewModel
                {
                    IdEmpleado = empleado.IdEmpleado,
                    Nombre = empleado.Nombre,
                    Apellido = empleado.Apellido,
                    Cargo = empleado.Cargo,
                    Salario = empleado.Salario,
                    Antiguedad = empleado.Antiguedad,
                    BeneficiosCalculados = beneficios
                };
                
                return View(beneficiosViewModel);
            }
            catch (EmpleadoNoEncontradoException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
} 