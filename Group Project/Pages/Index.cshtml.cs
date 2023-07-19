using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Group_Project.Data;
using Group_Project.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Group_Project.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public string EmailAddress { get; set; } = String.Empty;

        public string Message { get; set; } = string.Empty;

        private readonly IConfiguration configuration;

        private readonly MyApplicationDbContext _myApplicationDbContext;

        public IndexModel(MyApplicationDbContext myApplicationDbContext, IConfiguration _configuration)
        {
            _myApplicationDbContext = myApplicationDbContext;
            configuration = _configuration;
        }

        public List<Post> Posts { get; set; } = new List<Post>();

        public IActionResult OnGet()
        {
            IQueryable<Post> PostsIQ = (from P in _myApplicationDbContext.Post orderby P.Date descending select P).Take(3);

            Posts = PostsIQ.ToList();

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
                SqlCommand AddEmailCmd = new("spAddEmail", sqlConn);
                AddEmailCmd.CommandType = CommandType.StoredProcedure;

                //Build our input parameter.
                AddEmailCmd.Parameters.AddWithValue("@EmailAddress", EmailAddress);

                try
                {
                    //Open the database
                    sqlConn.Open();

                    //use ExecuteNonQuery to execute our stored procedure
                    AddEmailCmd.ExecuteNonQuery();

                    Message = "Email successfully added";

                    //Clear the fields
                    EmailAddress = "";
                    ModelState.Clear();

                    //return
                    return this.OnGet();
                }
                catch (SqlException exc)
                {
                    if (exc.Number == 2601)
                    {
                        Message = "Email is already subscribed";
                    }
                    else
                    {
                        Message = "Email could not be added at this time, please try again later";
                    }
                       
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