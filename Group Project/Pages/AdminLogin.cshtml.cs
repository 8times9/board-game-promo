using Group_Project.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Group_Project.Pages
{
    public class AdminLoginModel : PageModel
    {
        public string Message { get; set; } = string.Empty;
        public string MessageColor { get; set; } = "Red";


        //The following is added for bound html element on the Razor page
        [BindProperty]
        //Define the validation customization prior to the property
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter your username")]
        [StringLength(100)]

        public string AdminUsername { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Password")]
        [StringLength(100)]
        public string AdminPassword { get; set; } = string.Empty;

        private readonly IConfiguration configuration;

        public AdminLoginModel(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            //Get the connection to your database using the 'name value' that you created in appsettings.json
            var strConn = configuration.GetConnectionString("DefaultConnection");

            //create an instance of the SQLConnection class.
            using (SqlConnection sqlConn = new(strConn))
            {
                //Use a SqlDataAdapter to execute stored procedure
                SqlDataAdapter sqlValidateUser = new SqlDataAdapter("spValidateUser", sqlConn);
                sqlValidateUser.SelectCommand.CommandType = CommandType.StoredProcedure;

                //input parameters
                sqlValidateUser.SelectCommand.Parameters.AddWithValue("@Username", AdminUsername);
                sqlValidateUser.SelectCommand.Parameters.AddWithValue("@Password", AdminPassword);

                //Use a try catch to execute
                try
                {
                    DataSet dsUserRecord = new DataSet();

                    sqlValidateUser.Fill(dsUserRecord);

                    if (dsUserRecord.Tables[0].Rows.Count == 0)
                    {
                        //If count was 0. the credentials were not valid.
                        Message = "Invalid login, please try again.";
                        return Page();
                    }
                    else
                    {
                        Account currentUser = new();
                        currentUser.AccountID = Convert.ToInt32(dsUserRecord.Tables[0].Rows[0]["AccountID"]);
                        currentUser.Username = dsUserRecord.Tables[0].Rows[0]["Username"].ToString();

                        HttpContext.Session.SetString("AccountID", currentUser.AccountID.ToString());
                        HttpContext.Session.SetString("Username", currentUser.Username);

                        AdminUsername = String.Empty;
                        AdminPassword = String.Empty;
                        ModelState.Clear();

                        return Redirect("/Admin");
                    }
                }
                catch (Exception exc)
                {
                    Message = exc.Message;
                    MessageColor = "Red";
                    return Page();
                }
            }
        }
    }
}