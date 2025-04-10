using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using Spire.Pdf;
using System.Linq.Expressions;

namespace PrinterService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrinterController : ControllerBase
    {
        private readonly ILogger<PrinterController> _logger;

        public PrinterController(ILogger<PrinterController> logger)
        {
            _logger = logger;
        }
        [HttpGet("list")]
        public IActionResult GetPrinters()
        {
            try
            {
                //obtener la lista de de impresoras instaladas 
                var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                if (printers.Count == 0)
                {
                    return NotFound("No se encontraron Impresoras instaladas");
                }
                return Ok(printers);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al obtener lista de impresoras");
                return StatusCode(500, "Error interno del servidor al listar impresoras");
            }
        }

        [HttpPost("printer-ticket")]
        public IActionResult PrintTicket(IFormFile file, [FromForm] string printerName) 
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No se envio ningun archivo.");
                }
                if (string.IsNullOrEmpty(printerName))
                {
                    return BadRequest("No se ingreso nombre de Impresora");
                }

                //Guardar Temporalmente el archivo

                var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf ");
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                //cargar el pdf 
                var pdfDocument = new PdfDocument();
                pdfDocument.LoadFromFile(tempFilePath);

                //configurar impresora
                pdfDocument.PrintSettings.PrinterName = printerName;
                pdfDocument.PrintSettings.SelectPageRange(1, pdfDocument.Pages.Count);
                pdfDocument.PrintSettings.SelectSinglePageLayout(Spire.Pdf.Print.PdfSinglePageScalingMode.FitSize);

                //enviar a imprimir
                pdfDocument.Print();

                //Liberar recursos
                pdfDocument.Close();

                //Eliminar el archivo temporal
                System.IO.File.Delete(tempFilePath);

                return Ok("PDF enviado a imprimir correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al imprimir el pdf: {ex.Message}");
            }
        }
    }
}
