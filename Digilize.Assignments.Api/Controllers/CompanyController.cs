using Digilize.Domain.Entities;
using Digilize.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Digilize.Domain.Entities;
using Digilize.Infrastructure.Repositories;

namespace Digilize.Assignments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IRepository<Company> _companyRepository;

        public CompanyController(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies = await _companyRepository.GetAllAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return NotFound($"Company with ID {id} not found.");

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] Company company)
        {
            if (company == null)
                return BadRequest("Invalid company data.");

            await _companyRepository.AddAsync(company);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] Company updatedCompany)
        {
            if (updatedCompany == null || id != updatedCompany.Id)
                return BadRequest("Invalid company data.");

            var existingCompany = await _companyRepository.GetByIdAsync(id);
            if (existingCompany == null)
                return NotFound($"Company with ID {id} not found.");

            existingCompany.CompanyName = updatedCompany.CompanyName;
            existingCompany.Address = updatedCompany.Address;

            await _companyRepository.UpdateAsync(existingCompany);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var existingCompany = await _companyRepository.GetByIdAsync(id);
            if (existingCompany == null)
                return NotFound($"Company with ID {id} not found.");

            await _companyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}