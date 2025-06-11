namespace backend.Extensions
{
    public static class ServiceResultExtensions
    {
        public static ActionResult ToActionResult<T>(this ServiceResult<T> serviceResult)
        {
            if (serviceResult.IsSuccess)
            {
                return serviceResult.StatusCode switch
                {
                    HttpStatusCode.Created => new CreatedResult(string.Empty, serviceResult.Data),
                    HttpStatusCode.NoContent => new NoContentResult(),
                    _ => new OkObjectResult(serviceResult.Data)
                };
            }

            return serviceResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(serviceResult),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(serviceResult),
                HttpStatusCode.NotFound => new NotFoundObjectResult(serviceResult),
                HttpStatusCode.Conflict => new ConflictObjectResult(serviceResult),
                _ => new ObjectResult(serviceResult) { StatusCode = (int)serviceResult.StatusCode }
            };
        }
    }
}
