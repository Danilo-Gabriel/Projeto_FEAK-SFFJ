// using FeakApi.Domain.Commands.Requests;
// using FeakApi.Domain.Commands.responses;
// using FeakApi.Domain.Handlers;
// using Microsoft.AspNetCore.Mvc;
//
// namespace FeakApi.Controllers
// {
//
//     [ApiController]
//     [Route("customers")]
//     public class CustomerController : ControllerBase
//     {
//
//
//         [HttpPost]
//         public CreateCustomerResponse Create(
//             [FromServices] ICreateCustomerHandler handler, [FromBody] CreateCustomerRequest command)
//
//         { 
//             return handler.Handle(command);
//         }
//     }
// }
