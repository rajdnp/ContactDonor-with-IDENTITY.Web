using ContactDonor.Web.Helpers;
using ContactDonor.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContactDonor.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return Json(new { data = roles });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleClaims(string roleid)
        {
            var role = await roleManager.FindByIdAsync(roleid);

            var result = await roleManager.GetClaimsAsync(role);

            if (result != null)
            {
                return Json(new
                {
                    roleclaims = result
                });
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to get the role claims"
            });

        }

        [HttpGet]
        public async Task<IActionResult> GetUserClaims(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            var result = await userManager.GetClaimsAsync(user);

            if (result != null)
            {
                return Json(new
                {
                    userclaims = result
                });
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to get the user claims"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return Json(new Response
                    {
                        Status = StatusTypes.Success,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Role created successfully"
                    });
                }
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to create the role."
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] EditRoleViewModel model, [FromQuery] string roleId)
        {

            if (ModelState.IsValid && roleId != null)
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    role.Name = model.RoleName;
                    var result = await roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return Json(new Response
                        {
                            Status = StatusTypes.Success,
                            StatusCode = HttpStatusCode.OK,
                            Message = "Role updated successfully"
                        });
                    }
                }
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to update the role."
            });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromQuery] string roleId)
        {
            if (roleId != null)
            {
                var role = await roleManager.FindByIdAsync(roleId);

                if (role != null)
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return Json(new Response
                        {
                            Status = StatusTypes.Success,
                            StatusCode = HttpStatusCode.OK,
                            Message = "Role deleted successfully"
                        });
                    }
                }
            }

            return Json(new Response
            {
                Status = StatusTypes.Success,
                StatusCode = HttpStatusCode.OK,
                Message = "Role deleted successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (users != null)
            {
                return Json(new
                {
                    users = users,
                    response = new Response
                    {
                        Status = StatusTypes.Success,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Returned all users."
                    }
                });
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to return users."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserRoles(string email)
        {
            if (email != null)
            {
                var user = await userManager.FindByEmailAsync(email);
                var roles = await userManager.GetRolesAsync(user);

                if (roles != null)
                {
                    return Json(new
                    {
                        roles = roles
                    });
                }
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to return user roles.."
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchUser([FromQuery] string email)
        {
            if (email != null)
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var result = new SearchUser
                    {
                        Email = user.Email,
                        Suggestion = user.Email
                    };

                    await Task.Delay(2000);
                    return Json(new
                    {
                        user = result
                    });
                }
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to search user."
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddClaimToUser([FromBody] string claim, [FromQuery] string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            var result = await userManager.AddClaimAsync(user, new Claim(claim, claim));

            if (result.Succeeded)
            {
                return Json(new Response
                {
                    Status = StatusTypes.Success,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Claim added successfully to the user"
                });
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to add claim to the user"
            });


        }

        [HttpPost]
        public async Task<IActionResult> AddClaimToRole([FromBody] string claim, [FromQuery] string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            var result = await roleManager.AddClaimAsync(role, new Claim(claim, claim));

            if (result.Succeeded)
            {
                return Json(new Response
                {
                    Status = StatusTypes.Success,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Claim added successfully to the role"
                });
            }

            return Json(new Response
            {
                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Failed to add claim to the role"
            });


        }


        [HttpPost]
        public async Task<IActionResult> AddUserToRole([FromQuery] string roleid, [FromQuery] string email)
        {
            if (roleid != null && email != null)
            {
                var role = await roleManager.FindByIdAsync(roleid);

                if (role != null)
                {
                    var user = await userManager.FindByEmailAsync(email);

                    if (user != null && !await userManager.IsInRoleAsync(user, role.Name))
                    {
                        var result = await userManager.AddToRoleAsync(user, role.Name);

                        if (result.Succeeded)
                        {
                            return Json(new Response
                            {
                                Status = StatusTypes.Success,
                                StatusCode = HttpStatusCode.OK,
                                Message = "User added to the role successfully"
                            });
                        }
                    }
                }
            }

            return Json(new Response
            {

                Status = StatusTypes.Failed,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Invalid request"

            });
        }

    }

    public class SearchUser
    {
        public string Email { get; set; }
        public string Suggestion { get; set; }
    }
}
