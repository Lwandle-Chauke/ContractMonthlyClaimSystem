namespace PROGPart2.Models.ViewModels
{
    public class HRDashboardViewModel
    {
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<LecturerViewModel> Lecturers { get; set; } = new List<LecturerViewModel>();
    }
}
