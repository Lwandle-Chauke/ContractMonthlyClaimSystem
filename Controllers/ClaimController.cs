using Microsoft.EntityFrameworkCore; // Necessary for ToListAsync and other async EF methods
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PROGPart2.Models.ViewModels;

public class ClaimController : Controller
{
    private readonly CMCSContext _context;

    public ClaimController(CMCSContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ReviewClaims()
    {
        var claims = await _context.Claims
            .Where(c => c.Status == "Pending")
            .ToListAsync();

        if (claims == null || !claims.Any())
        {
            ViewBag.Message = "No pending claims for review.";
        }

        return View(claims);
    }

    public IActionResult UpdateClaimStatus(int claimId)
    {
        var claim = _context.Claims.FirstOrDefault(c => c.Id == claimId);

        if (claim == null)
        {
            return NotFound();
        }

        var viewModel = new UpdateClaimStatusViewModel
        {
            ClaimId = claim.Id,
            LecturerName = claim.LecturerName,
            CurrentStatus = claim.Status,
            StatusOptions = new List<string> { "Pending", "Accepted", "Rejected" }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClaimStatus(UpdateClaimStatusViewModel model)
    {
        if (ModelState.IsValid)
        {
            var claim = await _context.Claims.FindAsync(model.ClaimId);

            if (claim != null)
            {
                claim.Status = model.SelectedStatus;
                _context.Entry(claim).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["Message"] = "Claim status updated successfully!";
                return RedirectToAction("TrackClaims");
            }

            ModelState.AddModelError(string.Empty, "Claim not found.");
        }

        return View(model);
    }



    [HttpPost]
    public async Task<IActionResult> SubmitClaim(Claim model)
    {
        // Validation for HoursWorked and HourlyRate
        if (model.HoursWorked <= 0 || model.HourlyRate <= 0)
        {
            TempData["Error"] = "Hours Worked and Hourly Rate must be greater than 0.";
            return View();
        }

        // Validation for StartDate and EndDate
        if (model.StartDate >= model.EndDate)
        {
            TempData["Error"] = "Start Date must be before End Date.";
            return View();
        }

        // Validation for ClaimDocument
        if (model.ClaimDocument == null || model.ClaimDocument.Length == 0)
        {
            TempData["Error"] = "Claim document is required.";
            return View();
        }

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
        var fileExtension = Path.GetExtension(model.ClaimDocument.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            TempData["Error"] = "Invalid file type. Only PDF, DOC, DOCX, and XLSX files are allowed.";
            return View();
        }

        const long maxFileSize = 8 * 1024 * 1024; // 8 MB
        if (model.ClaimDocument.Length > maxFileSize)
        {
            TempData["Error"] = "File size must not exceed 8 MB.";
            return View();
        }

        // Auto-calculate total amount based on HoursWorked and HourlyRate
        var totalAmount = model.HoursWorked * model.HourlyRate;

        // Define the directory where files will be stored
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "claimdocuments");

        // Check if the directory exists, and create it if it doesn't
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save the uploaded claim document to the directory
        var filePath = Path.Combine(directoryPath, model.ClaimDocument.FileName);
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ClaimDocument.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred while uploading the file: {ex.Message}";
            return View();
        }

        // Store the file path (relative path) in the model for the database
        model.DocumentPath = Path.Combine("claimdocuments", model.ClaimDocument.FileName);

        // Create a new claim object with all the model properties, including the file path
        var claim = new Claim
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            HoursWorked = model.HoursWorked,
            HourlyRate = model.HourlyRate,
            University = model.University,
            DocumentPath = model.DocumentPath,
            LecturerEmail = TempData["UserEmail"]?.ToString(), // Assuming UserEmail is stored in TempData
            LecturerName = model.LecturerName,
            ProgrammeCoordinatorName = model.ProgrammeCoordinatorName,
            Status = "Pending" // Default status
        };

        // Save the claim to the database
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();

        // Display success message
        TempData["Message"] = $"Claim submitted successfully! Total Amount: {totalAmount:C}";
        return RedirectToAction("TrackClaims");
    }

    [HttpPost]
    public async Task<IActionResult> UploadClaimDocument(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            // Process file here, e.g., save it to disk
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update the Claim with the file path or other info
            // Example: _context.Claims.Add(new Claim { ClaimDocumentPath = filePath });
            return RedirectToAction("Index");
        }

        return BadRequest("No file uploaded.");
    }



// GET: /Claim/TrackClaims
public async Task<IActionResult> TrackClaims()
    {
        // Ensure the user's email is retrieved correctly
        var lecturerEmail = TempData["UserEmail"]?.ToString();

        // If email is not found, you may want to handle this case (e.g., redirect to login page)
        if (string.IsNullOrEmpty(lecturerEmail))
        {
            TempData["ErrorMessage"] = "You must be logged in to view your claims.";
            return RedirectToAction("Login", "Account");  // Assuming you have a login page
        }

        // Retrieve claims based on the logged-in user's email (LecturerEmail)
        var claims = await _context.Claims
            .Where(c => c.LecturerEmail == lecturerEmail) // Filter by the logged-in user's email
            .ToListAsync();

        // Check if claims exist and provide a message if no claims found
        if (claims == null || claims.Count == 0)
        {
            TempData["Message"] = "You have no claims at the moment.";
        }

        return View(claims);
    }



    // GET: /Claim/CoordinatorClaims
    [Authorize(Roles = "ProgrammeCoordinator")] // Ensure only Programme Coordinators can access this
    public async Task<IActionResult> CoordinatorClaims()
    {
        // Get the logged-in coordinator's name or email from the authenticated user identity
        var coordinatorName = User?.Identity?.Name;

        if (string.IsNullOrEmpty(coordinatorName))
        {
            // Redirect to login if coordinator name is not found in the user identity
            return RedirectToAction("Login", "Account");
        }

        // Retrieve claims that have been assigned to this coordinator and are pending
        var claims = await _context.Claims
            .Where(c => c.ProgrammeCoordinatorName == coordinatorName && c.Status == "Pending")
            .ToListAsync();

        // If no claims are found, set a message to ViewBag
        if (!claims.Any())
        {
            ViewBag.Message = "No pending claims submitted for review.";
        }

        return View(claims); // Pass the claims to the view for the coordinator
    }



    // POST: /Claim/Create
    [HttpPost]
    public async Task<IActionResult> Create(Claim claim)
    {
        if (ModelState.IsValid)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // Or wherever you'd like to redirect after creation
        }
        return View(claim);
    }

    // GET: /Claim/DownloadClaimsReport
    // GET: /Claim/DownloadClaimsReport
    public async Task<IActionResult> DownloadClaimsReport()
    {
        var lecturerEmail = TempData["UserEmail"]?.ToString();
        if (string.IsNullOrEmpty(lecturerEmail))
        {
            TempData["ErrorMessage"] = "You must be logged in to download your claims.";
            return RedirectToAction("Login", "Account");
        }

        var claims = await _context.Claims
            .Where(c => c.LecturerEmail == lecturerEmail)
            .ToListAsync();

        if (claims == null || claims.Count == 0)
        {
            TempData["Message"] = "No claims found for download.";
            return RedirectToAction("TrackClaims");
        }

        // CSV Header
        var csvContent = "Claim ID,Start Date,End Date,Hours Worked,Hourly Rate,Total Amount\n";

        foreach (var claim in claims)
        {
            // Validate claim data
            var hoursWorked = claim.HoursWorked > 0 ? claim.HoursWorked : 0;
            var hourlyRate = claim.HourlyRate > 0 ? claim.HourlyRate : 0;
            var totalAmount = hoursWorked * hourlyRate;

            // Format data and append to CSV content
            csvContent += $"\"{claim.Id}\",\"{claim.StartDate:yyyy-MM-dd}\",\"{claim.EndDate:yyyy-MM-dd}\"," +
                          $"\"{hoursWorked}\",\"{hourlyRate:F2}\",\"R {totalAmount:F2}\"\n";
        }

        // Encode CSV with UTF-8 BOM for better compatibility
        var fileBytes = System.Text.Encoding.UTF8.GetPreamble()
            .Concat(System.Text.Encoding.UTF8.GetBytes(csvContent))
            .ToArray();

        return File(fileBytes, "text/csv", "claims_report.csv");
    }



}