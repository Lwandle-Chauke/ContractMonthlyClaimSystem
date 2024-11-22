using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PROGPart2.ViewModels;
using FluentValidation;

public class AccountController : Controller
{
    private readonly CMCSContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    private readonly IValidator<Claim> _validator;

    public AccountController(CMCSContext context, IValidator<Claim> validator)
    {
        _context = context;
        _validator = validator;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Role = model.Role // Save the role
            };

            // Hash the password after user is created
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success)
            {
                TempData["Message"] = "Login successful!";
                TempData["UserName"] = user.Name; // Store user name in TempData
                TempData["UserRole"] = user.Role; // Store user role in TempData
                // After successful login
                TempData["UserEmail"] = user.Email; // Save lecturer's email in TempData


                return RedirectToAction("RoleLanding", new { role = user.Role });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
        }
        return View(model);
    }

    // Role Landing Method
    public IActionResult RoleLanding(string role)
    {
        switch (role)
        {
            case "Lecturer":
                return RedirectToAction("LecturerLanding");
            case "ProgrammeCoordinator/Manager":
                return RedirectToAction("ProgrammeCoordinatorLanding");
            default:
                TempData["ErrorMessage"] = "Invalid role. Please contact support.";
                return RedirectToAction("Login");
        }
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


// GET: /Account/TrackClaims
public async Task<IActionResult> TrackClaims()
    {
        var lecturerEmail = TempData["UserEmail"]?.ToString(); // Assuming you have stored the email in TempData
        var claims = await _context.Claims
            .Where(c => c.LecturerEmail == lecturerEmail) // Filter claims by lecturer email
            .ToListAsync();

        return View(claims); // Pass claims to the view
    }

    // Programme Coordinator Action to Accept/Reject Claims
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClaimStatus(int claimId, string status)
    {
        if (string.IsNullOrEmpty(status) || !new[] { "Accepted", "Rejected", "Pending" }.Contains(status))
        {
            TempData["ErrorMessage"] = "Invalid status selection.";
            return RedirectToAction("ReviewClaims", "Claim");
        }

        var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == claimId);
        if (claim == null)
        {
            TempData["ErrorMessage"] = "Claim not found.";
            return RedirectToAction("ReviewClaims", "Claim");
        }

        claim.Status = status;

        _context.Entry(claim).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        TempData["Message"] = "Claim status updated successfully!";
        return RedirectToAction("ReviewClaims", "Claim");
    }

    // View Claim Details
    public async Task<IActionResult> ViewClaimDetails(int id)
    {
        var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
        if (claim == null)
        {
            return NotFound();
        }
        return View(claim);
    }

    // GET: Programme Coordinator Landing Page
    public async Task<IActionResult> ProgrammeCoordinatorLanding()
    {
        var claims = await _context.Claims
            .Where(c => c.Status == "Pending")
            .ToListAsync();

        return View(claims);
    }

    // GET: /Account/ManageProfile
    public async Task<IActionResult> ManageProfile()
    {
        var userEmail = TempData["UserEmail"]?.ToString();

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // Lecturer landing page
    public IActionResult LecturerLanding()
    {
        var userName = TempData["UserName"]?.ToString() ?? "Lecturer";
        ViewBag.UserName = userName;

        return View();
    }

    // Success page for login/registration
    public IActionResult Success()
    {
        return View();
    }


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
