using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Base.Common;
using TFU.Common;
using Base.Common.Extension;
namespace Base.API
{
    public class BaseAPIController : ControllerBase
    {
        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="data">The extend data.</param>
        /// <returns></returns>
        protected ActionResult Error(string message, object data = null)
        {
            return new BadRequestObjectResult(new TFUResponse
            {
                Data = data,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = message
            });
        }


        protected ActionResult Success(string message, object data = null)
        {
            return Ok(new TFUResponse
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = System.Net.HttpStatusCode.OK
            });
        }


        protected ActionResult Conflict(string message, object data = null)
        {
            return Conflict(new TFUResponse
            {
                Success = false,
                Data = data,
                Message = message,
                StatusCode = System.Net.HttpStatusCode.Conflict
            });
        }

        protected ActionResult Forbiden(string message)
        {
            return Forbiden(message);
        }
        /// <summary>
        /// Gets the data failed.
        /// </summary>
        /// <returns></returns>
        protected ActionResult GetError()
        {
            return Error(Constants.GetDataFailed);
        }


        /// <summary>
        /// Gets the data failed.
        /// </summary>
        /// <returns></returns>
        protected ActionResult GetError(string message)
        {
            return Error(message);
        }

        /// <summary>
        /// Saves the data failed.
        /// </summary>
        /// <returns></returns>
        protected ActionResult SaveError(object data = null)
        {
            return Error(Constants.SaveDataFailed, data);
        }

        /// <summary>
        /// Models the invalid.
        /// </summary>
        /// <returns></returns>
        protected ActionResult ModelInvalid()
        {
            var errors = ModelState.Where(m => m.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key.ToCamelCase(),
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).First()).ToList();
            return new BadRequestObjectResult(new TFUResponse
            {
                Errors = errors,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = Constants.SaveDataFailed
            });
        }

        /// <summary>
        /// Successes request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected ActionResult Success(object data, string message)
        {
            return new OkObjectResult(new TFUResponse
            {
                Data = data,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = message,
                Success = true
            });
        }

        /// <summary>
        /// Gets the data successfully.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected ActionResult GetSuccess(object data)
        {
            return Success(data, Constants.GetDataSuccess);
        }

        /// <summary>
        /// Saves the data successfully
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected ActionResult SaveSuccess(object data)
        {
            return Success(data, Constants.SaveDataSuccess);
        }

        protected ActionResult CheckSucess(object data)
        {
            return Success(data, Constants.CheckSuccess);
        }


        protected ActionResult CheckError(object data)
        {
            return Error(Constants.CheckFailed, data);
        }



        /// <summary>
        /// Get the loged in UserName;
        /// </summary>
        protected string UserName => User.FindFirst(ClaimTypes.Name)?.Value;

        /// <summary>
        /// Get the logged in user email.
        /// </summary>
        protected string UserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

        /// <summary>
        /// Get the loged in UserId;
        /// </summary>
        protected int UserId
        {
            get
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(id, out int userId);
                return userId;
            }
        }

        /// <summary>
        /// The boolean value that determined whether current user is a Seller
        /// </summary>
        protected bool IsSeller
        {
            get
            {
                var isseller = User.FindFirst(Constants.IS_SELLER)?.Value;
                bool.TryParse(isseller, out bool isSeller);
                return isSeller;
            }
        }

        protected bool IsAdmin
        {
            get
            {
                var isadmin = User.FindFirst(Constants.IS_ADMIN)?.Value;
                bool.TryParse(isadmin, out bool isAdmin);
                return isAdmin;
            }
        }

        protected bool IsCreator
        {
            get
            {
                var iscreator = User.FindFirst(Constants.IS_CREATOR)?.Value;
                bool.TryParse(iscreator, out bool isCreator);
                return isCreator;
            }
        }


        protected string ReffererCode => User.FindFirst(Constants.REFFER_CODE)?.Value;

        protected long EventId
        {
            get
            {
                return HttpContext.GetEventId();
            }
        }

        protected string EventCode
        {
            get
            {
                return HttpContext.GetEventCode();
            }
        }

    }
}
