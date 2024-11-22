using Microsoft.AspNetCore.Mvc;
using PROGPart2.Models;
using PROGPart2.Models.ViewModels;
using System.Linq;
using System.IO; // For MemoryStream
using iTextSharp.text;
using iTextSharp.text.pdf; // For PDF generation
using System;

namespace PROGPart2.Controllers
{
    namespace PROGPart2.Controllers
    {
        public class HRController : Controller
        {
            private readonly CMCSContext _context;

            public HRController(CMCSContext context)
            {
                _context = context;
            }

            // Display the HR Dashboard
            public IActionResult HRDashboard()
            {
                var lecturers = _context.Users
                    .Where(u => u.Role == "Lecturer")
                    .Select(u => new LecturerViewModel
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        HoursWorked = _context.Claims
                            .Where(c => c.LecturerEmail == u.Email)
                            .Sum(c => c.HoursWorked)
                    })
                    .ToList();

                var model = new HRDashboardViewModel
                {
                    Claims = _context.Claims.ToList(),
                    Lecturers = lecturers
                };

                return View(model);
            }

            // Handle report generation for Claims or Lecturers
            [HttpPost]
            public IActionResult GenerateReport(string reportType)
            {
                if (reportType == "claims")
                {
                    return GenerateClaimsReportPdf();
                }
                else if (reportType == "lecturers")
                {
                    return GenerateLecturerActivityReportPdf();
                }

                TempData["Error"] = "Invalid report type.";
                return RedirectToAction("HRDashboard");
            }

            // Generate Claims Report as a PDF
            public FileResult GenerateClaimsReportPdf()
            {
                var claims = _context.Claims.ToList();
                var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                using (var ms = new MemoryStream())
                using (var doc = new Document(PageSize.A4, 10f, 10f, 20f, 20f))
                {
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // Add title and date
                    doc.Add(new Paragraph($"Claims Report (Generated on {currentDate})", FontFactory.GetFont("Arial", 16, Font.BOLD)));
                    doc.Add(new Paragraph("\n"));

                    // Create table
                    PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.AddCell("Claim ID");
                    table.AddCell("Lecturer Email");
                    table.AddCell("Status");
                    table.AddCell("Amount");

                    foreach (var claim in claims)
                    {
                        table.AddCell(claim.Id.ToString());
                        table.AddCell(claim.LecturerEmail);
                        table.AddCell(claim.Status);
                        table.AddCell($"R {claim.HoursWorked * claim.HourlyRate:F2}");
                    }

                    doc.Add(table);
                    doc.Close();

                    return File(ms.ToArray(), "application/pdf", "ClaimsReport.pdf");
                }
            }

            // Generate Lecturer Activity Report as a PDF
            public FileResult GenerateLecturerActivityReportPdf()
            {
                var lecturers = _context.Users
                    .Where(u => u.Role == "Lecturer")
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        HoursWorked = _context.Claims
                            .Where(c => c.LecturerEmail == u.Email)
                            .Sum(c => c.HoursWorked)
                    })
                    .ToList();

                var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                using (var ms = new MemoryStream())
                using (var doc = new Document(PageSize.A4, 10f, 10f, 20f, 20f))
                {
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // Add title and date
                    doc.Add(new Paragraph($"Lecturer Activity Report (Generated on {currentDate})", FontFactory.GetFont("Arial", 16, Font.BOLD)));
                    doc.Add(new Paragraph("\n"));

                    // Create table
                    PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.AddCell("Lecturer ID");
                    table.AddCell("Name");
                    table.AddCell("Email");
                    table.AddCell("Total Work Hours");

                    foreach (var lecturer in lecturers)
                    {
                        table.AddCell(lecturer.Id.ToString());
                        table.AddCell(lecturer.Name);
                        table.AddCell(lecturer.Email);
                        table.AddCell(lecturer.HoursWorked.ToString());
                    }

                    doc.Add(table);
                    doc.Close();

                    return File(ms.ToArray(), "application/pdf", "LecturerActivityReport.pdf");
                }
            }
        }
    }
}
