using Group_Project.Data;
using Group_Project.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Group_Project.Pages
{
    public class AdminModel : PageModel
    {
        [BindProperty]
        [Display(Name = "Site post")]
        [Required(ErrorMessage = "Please enter text for the post")]
        [StringLength(100)]
        public string Post { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        private readonly IConfiguration configuration;

        private readonly MyApplicationDbContext _myApplicationDbContext;

        public AdminModel(MyApplicationDbContext myApplicationDbContext, IConfiguration _configuration)
        {
            _myApplicationDbContext = myApplicationDbContext;
            configuration = _configuration;
        }

        public List<Emails> Emails { get; set; } = new List<Emails>();

        public IActionResult OnGet()
        {
            var Username = HttpContext.Session.GetString("Username");
            if (Username == null)
            {
                return Redirect("/AdminLogin");
            }

            IQueryable<Emails> EmailsIQ = from P in _myApplicationDbContext.Emails select P;
            Emails = EmailsIQ.ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            //Check if the ModelState is valid, validation passed.
            if (ModelState.IsValid)
            {
                //Connect to database
                var strConn = configuration.GetConnectionString("DefaultConnection");

                //Create an instance of the SqlConnection Class
                using SqlConnection sqlConn = new(strConn);
                SqlCommand AddEmailCmd = new("spAddPost", sqlConn);
                AddEmailCmd.CommandType = CommandType.StoredProcedure;

                //Build our input parameter.
                AddEmailCmd.Parameters.AddWithValue("@Text", Post);

                try
                {
                    //Open the database
                    sqlConn.Open();

                    //use ExecuteNonQuery to execute our stored procedure
                    AddEmailCmd.ExecuteNonQuery();

                    Message = "Post successfully added";

                    //Clear the fields
                    Post = "";
                    ModelState.Clear();

                    //return
                    return this.OnGet();
                }
                catch (Exception ex)
                {
                    Message = "Post could not be added at this time, please try again later - " + ex.Message;

                    //return
                    return this.OnGet();
                }
            }
            else
            {
                return this.OnGet();
            }
        }
    }
}