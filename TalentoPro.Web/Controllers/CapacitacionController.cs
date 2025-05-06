using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentoPro.Application.Excepciones;
using TalentoPro.Application.Models;
using TalentoPro.Application.Servicios;

namespace TalentoPro.Web.Controllers
{
    public class CapacitacionController : Controller
    {
        private readonly ICapacitacionServicio _capacitacionServicio;

        public CapacitacionController(ICapacitacionServicio capacitacionServicio)
        {
            _capacitacionServicio = capacitacionServicio ?? throw new ArgumentNullException(nameof(capacitacionServicio));
        }

        // GET: Capacitacion
        public async Task<IActionResult> Index()
        {
            try
            {
                var capacitaciones = await _capacitacionServicio.ListarCapacitaciones();
                return View(capacitaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Capacitacion/Details/CAPXXX
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Capacitacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Capacitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CapacitacionSolicitudModel capacitacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _capacitacionServicio.RegistrarCapacitacion(capacitacion);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(capacitacion);
            }
        }

        // GET: Capacitacion/Edit/CAPXXX
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                
                var capacitacionSolicitud = new CapacitacionSolicitudModel
                {
                    IdCapacitacion = capacitacion.IdCapacitacion,
                    NombreCurso = capacitacion.NombreCurso,
                    Descripcion = capacitacion.Descripcion,
                    Instructor = capacitacion.Instructor,
                    FechaInicio = capacitacion.FechaInicio,
                    FechaFin = capacitacion.FechaFin,
                    DuracionHoras = capacitacion.DuracionHoras,
                    Modalidad = capacitacion.Modalidad
                };
                
                return View(capacitacionSolicitud);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Capacitacion/Edit/CAPXXX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CapacitacionSolicitudModel capacitacion)
        {
            try
            {
                if (id != capacitacion.IdCapacitacion)
                {
                    return BadRequest("ID de capacitación no coincide");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _capacitacionServicio.ActualizarCapacitacion(capacitacion);
                    if (resultado.Exito)
                    {
                        TempData["Success"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Index));
                    }
                    
                    ModelState.AddModelError("", resultado.Mensaje);
                }
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(capacitacion);
            }
        }

        // GET: Capacitacion/ChangeStatus/CAPXXX
        public async Task<IActionResult> ChangeStatus(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Capacitacion/ChangeStatus/CAPXXX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, char estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var resultado = await _capacitacionServicio.CambiarEstadoCapacitacion(id, estado);
                if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", resultado.Mensaje);
                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Capacitacion/Delete/CAPXXX
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("ID de capacitación no proporcionado");
                }

                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Capacitacion/Delete/CAPXXX
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var resultado = await _capacitacionServicio.EliminarCapacitacion(id);
                if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", resultado.Mensaje);
                var capacitacion = await _capacitacionServicio.ObtenerCapacitacionPorId(id);
                return View(capacitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Capacitacion/Active
        public async Task<IActionResult> Active()
        {
            try
            {
                var capacitaciones = await _capacitacionServicio.ObtenerCapacitacionesPorEstado('A');
                return View("Index", capacitaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Capacitacion/Vigentes
        public async Task<IActionResult> Vigentes()
        {
            try
            {
                var capacitaciones = await _capacitacionServicio.ObtenerCapacitacionesVigentes();
                return View("Index", capacitaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
} 