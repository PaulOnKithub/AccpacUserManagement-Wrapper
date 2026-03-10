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
        [ResponseType(typeof(List<UserDto>))]
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
        [ResponseType(typeof(UserDto))]
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



    }
}