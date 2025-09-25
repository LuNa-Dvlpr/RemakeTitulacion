using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.IO;
using System.Linq;
using Titulacion.Models;

namespace Titulacion.Clases
{
    public class ComprobanteCLS
    {
        public FileStream GenerarComprobante(TutoriasContext db, string boletaUsuario, string wwwRootPath)
        {
            try
            {
                var us = db.Usuarios.FirstOrDefault(x => x.User == boletaUsuario);
                var alm = db.Alumno.FirstOrDefault(x => x.IdUsuario == us.IdUsuario);
                var inscrip = db.Inscripcion.FirstOrDefault(x => x.IdAlumno == alm.IdAlumno);
                var prof = db.Profesor.FirstOrDefault(x => x.IdProfesor == inscrip.IdProfesor);

                if (us == null || alm == null || inscrip == null || prof == null)
                {
                    throw new Exception("No se encontraron todos los datos necesarios para generar el comprobante.");
                }

                string nombreArchivo = $"Comprobante_{boletaUsuario}.pdf"; // Cambiado para que el nombre sea más claro y consistente

                FileStream stream = new FileStream(nombreArchivo, FileMode.Create, FileAccess.Write);
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, iText.Kernel.Geom.PageSize.A5.Rotate());
                document.SetMargins(30, 25, 25, 25);

                PdfFont fontNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Leemos las imágenes desde la ruta local
                string rutaLogoIpn = Path.Combine(wwwRootPath, "img", "IPN.png");
                string rutaLogoCecyt = Path.Combine(wwwRootPath, "img", "Cecyt13.jpg");

                ImageData logoIpnData = ImageDataFactory.Create(rutaLogoIpn);
                Image logo1 = new Image(logoIpnData).ScaleToFit(50, 50).SetFixedPosition(1, 23, 300);
                document.Add(logo1);

                ImageData logoCecytData = ImageDataFactory.Create(rutaLogoCecyt);
                Image logo2 = new Image(logoCecytData).ScaleToFit(70, 70).SetFixedPosition(1, 490, 300);
                document.Add(logo2);

                // --- SECCIÓN CLAVE: Asegurándonos de que todo el texto esté aquí ---
                // Encabezados
                document.Add(new Paragraph("Instituto Politécnico Nacional").SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER));
                document.Add(new Paragraph("Centro de Estudios Científicos y Tecnológicos No.13").SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER));
                document.Add(new Paragraph("Ricardo Flores Magón").SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER));
                document.Add(new Paragraph("\n\n")); // Espacios en blanco
                document.Add(new Paragraph("Comprobante de inscripción a tutorías individuales").SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER));
                document.Add(new Paragraph("\n\n")); // Espacios en blanco

                document.Add(new Paragraph("Folio: " + inscrip.Folio).SetFont(fontBold).SetTextAlignment(TextAlignment.LEFT));
                document.Add(new Paragraph("\n")); // Espacio en blanco

                // Tablas con información
                Table tabl1 = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                tabl1.AddCell(new Cell().Add(new Paragraph($"Alumno: {alm.ApellidoPat} {alm.ApellidoMat} {alm.Nombre}").SetFont(fontNormal)).SetBorder(Border.NO_BORDER));
                tabl1.AddCell(new Cell().Add(new Paragraph("Boleta:" + boletaUsuario).SetFont(fontNormal)).SetBorder(Border.NO_BORDER));
                document.Add(tabl1);

                Table tabl2 = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                tabl2.AddCell(new Cell().Add(new Paragraph($"Profesor: {prof.ApellidoPat} {prof.ApellidoMat} {prof.Nombre}").SetFont(fontNormal)).SetBorder(Border.NO_BORDER));
                tabl2.AddCell(new Cell().Add(new Paragraph("Grupo:" + alm.Grupo).SetFont(fontNormal)).SetBorder(Border.NO_BORDER));
                document.Add(tabl2);

                document.Add(new Paragraph("\n")); // Espacio en blanco
                document.Add(new Paragraph($"Comprobante generado el {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm:ss} hrs.").SetFont(fontBold));

                document.Close();

                return new FileStream(nombreArchivo, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                // Este bloque se encarga de generar un PDF de error si algo falla
                string errorArchivo = "error.pdf";
                var writer = new PdfWriter(errorArchivo);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                document.Add(new Paragraph("Ocurrió un error al generar el comprobante."));
                document.Add(new Paragraph("Detalle: " + ex.Message)); // Muestra el mensaje de la excepción para depuración
                document.Close();
                return new FileStream(errorArchivo, FileMode.Open, FileAccess.Read);
            }
        }
    }
}