using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.DTO;
//using API.Security;
using DAL.Model;
using BLL.Eventhub;
using BLL.Service;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private IMapper _mapper;
        private readonly PaymentService _orderService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger, IUnitOfWork uow, IMapper mapper, IConfiguration config, IApproveEventSenderFactory msgSernderFactory) //, IGameMessageSenderFactory msgSernderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _orderService ??= new PaymentService(uow, config, msgSernderFactory); //msgSernderFactory);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<PaymentDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetAll()
        {
            List<Payments> result = await _orderService.GetAll();
            if (result != null)
            {
                List<PaymentDTO> mappedResult = _mapper.Map<List<PaymentDTO>>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Get all by user id
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("all/{userId}")]
        [ProducesResponseType(typeof(List<PaymentDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetByUserId([FromRoute] Guid userId)
        {
            List<Payments> result = await _orderService.GetAllPaymentByUserAsync(userId);
            if (result != null)
            {
                List<PaymentDTO> mappedResult = _mapper.Map<List<PaymentDTO>>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }


        /// <summary>
        /// Get pending order by user to use as cart
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("{orderId}")]
        [ProducesResponseType(typeof(List<PaymentDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetPaymentByOrderId([FromRoute] Guid orderId)
        {
            Payments result = await _orderService.GetPaymentByOrderId(orderId);
            if (result != null)
            {
                PaymentDTO mappedResult = _mapper.Map<PaymentDTO>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }


        /// <summary>
        /// Update order 
        /// </summary>
        /// <param order="order">order data.</param>
        /// <response code="200">Request ok.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(PaymentDTO), 200)]
        public async Task<ActionResult> Update([FromBody] PaymentDTO paymentDTO)
        {
            Payments p = _mapper.Map<Payments>(paymentDTO);
            await _orderService.Pay(p);
            return new OkResult();
        }
    }
}
