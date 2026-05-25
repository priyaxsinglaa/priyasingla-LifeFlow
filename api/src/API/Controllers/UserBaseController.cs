using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.DTOs;
namespace API.Controllers
{
    public class UserBaseController(IUserInterface service, IDonationService donationService): BaseAPIController
    {
        private readonly IUserInterface _service = service;
        private readonly IDonationService _donationService = donationService;
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("You are authorized");
        }
        private int CurrentUserId 
        {
            get
            {
                var claim = User.FindFirst("UserId");
                return claim == null ? 0 : int.Parse(claim.Value);
            }
        }
        [Authorize]
        [HttpPost("Onboarding")]
        public async Task<IActionResult> SaveOnboarding(OnBoardingDto dto)
        {
            var result = await _service.SaveOnboarding(CurrentUserId, dto);
            return Ok(new { Message = result });
        }
        [Authorize]
        [HttpPost("DonationDetails")]
        public async Task<IActionResult> AddDonation(DonationDto dto)
        {
            
            var result = await _donationService.AddDonation(CurrentUserId, dto);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("DonationCount")]
        public async Task<IActionResult> GetTotalCount()
        {
            var count = await _donationService.GetTotalDonationCount(CurrentUserId);
            return Ok(new { TotalDonations = count });
        }
    }
}
