using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using TalentoPro.Application.Excepciones;
using TalentoPro.Application.Models;
using TalentoPro.Application.Servicios;
using System.Linq;

namespace TalentoPro.Web.Controllers
{
    public class InscripcionController : Controller
    {
        private readonly IInscripcionServicio _inscripcionServicio;
        private readonly IEmpleadoServicio _empleadoServicio;
        private readonly ICapacitacionServicio _capacitacionServicio;

        public InscripcionController(
            IInscripcionServicio inscripcionServicio,
            IEmpleadoServicio empleadoServicio,
            ICapacitacionServicio capacitacionServicio)
        {
            _inscripcionServicio = inscripcionServicio ?? throw new ArgumentNullException(nameof(inscripcionServicio));
            _empleadoServicio = empleadoServicio ?? throw new ArgumentNullException(nameof(empleadoServicio));
            _capacitacionServicio = capacitacionServicio ?? throw new ArgumentNullException(nameof(capacitacionServicio));
        }

        // Método para cargar listas de empleados y capacitaciones
        private async Task CargarListasDropdown()
        {
            try
            {
                // Cargar lista de empleados
                var empleados = await _empleadoServicio.ListarEmpleados();
                ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre");

                // Cargar lista de capacitaciones vigentes
                var capacitaciones = await _capacitacionServicio.ObtenerCapacitacionesVigentes();
                
                if (capacitaciones != null && capacitaciones.Any())
                {
                    ViewBag.Capacitaciones = new SelectList(capacitaciones, "IdCapacitacion", "NombreCurso");
                    ViewBag.CapacitacionesCount = capacitaciones.Count();
                }
                else
                {
                    // Si no hay capacitaciones, asignar una lista vacía
                    ViewBag.Capacitaciones = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewBag.CapacitacionesCount = 0;
                }
            }
            catch (Exception ex)
            {
                // En caso de error, inicializar las listas como vacías para evitar errores en la vista
                ViewBag.Empleados = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.Capacitaciones = new SelectList(Enumerable.Empty<SelectListItem>());
                
                // Registrar el error para debugging
                System.Diagnostics.Debug.WriteLine($"Error al cargar listas dropdown: {ex.Message}");
                
                // Opcionalmente, si tienes un servicio de logs, podrías registrar el error aquí
            }
        }

        // GET: Inscripcion
        public async Task<IActionResult> Index()
        {
            try
            {
                var inscripciones = await _inscripcionServicio.ListarInscripciones();
                return View(inscripciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/Details/INSXXX
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasDropdown();
            
            // Verificar si hay capacitaciones vigentes disponibles
            if (ViewBag.CapacitacionesCount == 0)
            {
                TempData["Warning"] = "No hay capacitaciones vigentes disponibles para inscripción. Por favor, contacte al administrador.";
            }
            
            // Generar un ID de inscripción predeterminado con segundos para evitar duplicados
            var inscripcionModel = new InscripcionSolicitudModel
            {
                IdInscripcion = $"INS{DateTime.Now:yyMMddHHmmss}",
                FechaInscripcion = DateTime.Today,
                EstadoInscripcion = 'P'
            };
            
            return View(inscripcionModel);
        }

        // POST: Inscripcion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InscripcionSolicitudModel inscripcion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _inscripcionServicio.RegistrarInscripcion(inscripcion);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        // Redireccionar directamente al índice sin tratar de obtener los detalles
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                
                await CargarListasDropdown();
                // Verificamos si hay capacitaciones disponibles después de recargar las listas
                if (ViewBag.CapacitacionesCount == 0)
                {
                    TempData["Warning"] = "No hay capacitaciones vigentes disponibles para inscripción. Por favor, contacte al administrador.";
                }
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarListasDropdown();
                return View(inscripcion);
            }
        }

        // GET: Inscripcion/Edit/INSXXX
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                
                var inscripcionSolicitud = new InscripcionSolicitudModel
                {
                    IdInscripcion = inscripcion.IdInscripcion,
                    IdEmpleado = inscripcion.IdEmpleado,
                    IdCapacitacion = inscripcion.IdCapacitacion,
                    FechaInscripcion = inscripcion.FechaInscripcion,
                    EstadoInscripcion = inscripcion.EstadoInscripcion,
                    Calificacion = inscripcion.Calificacion,
                    Comentarios = inscripcion.Comentarios
                };
                
                await CargarListasDropdown();

                // Verificar si hay capacitaciones vigentes disponibles
                if (ViewBag.CapacitacionesCount == 0)
                {
                    TempData["Warning"] = "No hay capacitaciones vigentes disponibles para edición. Algunas opciones pueden estar limitadas.";
                }
                
                return View(inscripcionSolicitud);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Inscripcion/Edit/INSXXX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, InscripcionSolicitudModel inscripcion)
        {
            try
            {
                if (id != inscripcion.IdInscripcion)
                {
                    return BadRequest("ID de inscripción no coincide");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _inscripcionServicio.ActualizarInscripcion(inscripcion);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                
                await CargarListasDropdown();
                
                // Verificar si hay capacitaciones vigentes disponibles
                if (ViewBag.CapacitacionesCount == 0)
                {
                    TempData["Warning"] = "No hay capacitaciones vigentes disponibles para edición. Algunas opciones pueden estar limitadas.";
                }
                
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarListasDropdown();
                return View(inscripcion);
            }
        }

        // GET: Inscripcion/UpdateScore/INSXXX
        public async Task<IActionResult> UpdateScore(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Inscripcion/UpdateScore/INSXXX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScore(string id, decimal calificacion, string comentarios)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var resultado = await _inscripcionServicio.ActualizarCalificacion(id, calificacion, comentarios);
                if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Details), new { id });
                }
                
                ModelState.AddModelError("", resultado.Mensaje);
                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/ChangeStatus/INSXXX
        public async Task<IActionResult> ChangeStatus(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Inscripcion/ChangeStatus/INSXXX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, char estadoInscripcion)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var resultado = await _inscripcionServicio.CambiarEstadoInscripcion(id, estadoInscripcion);
                if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", resultado.Mensaje);
                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/Delete/INSXXX
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de inscripción no proporcionado");
                }

                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Inscripcion/Delete/INSXXX
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var resultado = await _inscripcionServicio.EliminarInscripcion(id);
                if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", resultado.Mensaje);
                var inscripcion = await _inscripcionServicio.ObtenerInscripcionPorId(id);
                return View(inscripcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/ByEmpleado/EMP001
        public async Task<IActionResult> ByEmpleado(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de empleado no proporcionado");
                }

                var inscripciones = await _inscripcionServicio.ObtenerInscripcionesPorEmpleado(id);
                
                // Obtener el nombre del empleado para mostrar en la vista
                var empleado = await _empleadoServicio.ObtenerEmpleadoPorId(id);
                ViewBag.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellido}";
                ViewBag.IdEmpleado = id;
                
                return View("Index", inscripciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/ByCapacitacion/CAP001
        public async Task<IActionResult> ByCapacitacion(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var inscripciones = await _inscripcionServicio.ObtenerInscripcionesPorCapacitacion(id);
                
                // Obtener el nombre de la capacitación para mostrar en la vista
                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                ViewBag.NombreCapacitacion = capacitacion.NombreCurso;
                ViewBag.IdCapacitacion = id;
                
                return View("Index", inscripciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Inscripcion/DiagnosticoCapacitaciones
        public async Task<IActionResult> DiagnosticoCapacitaciones()
        {
            try
            {
                var capacitaciones = await _capacitacionServicio.ObtenerCapacitacionesVigentes();
                var todasLasCapacitaciones = await _capacitacionServicio.ListarCapacitaciones();
                
                ViewBag.CapacitacionesVigentes = capacitaciones;
                ViewBag.TodasLasCapacitaciones = todasLasCapacitaciones;
                ViewBag.TieneCapacitacionesVigentes = capacitaciones != null && capacitaciones.Any();
                ViewBag.CantidadVigentes = capacitaciones?.Count() ?? 0;
                ViewBag.CantidadTotal = todasLasCapacitaciones?.Count() ?? 0;
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al obtener diagnóstico de capacitaciones: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inscripcion/CrearCapacitacionPrueba
        public IActionResult CrearCapacitacionPrueba()
        {
            // Redireccionar a la página de creación de capacitaciones
            TempData["Info"] = "Cree una capacitación con estado Activo (A) y fechas vigentes para resolver el problema.";
            return RedirectToAction("Create", "Capacitacion");
        }
    }
} 