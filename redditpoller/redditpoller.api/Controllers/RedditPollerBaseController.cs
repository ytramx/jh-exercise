using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using redditpoller.api.Models;

namespace redditpoller.api.Controllers
{
    [ApiController]
    public class RedditPollerBaseController : ControllerBase
    {
        public override CreatedResult Created(string uri, [ActionResultObjectValue] object? value)
        {
            var retval = new ResponseContainer<object?>(value, true);
            return base.Created(uri, retval);
        }
    }
}
