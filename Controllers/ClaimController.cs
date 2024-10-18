using Microsoft.EntityFrameworkCore; // Necessary for ToListAsync and other async EF methods
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class ClaimController : Controller
{
    private readonly CMCSContext _context;

    public ClaimController(CMCSContext context)
    {
        _context = context;
    }

    // GET: /Claim/ReviewClaims
    public async Task<IActionResult> ReviewClaims()
    {
        // Retrieves all claims with "Pending" status
        var claims = await _context.Claims
            .Where(c => c.Status == "Pending")
            .ToListAsync();

        if (claims == null || !claims.Any())
        {
            ViewBag.Message = "No pending claims for review.";
        }

        return View(claims);
    }

    // POST: /Claim/UpdateClaimStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClaimStatus(int id, string status)
    {
        // Find the claim by ID
        var claim = await _context.Claims.FindAsync(id);

        if (claim == null)
        {
            return NotFound(); // Handle the case where the claim does not exist
        }

        // Update the status of the claim to either "Accepted" or "Rejected"
        claim.Status = status; // "Accepted" or "Rejected"

        // Save the updated claim status back to the database
        await _context.SaveChangesAsync();

        return RedirectToAction("ReviewClaims"); // Redirect back to the review claims page
    }

    // GET: /Claim/TrackClaims
    public async Task<IActionResult> TrackClaims()
    {
        // Get the lecturer's email from session or authenticated user
        var lecturerEmail = TempData["UserEmail"]?.ToString();

        if (string.IsNullOrEmpty(lecturerEmail))
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if email is not found
        }

        // Retrieve all claims (regardless of status) associated with the logged-in lecturer
        var claims = await _context.Claims
            .Where(c => c.LecturerEmail == lecturerEmail) // Filter claims by the lecturer's email
            .ToListAsync();

        if (claims == null || !claims.Any())
        {
            ViewBag.Message = "No claims found.";
        }

        return View(claims); // Pass the claims to the view
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
}
