namespace FlashcardApp.Api.Extensions
{
    public static class ServiceResultExtensions
    {
        public static ActionResult<ServiceResult<T>> ToActionResult<T>(this ServiceResult<T> serviceResult)
        {
            if (serviceResult.IsSuccess)
            {
                return serviceResult.StatusCode switch
                {
                    HttpStatusCode.Created => new CreatedResult(string.Empty, serviceResult),
                    HttpStatusCode.NoContent => new NoContentResult(),
                    _ => new OkObjectResult(serviceResult)
                };
            }

            return serviceResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(serviceResult),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(serviceResult),
                HttpStatusCode.NotFound => new NotFoundObjectResult(serviceResult),
                HttpStatusCode.Conflict => new ConflictObjectResult(serviceResult),
                HttpStatusCode.Forbidden => new ForbidResult(),
                _ => new ObjectResult(serviceResult) { StatusCode = (int)serviceResult.StatusCode }
            };
        }
    }
}
