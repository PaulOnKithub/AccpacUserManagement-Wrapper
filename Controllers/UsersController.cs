using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using AccpacUserManagement_Wrapper.Models;
using AccpacUserManagement_Wrapper.Services.Sage300Services;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPatchAttribute = System.Web.Http.HttpPatchAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace AccpacUserManagement_Wrapper.Controllers
{

    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {

        private readonly UserService userService;

        public UsersController()
        {
            userService = new UserService();
        }

        /// <summary>
        /// Gets all customers from Sage 300.
        /// </summary>
        /// <returns>List of customers.</returns>
        /// <response code="200">Returns the list of customers</response>
        /// <response code="500">If there is an internal server error</response>
        [System.Web.Http.HttpGet]
        [Route("")]
        [ResponseType(typeof(List<UserAttributesDto>))]
        public async Task<IHttpActionResult> getUsers()
        {
            try
            {
                var users = await userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Gets a specific user by ID.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <returns>The customer details.</returns>
        /// <response code="200">Returns the customer</response>
        /// <response code="404">If the customer is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserAttributesDto))]
        public async Task<IHttpActionResult> getUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("User ID is required");
                }

                var user = await userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("")]
        [ResponseType(typeof(UserAttributesDto))]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] UserAttributesDto user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is required");
                }

                if (string.IsNullOrWhiteSpace(user.UserId))
                {
                    return BadRequest("User ID is required");
                }

                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    return BadRequest("User Name is required");
                }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest("Password is required");
                }

                var createdUser = await userService.CreateUser(user);

                return Created(
                    new Uri(Request.RequestUri + "/" + createdUser.UserId),
                    createdUser
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("must include"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);

            }
        }

        [HttpPatch]
        [Route("disable/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> disableUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("User ID is required");
                }
                //await userService.GetUserByIdAsync(id);

                await userService.DisableUser(id);

                return Ok();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPatch]
        [Route("enable/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> enableUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("User ID is required");
                }
                //await userService.GetUserByIdAsync(id);

                await userService.EnableUser(id);

                return Ok();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        [HttpGet]
        [Route("roles")]
        [ResponseType(typeof(List<RolesDto>))]
        public async Task<IHttpActionResult> getUserRoles()
        {
            try
            {
                var userRoles = await userService.GetUserRolesAsync();
                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("userroles")]
        [ResponseType(typeof(List<UserRolesDto>))]
        public async Task<IHttpActionResult> getUsersWithRoles()
        {
            try
            {
                var usersWithRoles = await userService.GetUserRoles();
                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }


}