using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PROGPart2.ViewModels;

public class AccountController : Controller
{
    private readonly CMCSContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public AccountController(CMCSContext context)
    {
        _context = context;
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

    // Login Method
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
                TempData["UserEmail"] = user.Email; // Store user email in TempData

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
                return Content("Invalid role. Please contact support."); // Show an error message
        }
    }

    // GET: /Account/SubmitClaim
    public IActionResult SubmitClaim()
    {
        return View();
    }

    // POST: /Account/SubmitClaim
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitClaim(DateTime StartDate, DateTime EndDate, int HoursWorked, decimal HourlyRate, string University, IFormFile ClaimDocument)
    {
        var totalAmount = HoursWorked * HourlyRate;

        // Validate the claim data
        if (ClaimDocument != null && ClaimDocument.Length > 0)
        {
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(ClaimDocument.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(string.Empty, "Invalid file type. Only PDF, DOC, and XLSX files are allowed.");
                return View();
            }

            const long maxFileSize = 8 * 1024 * 1024; // 8 MB
            if (ClaimDocument.Length > maxFileSize)
            {
                ModelState.AddModelError(string.Empty, "File size must not exceed 8 MB.");
                return View();
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, ClaimDocument.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ClaimDocument.CopyToAsync(fileStream);
            }
        }

        // Safely handle potential nulls
        string documentPath = ClaimDocument?.FileName ?? "default_filename"; // Handle possible null filename
        string lecturerEmail = TempData["UserEmail"]?.ToString() ?? "default_email@example.com"; // Handle possible null email

        var claim = new Claim
        {
            StartDate = StartDate,
            EndDate = EndDate,
            HoursWorked = HoursWorked,
            HourlyRate = HourlyRate,
            University = University,
            DocumentPath = documentPath, // Save the file name or path
            LecturerEmail = lecturerEmail, // Retrieve the email from TempData
            Status = "Pending" // Default status
        };

        // Add the claim to the context and save
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Claim submitted successfully! Total Amount: {totalAmount.ToString("C", new System.Globalization.CultureInfo("en-ZA"))}";
        return RedirectToAction("LecturerLanding");
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
        // Retrieve the claim from the database based on the claimId
        var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == claimId);

        if (claim != null)
        {
            // Update the status to either "Accepted" or "Rejected" based on the form submission
            claim.Status = status;

            // Save the updated claim status back to the database
            await _context.SaveChangesAsync();
        }

        // Redirect to the list of pending claims for Programme Coordinator after status update
        return RedirectToAction("ReviewClaims", "Claim");
    }

    public async Task<IActionResult> ViewClaimDetails(int id)
    {
        var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id); // Fetch claim from the database by Id
        if (claim == null)
        {
            return NotFound(); // Handle if claim doesn't exist
        }
        return View(claim); // Pass the claim model to the view
    }

    // GET: Programme Coordinator Landing Page
    public async Task<IActionResult> ProgrammeCoordinatorLanding()
    {
        var claims = await _context.Claims
            .Where(c => c.Status == "Pending") // Get pending claims for review
            .ToListAsync();

        return View(claims); // Pass claims to the view
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

        return View(user); // Pass the user model to the view
    }

    // Lecturer landing page
    public IActionResult LecturerLanding()
    {
        // Retrieve the user name from TempData
        var userName = TempData["UserName"]?.ToString() ?? "Lecturer";
        ViewBag.UserName = userName;

        return View();
    }

    // Success page for login/registration
    public IActionResult Success()
    {
        return View();
    }
}
