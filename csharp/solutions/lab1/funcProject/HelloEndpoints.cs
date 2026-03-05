using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace FuncProject
{
    public class HelloEndpoints
    {
        [Function("Hello")]
        public IActionResult Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hello")]
            HttpRequest req)
        {
            string name = req.Query["name"].FirstOrDefault() ?? "World";
            return new OkObjectResult($"Hello, {name}!");
        }

        [Function("HelloUser")]
        public IActionResult HelloUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "helloUser")]
            HttpRequest req,
            [Microsoft.Azure.Functions.Worker.Http.FromBody] HelloRequest request)
        {
            return new OkObjectResult($"Hello, {request.Name}!");
        }
    }
}
